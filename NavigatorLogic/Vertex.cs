using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavigatorLogic
{
    public class Vertex
    {
        private int Id;
        private double X;
        private double Y;
        private string Name;

        public Vertex(int id, double x, double y, string name)
        {
            Id = id;
            X = x;
            Y = y; 
            Name = name;
        }
    }
}
