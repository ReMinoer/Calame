namespace Calame.AutoUpdate
{
    public interface IAutoUpdateConfiguration
    {
        string ProductId { get; }
        string ApplicationOAuthId { get; }
        string ApplicationOAuthSecret { get; }
        string RepositoryOwner { get; }
        string RepositoryName { get; }
        string InstallerAssetName { get; }
        string WebViewUserDataFolder { get; }
    }
}