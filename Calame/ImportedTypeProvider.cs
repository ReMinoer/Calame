using System;
using System.Collections.Generic;
using Diese.Collections.ReadOnly;
using Glyph.Export;

namespace Calame
{
    public class ImportedTypeProvider : IImportedTypeProvider
    {
        public ReadOnlyList<Type> Types { get; }
        IEnumerable<Type> IImportedTypeProvider.Types => Types;

        public ImportedTypeProvider()
        {
            Types = new ReadOnlyList<Type>(TypeImporter.GetTypesFromLocalAssemblies());
        }
    }
}