using System;
using System.Collections;
using System.Collections.Generic;

namespace Steamline.co.Api.V1.Helpers
{
    public class DirectedGraph<Vert> where Vert : IComparable
    {
        private Dictionary<Vert, HashSet<Vert>> _graph = new Dictionary<Vert, HashSet<Vert>>();

        public void AddVertex(Vert vertex) {
            _graph.Add(vertex, new HashSet<Vert>());
        }

        public void AddEdge((Vert From, Vert To) edge) {
            _graph[edge.From].Add(edge.To);
        }

        public bool HasAPath(Vert start, Vert end) {
            if (!_graph.ContainsKey(start) || !_graph.ContainsKey(end)) {
                return false;
            }

            var visited = new HashSet<Vert>();
            var stack = new Stack<Vert>();

            stack.Push(start);

            while (stack.Count > 0)  {
                var vert = stack.Pop();

                if (visited.Contains(vert)) {
                    continue;
                }

                visited.Add(vert);

                // break out early
                if (vert.Equals(end)) {
                    return true;
                }

                foreach (var neighbor in _graph[vert]) {
                    if (visited.Contains(neighbor)) {
                        continue;
                    }

                    stack.Push(neighbor);
                }
            }

            return false;
        }
    }
}