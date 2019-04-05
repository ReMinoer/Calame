using Fingear;
using Fingear.Interactives;

namespace Calame.Viewer
{
    public class EditorSessionInteractive : InteractiveContainer<IInteractive>
    {
        private bool _editionMode;

        public bool EditionMode
        {
            get => _editionMode;
            set
            {
                if (_editionMode == value)
                    return;

                _editionMode = value;

                if (_editionMode)
                {
                    Session.Reset();
                    Editor.Update(0);
                }
                else
                {
                    Editor.Reset();
                    Session.Update(0);
                }
            }
        }

        public IInteractive Editor
        {
            get => Components[0];
            set => Components[0] = value;
        }

        public IInteractive Session
        {
            get => Components[1];
            set => Components[1] = value;
        }

        protected override void UpdateEnabled(float elapsedTime)
        {
            if (EditionMode)
                Editor.Update(elapsedTime);
            else
                Session.Update(elapsedTime);
        }

        public override void Reset()
        {
            if (EditionMode)
                Editor.Reset();
            else
                Session.Reset();
        }
    }
}