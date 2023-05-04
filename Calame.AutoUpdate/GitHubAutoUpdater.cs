using Octokit;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Calame.AutoUpdate
{
    public class GitHubAutoUpdater
    {
        private const string MessageCaption = "Auto-Update";

        static public async Task<string> CheckUpdatesAndAskUserToDownload(IAutoUpdateConfiguration configuration, ILogger logger)
        {
            GitHubClient gitHubClient = await GetGitHubClientAsync(configuration, logger);
            if (gitHubClient is null)
                return null;

            Release latestRelease = await CheckUpdatesAsync(gitHubClient, configuration, logger);
            if (latestRelease is null)
                return null;
            
            return await DownloadInstallerAsset(gitHubClient, configuration, latestRelease);
        }

        static private async Task<GitHubClient> GetGitHubClientAsync(IAutoUpdateConfiguration configuration, ILogger logger)
        {
            if (configuration?.ProductId is null)
                return null;

            string message;

            var gitHubClient = new GitHubClient(new ProductHeaderValue(configuration.ProductId));

            string applicationOAuthId = configuration.ApplicationOAuthId;
            string applicationOAuthSecret = configuration.ApplicationOAuthSecret;

            if (applicationOAuthId == null || applicationOAuthSecret == null)
                return gitHubClient;

            Uri loginUri;
            string requestState;
            HttpListener httpListener;
            try
            {
                (loginUri, requestState, httpListener) = GitHubAutoUpdateApi.GetLoginUri(gitHubClient, applicationOAuthId);
            }
            catch (Exception ex)
            {
                message = "Failed to get the login URL on GitHub.";

                logger.LogError(ex, message);
                MessageBox.Show(message, MessageCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            var webBrowserDialog = new WebBrowserDialog(loginUri, configuration.WebViewUserDataFolder)
            {
                Owner = System.Windows.Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Topmost = true,
                Width = 600,
                Height = 800,
                Title = "Connect to GitHub..."
            };
            webBrowserDialog.Closed += OnClosed;
            webBrowserDialog.Show();

            void OnClosed(object sender, EventArgs e) => httpListener.Close();

            HttpListenerContext httpListenerContext;
            try
            {
                httpListenerContext = await httpListener.GetContextAsync();
            }
            catch (Exception ex)
            {
                webBrowserDialog.Close();

                message = "Authentication to GitHub failed.";

                logger.LogError(ex, message);
                MessageBox.Show(message, MessageCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            webBrowserDialog.Close();

            try
            {
                await GitHubAutoUpdateApi.GetAccessTokenAsync(gitHubClient, applicationOAuthId, applicationOAuthSecret, requestState, httpListenerContext);
            }
            catch (Exception ex)
            {
                message = "Failed to connect to GitHub.";

                logger.LogError(ex, message);
                MessageBox.Show(message, MessageCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return gitHubClient;
        }

        static private async Task<Release> CheckUpdatesAsync(GitHubClient gitHubClient, IAutoUpdateConfiguration configuration, ILogger logger)
        {
            string repositoryOwner = configuration.RepositoryOwner;
            string repositoryName = configuration.RepositoryName;
            string message;

            Release latestRelease;
            try
            {
                latestRelease = await GitHubAutoUpdateApi.GetLatestRelease(gitHubClient, repositoryOwner, repositoryName);
            }
            catch (Exception ex)
            {
                message = "Failed to get the latest release tag.";
                    
                logger.LogError(ex, message);
                MessageBox.Show(message, MessageCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            if (CalameUtils.IsDevelopmentBuild())
            {
                message = $"Latest version is: {latestRelease.TagName}.\n\n(This development build will not be updated.)";

                MessageBox.Show(message, MessageCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }

            if (latestRelease.TagName == CalameUtils.GetVersion())
            {
                message = $"You are using the latest version. ({latestRelease.TagName})";

                MessageBox.Show(message, MessageCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }

            if (latestRelease.Assets.All(x => x.Name != configuration.InstallerAssetName))
            {
                message = $"Latest version is {latestRelease.TagName} but no installer is available yet. Retry to update in a few minutes.";

                MessageBox.Show(message, MessageCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }

            MessageBoxResult messageBoxResult = MessageBox.Show($"Do you want to download the last version ({latestRelease.TagName}) ?", MessageCaption,
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

            return messageBoxResult == MessageBoxResult.Yes ? latestRelease : null;
        }

        static private async Task<string> DownloadInstallerAsset(GitHubClient gitHubClient, IAutoUpdateConfiguration configuration, Release release)
        {
            string downloadFolderPath = GetRandomFolderPath(Path.GetTempPath());
            string installerAssetName = configuration.InstallerAssetName;
            byte[] assetBytes = await GitHubAutoUpdateApi.DownloadAsset(gitHubClient, release, installerAssetName);

            string assetFilePath = Path.Combine(downloadFolderPath, installerAssetName);
            using (FileStream assetFileStream = File.Create(assetFilePath))
            {
                await assetFileStream.WriteAsync(assetBytes);
            }

            return assetFilePath;
        }

        static private string GetRandomFolderPath(string parentFolderPath)
        {
            string folderPath;
            do
            {
                folderPath = Path.Combine(parentFolderPath, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
            }
            while (Directory.Exists(folderPath));

            return folderPath;
        }
    }
}