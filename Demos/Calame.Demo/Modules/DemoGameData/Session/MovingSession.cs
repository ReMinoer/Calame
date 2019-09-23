using System.ComponentModel.Composition;
using Calame.SceneViewer;
using Fingear;
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Calame.Demo.Modules.DemoGameData.Session
{
    [Export(typeof(ISession))]
    public class MovingSession : ISession
    {
        public string DisplayName => "Moving Demo";
        public string ContentPath => null;

        public void PrepareSession(ISessionContext context)
        {
            var gameRoot = context.EditorRoot.Add<GlyphObject>();
            gameRoot.Name = "Game Root";
            gameRoot.Add<SceneNode>();

            var gameViewRoot = context.Engine.Root.Add<GlyphObject>();
            gameViewRoot.Add<SceneNode>().MakesRoot();

            var gameView = gameViewRoot.Add<UniformFillTargetView>();
            gameView.UniformView.Name = "Game View";
            gameView.ParentView = context.RootView;
            gameView.Size = new Vector2(1920, 1080);

            gameView.Camera = gameRoot.Add<Camera>();

            var player = gameRoot.Add<GlyphObject>();
            player.Name = "Player";
            var playerSceneNode = player.Add<SceneNode>();
            player.Add<FilledRectangleSprite>();
            player.Add<SpriteRenderer>();

            context.SessionInteractive.Add(gameRoot.Add<InteractiveRoot>().Interactive);

            var playerMoveInput = new Control<System.Numerics.Vector2>(InputSystem.Instance.Keyboard[Keys.Left, Keys.Right, Keys.Down, Keys.Up].Vector(-System.Numerics.Vector2.One, System.Numerics.Vector2.One));
            player.Add<Controls>().Add(playerMoveInput);
            
            player.Schedulers.Update.Plan(elapsedTime =>
            {
                const float speed = 1000f;
                if (playerMoveInput.IsActive(out System.Numerics.Vector2 inputVector))
                    playerSceneNode.Position += inputVector.AsMonoGameVector().Normalized() * speed * elapsedTime.Delta;
            });
        }
    }
}