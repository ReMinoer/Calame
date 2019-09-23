using System;
using System.Collections.Generic;
using System.IO;
using Glyph;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;

namespace Calame
{
    public class ContentLibraryProvider : IContentLibraryProvider
    {
        static private readonly IServiceProvider ServiceProvider = new DummyServiceProvider(D3D11Client.GraphicsDevice);
        static private readonly IContentLibrary NullContentLibrary = new UnusedContentLibrary(ServiceProvider);

        private readonly Dictionary<string, IContentLibrary> _contentLibraries = new Dictionary<string, IContentLibrary>(StringComparer.OrdinalIgnoreCase);

        public IContentLibrary Get(string path)
        {
            if (path == null)
                return NullContentLibrary;

            string fullPath = Path.GetFullPath(path);
            if (!_contentLibraries.TryGetValue(fullPath, out IContentLibrary contentManager))
                _contentLibraries.Add(fullPath, contentManager = new ContentLibrary(ServiceProvider, path));

            return contentManager;
        }

        public bool Remove(string path)
        {
            return _contentLibraries.Remove(Path.GetFullPath(path));
        }

        private sealed class DummyServiceProvider : IServiceProvider
        {
            private readonly DummyGraphicsDeviceService _graphicsDeviceService;

            public DummyServiceProvider(GraphicsDevice graphicsDevice)
            {
                _graphicsDeviceService = new DummyGraphicsDeviceService(graphicsDevice);
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(IGraphicsDeviceService))
                    return _graphicsDeviceService;

                throw new InvalidOperationException();
            }

            private sealed class DummyGraphicsDeviceService : IGraphicsDeviceService
            {
                public GraphicsDevice GraphicsDevice { get; }

                public DummyGraphicsDeviceService(GraphicsDevice graphicsDevice)
                {
                    GraphicsDevice = graphicsDevice;
                }

                public event EventHandler<EventArgs> DeviceCreated { add { } remove { } }
                public event EventHandler<EventArgs> DeviceDisposing { add { } remove { } }
                public event EventHandler<EventArgs> DeviceReset { add { } remove { } }
                public event EventHandler<EventArgs> DeviceResetting { add { } remove { } }
            }
        }
    }
}