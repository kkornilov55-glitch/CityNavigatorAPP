using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavigatorLogic
{
    public class Graph
    {
        private List<Vertex> Vertices = new List<Vertex>();
        private List<Edge> Edges = new List<Edge>();
        private Dictionary<int, List<Edge>> AdjacencyList = new Dictionary<int, List<Edge>>();

        public void AddVertex(int id, float x, float y, string name)
        {
            Vertices.Add(new Vertex(id, x, y, name));
            if (!AdjacencyList.ContainsKey(id))
                AdjacencyList[id] = new List<Edge>();
        }
        public void AddEdge(int from, int to, double length, double speed = 40.0)
        {
            var edge = new Edge(from, to, length, speed);
            Edges.Add(edge);

            if (!AdjacencyList.ContainsKey(from)) AdjacencyList[from] = new List<Edge>();
            if (!AdjacencyList.ContainsKey(to)) AdjacencyList[to] = new List<Edge>();

            AdjacencyList[from].Add(edge);
            AdjacencyList[to].Add(edge);
        }
        public List<Edge> GetNeighbors(int vertexId)
        {
            if (AdjacencyList.ContainsKey(vertexId))
                return AdjacencyList[vertexId];
            return new List<Edge>(); // пустой список, если вершины нет
        }
        public List<Vertex> GetVertices() => Vertices;
        public List<Edge> GetEdges() => Edges;
    }
}
