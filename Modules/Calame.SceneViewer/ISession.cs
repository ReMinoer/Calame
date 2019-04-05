namespace Calame.SceneViewer
{
    public interface ISession
    {
        string DisplayName { get; }
        string ContentPath { get; }
        void PrepareSession(ISessionContext context);
    }
}