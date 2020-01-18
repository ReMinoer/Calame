using System.Collections.ObjectModel;
using System.Linq;
using Glyph.Composition;
using Stave;

namespace Calame
{
    public class ComponentFilter : IComponentFilter
    {
        public ObservableCollection<IGlyphComponent> ExcludedRoots { get; } = new ObservableCollection<IGlyphComponent>();

        public bool Filter(IGlyphComponent component)
        {
            return !ExcludedRoots.Any(x => x == component || x.ChildrenQueue().Contains(component));
        }
    }
}