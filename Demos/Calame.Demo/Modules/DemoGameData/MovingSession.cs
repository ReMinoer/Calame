using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.SceneViewer;
using Glyph.Core;
using Glyph.Core.Inputs;
using Glyph.Graphics.Renderer;
using Glyph.Graphics.Shapes;
using Fingear.Controls;
using Fingear.Inputs.Converters;
using Fingear.MonoGame;
using Glyph;
using Glyph.Content;
using Glyph.Graphics;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Calame.Demo.Modules.DemoGameData
{
    [Export(typeof(ISession))]
    public class MovingSession : ISession
    {
        public string DisplayName => "Moving Demo";
        public bool ShowAsNewFileType => true;

        public IContentLibrary CreateContentLibrary(IGraphicsDeviceService graphicsDeviceService, ILogger logger) => new UnusedContentLibrary();

        private const string GameViewRootKey = nameof(GameViewRootKey);
        private const string GameViewKey = nameof(GameViewKey);

        public Task PrepareSessionAsync(ISessionContext sessionContext)
        {
            var gameViewRoot = sessionContext.Engine.Root.Add<GlyphObject>();
            gameViewRoot.Add<SceneNode>().MakesRoot();

            var gameView = gameViewRoot.Add<UniformFillTargetView>();
            gameView.UniformView.Name = "Game View";
            gameView.ParentView = sessionContext.RootView;
            gameView.Size = new Vector2(1920, 1080);

            sessionContext.Engine.Root.SetKeyedComponent(GameViewRootKey, gameViewRoot);
            gameViewRoot.SetKeyedComponent(GameViewKey, gameView);

            return Task.CompletedTask;
        }

        public Task ResetSessionAsync(ISessionContext sessionContext)
        {
            sessionContext.UserRoot.ClearAndDisposeComponents();

            var gameRoot = sessionContext.UserRoot.Add<GlyphObject>();
            gameRoot.Name = "Game Root";
            gameRoot.Add<SceneNode>();

            var gameViewRoot = sessionContext.Engine.Root.GetKeyedComponent<GlyphObject>(GameViewRootKey);
            var gameView = gameViewRoot.GetKeyedComponent<UniformFillTargetView>(GameViewKey);
            gameView.Camera = gameRoot.Add<Camera>();

            var player = gameRoot.Add<GlyphObject>();
            player.Name = "Player";
            var playerSceneNode = player.Add<SceneNode>();
            player.Add<FilledRectangleSprite>();
            player.Add<SpriteRenderer>();

            sessionContext.SessionInteractive.Add(gameRoot.Add<InteractiveRoot>().Interactive);

            var playerMoveInput = new Control<System.Numerics.Vector2>(InputSystem.Instance.Keyboard[Keys.Left, Keys.Right, Keys.Down, Keys.Up].Vector(-System.Numerics.Vector2.One, System.Numerics.Vector2.One));
            player.Add<Controls>().Add(playerMoveInput);

            player.Schedulers.Update.Plan(elapsedTime =>
            {
                const float speed = 1000f;
                if (playerMoveInput.IsActive(out System.Numerics.Vector2 inputVector))
                    playerSceneNode.Position += inputVector.AsMonoGameVector().Normalized() * speed * elapsedTime.Delta;
            });

            return Task.CompletedTask;
        }
    }
}