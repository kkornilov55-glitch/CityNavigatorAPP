using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavigatorLogic
{
    public class Vertex
    {
        public int Id {  get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public string Name { get; private set; }

        public Vertex(int id, float x, float y, string name)
        {
            Id = id;
            X = x;
            Y = y; 
            Name = name;
        }
    }
}
