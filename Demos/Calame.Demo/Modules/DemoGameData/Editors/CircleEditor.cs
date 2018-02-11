﻿using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Calame.DataModelViewer;
using Calame.Demo.Data;
using Glyph.Composition.Modelization;
using Glyph.IO;

namespace Calame.Demo.Modules.DemoGameData.Editors
{
    [Export(typeof(IEditor))]
    public class CircleEditor : ViewerEditorBase<CircleData>
    {
        public override string ContentPath => "Content/";
        protected override ISaveLoadFormat<CircleData> SaveLoadFormat => new XmlSerializationFormat<CircleData>("Circle", ".circle");
        public override async Task<IGlyphCreator> NewDataAsync() => new CircleData();
    }
}