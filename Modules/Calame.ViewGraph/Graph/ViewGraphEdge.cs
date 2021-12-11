using QuikGraph;

namespace Calame.ViewGraph.Graph
{
    public class ViewGraphEdge : Edge<ViewGraphVertex>
    {
        public ViewGraphEdge(ViewGraphVertex source, ViewGraphVertex target)
            : base(source, target) {}
    }
}