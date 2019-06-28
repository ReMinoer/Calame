using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Glyph.Composition;
using Glyph.Tools;
using Glyph.Tools.Brushing;
using Glyph.Tools.Brushing.Space;

namespace Calame.BrushPanel.ViewModels
{
    public abstract class BrushViewModelBase<TCanvas, TArgs, TPaint> : IBrushViewModel
        where TPaint : IPaint
    {
        [Browsable(false)]
        public abstract string DisplayName { get; }
        [Browsable(false)]
        public abstract object IconKey { get; }
        [Browsable(false)]
        public Type CanvasType { get; } = typeof(TCanvas);
        [Browsable(false)]
        protected abstract IBrush<TCanvas, TArgs, TPaint> Brush { get; }

        private ObservableCollection<IPaintViewModel<TPaint>> _paints;
        [ImportMany, Browsable(false)]
        public IEnumerable<IPaintViewModel<TPaint>> Paints
        {
            get => _paints;
            set => _paints = new ObservableCollection<IPaintViewModel<TPaint>>(value);
        }
        
        IBrush IBrushViewModel.Brush => Brush;
        IEnumerable<IPaintViewModel> IBrushViewModel.Paints => Paints;

        public bool CanStartApply(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.CanStartApply((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void StartApply(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.StartApply((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void UpdateApply(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.UpdateApply((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public bool CanEndApply(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.CanEndApply((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void EndApply(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.EndApply((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void OnInvalidStart(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.OnInvalidStart((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void OnCancellation(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.OnCancellation((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void OnInvalidEnd(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.OnInvalidEnd((TCanvas)canvas, (TArgs)args, (TPaint)paint);
    }
}