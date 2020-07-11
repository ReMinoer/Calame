using System;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;

namespace Calame
{
    public class WpfGraphicsDeviceService : IGraphicsDeviceService
    {
        static private WpfGraphicsDeviceService _instance;
        static public WpfGraphicsDeviceService Instance => _instance ?? (_instance = new WpfGraphicsDeviceService());

        public GraphicsDevice GraphicsDevice => D3D11Client.GraphicsDevice;

        public event EventHandler<EventArgs> DeviceCreated { add { } remove { } }
        public event EventHandler<EventArgs> DeviceDisposing { add { } remove { } }
        public event EventHandler<EventArgs> DeviceReset { add { } remove { } }
        public event EventHandler<EventArgs> DeviceResetting { add { } remove { } }
    }
}