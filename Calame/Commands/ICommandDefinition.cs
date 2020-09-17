using System;

namespace Calame.Commands
{
    public interface ICommandDefinition
    {
        string Name { get; }
        string Text { get; }
        string ToolTip { get; }
        Uri IconSource { get; }
        bool IsList { get; }
    }
}