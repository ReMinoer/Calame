using System.Collections.Generic;
using GraphShape.Algorithms;
using GraphShape.Algorithms.Layout;
using QuikGraph;

namespace Calame.ViewGraph.Layout
{
    public class MultiTreeLayoutAlgorithmFactory<TVertex, TEdge, TGraph> : ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        public const string AlgorithmType = "MultiTree";
        public IEnumerable<string> AlgorithmTypes { get; } = new[] { AlgorithmType };

        public ILayoutAlgorithm<TVertex, TEdge, TGraph> CreateAlgorithm(string algorithmType, ILayoutContext<TVertex, TEdge, TGraph> context, ILayoutParameters parameters)
        {
            if (!IsValidAlgorithm(algorithmType))
                return null;

            return new MultiTreeLayoutAlgorithm<TVertex, TEdge, TGraph>(context.Graph, context.Positions, context.Sizes, parameters as MultiTreeLayoutParameters);
        }

        public ILayoutParameters CreateParameters(string algorithmType, ILayoutParameters parameters)
        {
            if (!IsValidAlgorithm(algorithmType))
                return null;

            return parameters.CreateNewParameters<MultiTreeLayoutParameters>();
        }

        public string GetAlgorithmType(ILayoutAlgorithm<TVertex, TEdge, TGraph> algorithm)
        {
            return algorithm is MultiTreeLayoutAlgorithm<TVertex, TEdge, TGraph> ? AlgorithmType : string.Empty;
        }

        public bool IsValidAlgorithm(string algorithmType) => algorithmType == AlgorithmType;
        public bool NeedEdgeRouting(string algorithmType) => IsValidAlgorithm(algorithmType);
        public bool NeedOverlapRemoval(string algorithmType) => IsValidAlgorithm(algorithmType);
    }
}