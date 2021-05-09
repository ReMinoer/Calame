using System;
using Gemini.Framework;

namespace Calame
{
    public interface IRunnableDocument : IDocument
    {
        Type RunCommandDefinitionType { get; }
    }
}