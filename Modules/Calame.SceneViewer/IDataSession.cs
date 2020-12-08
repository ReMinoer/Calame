using Glyph.Composition.Modelization;

namespace Calame.SceneViewer
{
    public interface IDataSession : ISession
    {
        IGlyphData Data { get; }
    }

    public interface IDataSession<TData> : IDataSession
        where TData : IGlyphData
    {
        new TData Data { get; set; }
    }
}