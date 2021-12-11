using System.Collections.Generic;
using System.Linq;
using GraphShape;
using GraphShape.Algorithms.Layout;
using QuikGraph;
using QuikGraph.Algorithms;

namespace Calame.ViewGraph.Layout
{
    public class MultiTreeLayoutAlgorithm<TVertex, TEdge, TGraph> : DefaultParameterizedLayoutAlgorithmBase<TVertex, TEdge, TGraph, MultiTreeLayoutParameters>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        private readonly IDictionary<TVertex, Size> _verticesSizes;

        public MultiTreeLayoutAlgorithm(
            TGraph visitedGraph,
            IDictionary<TVertex, Size> verticesSizes,
            MultiTreeLayoutParameters parameters = null)
            : this(visitedGraph, null, verticesSizes, parameters)
        {
        }

        public MultiTreeLayoutAlgorithm(
            TGraph visitedGraph,
            IDictionary<TVertex, Point> verticesPositions,
            IDictionary<TVertex, Size> verticesSizes,
            MultiTreeLayoutParameters parameters = null)
            : base(visitedGraph, verticesPositions, parameters)
        {
            _verticesSizes = verticesSizes;
        }
        
        protected override void InternalCompute()
        {
            if (VisitedGraph.VertexCount == 0)
                return;

            double position = 0;
            foreach (TVertex root in VisitedGraph.Roots())
            {
                var tree = new BidirectionalGraph<TVertex, TEdge>();
                tree.AddVertex(root);
                AddChildren(VisitedGraph, tree, root);

                Dictionary<TVertex, Point> verticesPositions = VerticesPositions.Where(x => tree.ContainsVertex(x.Key)).ToDictionary(x => x.Key, x => x.Value);
                Dictionary<TVertex, Size> verticesSizes = _verticesSizes.Where(x => tree.ContainsVertex(x.Key)).ToDictionary(x => x.Key, x => x.Value);

                var simpleTreeLayoutAlgorithm = new SimpleTreeLayoutAlgorithm<TVertex, TEdge, BidirectionalGraph<TVertex, TEdge>>(tree, verticesPositions, verticesSizes, Parameters);
                simpleTreeLayoutAlgorithm.Compute();

                foreach (TVertex vertex in tree.Vertices)
                    VerticesPositions[vertex] = new Point(simpleTreeLayoutAlgorithm.VerticesPositions[vertex].X + position, simpleTreeLayoutAlgorithm.VerticesPositions[vertex].Y);

                double left = double.MaxValue;
                double top = double.MaxValue;
                double right = double.MinValue;
                double bottom = double.MinValue;

                foreach (TVertex vertex in tree.Vertices)
                {
                    Point nodePosition = simpleTreeLayoutAlgorithm.VerticesPositions[vertex];
                    Size nodeSize = verticesSizes[vertex];

                    Rect nodeRect = new Rect(nodePosition, nodeSize);

                    if (nodeRect.Left < left)
                        left = nodeRect.Left;
                    if (nodeRect.Top < top)
                        top = nodeRect.Top;
                    if (nodeRect.Right > right)
                        right = nodeRect.Right;
                    if (nodeRect.Bottom > bottom)
                        bottom = nodeRect.Bottom;
                }

                var treeSize = new Size(right - left, bottom - top);
                position += treeSize.Width;
            }
        }

        private void AddChildren(TGraph graph, BidirectionalGraph<TVertex, TEdge> tree, TVertex vertex)
        {
            foreach (TEdge outEdge in graph.OutEdges(vertex))
            {
                tree.AddVertex(outEdge.Target);
                tree.AddEdge(outEdge);

                AddChildren(graph, tree, outEdge.Target);
            }
        }
    }
}