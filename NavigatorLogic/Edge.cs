namespace NavigatorLogic
{
    public class Edge
    {
        public int From { get; private set; }
        public int To { get; private set; }
        public double Length { get; private set; }

        public Edge(int from, int to, double length)
        {
            From = from;
            To = to;
            Length = length;
        }
    }
}
