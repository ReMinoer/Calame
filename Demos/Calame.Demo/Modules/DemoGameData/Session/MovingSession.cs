using System;
using System.ComponentModel.Composition;
using Calame.SceneViewer;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Engine;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Fingear.Controls;
using Fingear.Converters;
using Fingear.MonoGame;
using Glyph;
using Glyph.Graphics;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Calame.Demo.Modules.DemoGameData.Session
{
    [Export(typeof(ISession))]
    public class MovingSession : ISession
    {
        private Viewport _viewport;
        public string DisplayName => "Moving Demo";
        public string ContentPath => "Content/";
        
        public void PrepareSession(GlyphEngine engine)
        {
            var view = engine.Injector.Resolve<View>();
            view.BoundingBox = new TopLeftRectangle(Vector2.Zero, VirtualResolution.Size);
            ViewManager.Main.RegisterView(view);
            
            engine.Root.Add<SceneNode>();
            
            var camera = engine.Root.Add<Camera>();
            view.Camera = camera;
            
            var movableObject = engine.Root.Add<GlyphObject>();
            var sceneNode = movableObject.Add<SceneNode>();

            var moveInput = new Control<System.Numerics.Vector2>(InputSystem.Instance.Keyboard[Keys.Left, Keys.Right, Keys.Down, Keys.Up].Vector(-System.Numerics.Vector2.One, System.Numerics.Vector2.One));
            movableObject.Add<Controls>().Register(moveInput);
            
            movableObject.Add<FilledRectangleSprite>();
            movableObject.Add<SpriteRenderer>();
            
            movableObject.Schedulers.Update.Plan(elapsedTime =>
            {
                if (moveInput.IsActive(out System.Numerics.Vector2 inputVector))
                    sceneNode.Position += inputVector.AsMonoGameVector().Normalized() * 1000f * elapsedTime.Delta;
            });
            
            var pixel = new Texture2D(engine.Injector.Resolve<Func<GraphicsDevice>>()(), 1, 1);
            pixel.SetData(new[] { Color.White });

            engine.Root.Schedulers.Draw.Plan(drawer =>
            {
                drawer.GraphicsDevice.SetRenderTarget(drawer.DefaultRenderTarget);
                _viewport = drawer.GraphicsDevice.Viewport;

                float targetAspectRatio = VirtualResolution.AspectRatio;

                int width = _viewport.Width;
                int height = (int)(width / targetAspectRatio + .5f);

                if (height > _viewport.Height)
                {
                    height = _viewport.Height;
                    width = (int)(height * targetAspectRatio + .5f);
                }

                var viewport = new Viewport
                {
                    X = _viewport.Width / 2 - width / 2,
                    Y = _viewport.Height / 2 - height / 2,
                    Width = width,
                    Height = height,
                    MinDepth = 0,
                    MaxDepth = 1
                };

                drawer.GraphicsDevice.Viewport = viewport;

                drawer.SpriteBatchStack.Push(SpriteBatchContext.Default);
                drawer.SpriteBatchStack.Current.Draw(pixel, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.CornflowerBlue);
                drawer.SpriteBatchStack.Pop();

                var drawSceneContext = new SpriteBatchContext
                {
                    SpriteSortMode = SpriteSortMode.Deferred,
                    TransformMatrix = drawer.ViewMatrix * drawer.ResolutionMatrix
                };

                drawer.SpriteBatchStack.Push(drawSceneContext);

            }).AtStart();

            engine.Root.Schedulers.Draw.Plan(drawer =>
            {
                drawer.SpriteBatchStack.Pop();
                drawer.GraphicsDevice.Viewport = _viewport;
            }).AtEnd();
        }
    }
}