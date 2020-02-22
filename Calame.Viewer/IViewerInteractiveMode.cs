﻿using System.Windows.Input;
using Fingear.Interactives;

namespace Calame.Viewer
{
    public interface IViewerInteractiveMode
    {
        string Name { get; }
        object IconId { get; }
        IInteractive Interactive { get; }
        Cursor Cursor { get; }
        bool UseFreeCamera { get; }
    }
}