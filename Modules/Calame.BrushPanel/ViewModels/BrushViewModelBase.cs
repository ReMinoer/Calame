using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Calame.Icons;
using Glyph.Composition;
using Glyph.Composition.Modelization;
using Glyph.Tools.Brushing;
using Glyph.Tools.Brushing.Space;
using Niddle;

namespace Calame.BrushPanel.ViewModels
{
    public abstract class BrushViewModelBase<TCanvas, TArgs, TPaint> : IBrushViewModel
        where TPaint : IPaint
    {
        [Browsable(false)]
        public abstract string DisplayName { get; }
        [Browsable(false)]
        public abstract IconDescription IconDescription { get; }
        [Browsable(false)]
        protected abstract IBrush<TCanvas, TArgs, TPaint> Brush { get; }
        
        private ObservableCollection<IPaintViewModel<TPaint>> _paints;
        [ImportMany, Browsable(false)]
        public IEnumerable<IPaintViewModel<TPaint>> Paints
        {
            get => _paints;
            set => _paints = new ObservableCollection<IPaintViewModel<TPaint>>(value);
        }
        
        IEnumerable<IPaintViewModel> IBrushViewModel.Paints => Paints;

        public bool IsValidForCanvas(object canvas)
        {
            return canvas is TCanvas;
        }

        public IGlyphComponent CreateCursor(IDependencyResolver dependencyResolver) => Brush.CreateCursor(dependencyResolver);
    }

    public abstract class EngineBrushViewModelBase<TCanvas, TArgs, TPaint> : BrushViewModelBase<TCanvas, TArgs, TPaint>, IEngineBrushViewModel
        where TPaint : IPaint
    {
        public void Update(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.Update((TCanvas)canvas, (TArgs)args, (TPaint)paint);
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

    public abstract class DataBrushViewModelBase<TCanvas, TArgs, TPaint> : BrushViewModelBase<TCanvas, TArgs, TPaint>, IDataBrushViewModel
        where TPaint : IPaint
    {
        public void Update(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.Update((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public bool CanStartApply(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.CanStartApply((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void StartApply(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.StartApply((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void UpdateApply(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.UpdateApply((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public bool CanEndApply(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.CanEndApply((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void EndApply(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.EndApply((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void OnInvalidStart(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.OnInvalidStart((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void OnCancellation(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.OnCancellation((TCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void OnInvalidEnd(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => Brush.OnInvalidEnd((TCanvas)canvas, (TArgs)args, (TPaint)paint);
    }

    public abstract class HybridBrushViewModelBase<TComponentCanvas, TDataCanvas, TArgs, TPaint> : IEngineBrushViewModel, IDataBrushViewModel
        where TPaint : IPaint
    {
        [Browsable(false)]
        public abstract string DisplayName { get; }
        [Browsable(false)]
        public abstract IconDescription IconDescription { get; }
        [Browsable(false)]
        protected abstract IBrush<TComponentCanvas, TArgs, TPaint> ComponentBrush { get; }
        [Browsable(false)]
        protected abstract IBrush<TDataCanvas, TArgs, TPaint> DataBrush { get; }

        private ObservableCollection<IPaintViewModel<TPaint>> _paints;
        [ImportMany, Browsable(false)]
        public IEnumerable<IPaintViewModel<TPaint>> Paints
        {
            get => _paints;
            set => _paints = new ObservableCollection<IPaintViewModel<TPaint>>(value);
        }
        
        IEnumerable<IPaintViewModel> IBrushViewModel.Paints => Paints;

        public bool IsValidForCanvas(object canvas)
        {
            return canvas is TComponentCanvas || canvas is TDataCanvas;
        }

        public void Update(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => ComponentBrush.Update((TComponentCanvas)canvas, (TArgs)args, (TPaint)paint);
        public bool CanStartApply(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => ComponentBrush.CanStartApply((TComponentCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void StartApply(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => ComponentBrush.StartApply((TComponentCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void UpdateApply(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => ComponentBrush.UpdateApply((TComponentCanvas)canvas, (TArgs)args, (TPaint)paint);
        public bool CanEndApply(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => ComponentBrush.CanEndApply((TComponentCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void EndApply(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => ComponentBrush.EndApply((TComponentCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void OnInvalidStart(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => ComponentBrush.OnInvalidStart((TComponentCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void OnCancellation(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => ComponentBrush.OnCancellation((TComponentCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void OnInvalidEnd(IGlyphComponent canvas, ISpaceBrushArgs args, IPaint paint)
            => ComponentBrush.OnInvalidEnd((TComponentCanvas)canvas, (TArgs)args, (TPaint)paint);

        public void Update(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => DataBrush.Update((TDataCanvas)canvas, (TArgs)args, (TPaint)paint);
        public bool CanStartApply(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => DataBrush.CanStartApply((TDataCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void StartApply(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => DataBrush.StartApply((TDataCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void UpdateApply(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => DataBrush.UpdateApply((TDataCanvas)canvas, (TArgs)args, (TPaint)paint);
        public bool CanEndApply(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => DataBrush.CanEndApply((TDataCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void EndApply(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => DataBrush.EndApply((TDataCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void OnInvalidStart(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => DataBrush.OnInvalidStart((TDataCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void OnCancellation(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => DataBrush.OnCancellation((TDataCanvas)canvas, (TArgs)args, (TPaint)paint);
        public void OnInvalidEnd(IGlyphData canvas, ISpaceBrushArgs args, IPaint paint)
            => DataBrush.OnInvalidEnd((TDataCanvas)canvas, (TArgs)args, (TPaint)paint);

        public IGlyphComponent CreateCursor(IDependencyResolver dependencyResolver) => ComponentBrush.CreateCursor(dependencyResolver);
    }
}