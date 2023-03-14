using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace Calame.AutoUpdate
{
    // https://octokitnet.readthedocs.io/en/latest/oauth-flow/
    // https://github.com/googlesamples/oauth-apps-for-windows/blob/master/OAuthDesktopApp/OAuthDesktopApp/MainWindow.xaml.cs
    // https://github.com/IdentityModel/IdentityModel.OidcClient.Samples/blob/main/HttpSysConsoleClient/ConsoleSystemBrowser/Program.cs
    static public class GitHubAutoUpdateApi
    {
        static public (Uri loginUri, string requestState, HttpListener httpListener) GetLoginUri(GitHubClient client, string oauthId)
        {
            var requestStateBuilder = new StringBuilder(32);
            var random = new Random();
            for (int i = 0; i < 32; i++)
                requestStateBuilder.Append((char)('a' + random.Next(0, 26)));

            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            string requestState = requestStateBuilder.ToString();
            string redirectUri = $"http://{IPAddress.Loopback}:{port}/";

            var loginRequest = new OauthLoginRequest(oauthId)
            {
                Scopes = { "repo" },
                State = requestState,
                RedirectUri = new Uri(redirectUri)
            };

            var httpListener = new HttpListener();
            httpListener.Prefixes.Add(redirectUri);
            httpListener.Start();

            return (client.Oauth.GetGitHubLoginUrl(loginRequest), requestState, httpListener);
        }
        
        static public async Task GetAccessTokenAsync(GitHubClient client, string oauthId, string oauthSecret, string requestState, HttpListenerContext httpListenerContext)
        {
            NameValueCollection queryStringCollection = httpListenerContext.Request.QueryString;

            string loginError = queryStringCollection.Get("error");
            if (loginError != null)
                throw new InvalidOperationException($"OAuth login error: {loginError}");

            string returnedState = queryStringCollection.Get("state");
            string returnedCode = queryStringCollection.Get("code");

            if (returnedCode is null || returnedState is null)
                throw new InvalidOperationException($"Malformed authorization response. ({queryStringCollection})");
            if (returnedState != requestState)
                throw new InvalidOperationException("Invalid authorization response state.");

            var tokenRequest = new OauthTokenRequest(oauthId, oauthSecret, returnedCode);
            OauthToken token = await client.Oauth.CreateAccessToken(tokenRequest);

            if (!string.IsNullOrWhiteSpace(token.ErrorDescription))
                throw new InvalidOperationException($"OAuth access token error: {token.ErrorDescription}");

            client.Credentials = new Credentials(token.AccessToken);
        }

        static public async Task<Release> GetLatestRelease(GitHubClient client, string repositoryOwner, string repositoryName)
        {
            return await client.Repository.Release.GetLatest(repositoryOwner, repositoryName);
        }

        static public async Task<byte[]> DownloadAsset(GitHubClient client, Release release, string assetName)
        {
            string assetUrl = release.Assets.FirstOrDefault(x => x.Name == assetName)?.Url;
            if (assetUrl is null)
                return null;

            IApiResponse<object> response = await client.Connection.Get<object>(new Uri(assetUrl), new Dictionary<string, string>(), "application/octet-stream");
            byte[] bytes = Encoding.ASCII.GetBytes(response.HttpResponse.Body.ToString());

            return bytes;
        }
    }
}