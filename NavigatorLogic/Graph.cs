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

        public void AddVertex(int id, float x, float y, string name) => Vertices.Add(new Vertex(id, x, y, name));
        public void AddEdge(int from, int to, double length) => Edges.Add(new Edge(from, to, length));
        public List<Vertex> GetVertices() => Vertices;
        public List<Edge> GetEdges() => Edges;
    }
}
