using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Calame
{
    public class ContentManagerProvider : IContentManagerProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, ContentManager> _contentManagers = new Dictionary<string, ContentManager>();
        
        public ContentManagerProvider(GraphicsDevice graphicsDevice)
        {
            _serviceProvider = new DummyServiceProvider(graphicsDevice);
        }
        
        public ContentManager Get(string path)
        {
            string fullPath = Path.GetFullPath(path);
            if (!_contentManagers.TryGetValue(fullPath, out ContentManager contentManager))
                _contentManagers.Add(fullPath, contentManager = new ContentManager(_serviceProvider, path));
            return contentManager;
        }

        public bool Remove(string path)
        {
            return _contentManagers.Remove(Path.GetFullPath(path));
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