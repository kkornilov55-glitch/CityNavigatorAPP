namespace NavigatorLogic
{
    public class Edge
    {
        public int From { get; private set; }
        public int To { get; private set; }
        public double Length { get; private set; }
        public double Speed { get; set; } = 45.0;  // По умолчанию 45 км/ч

        public Edge(int from, int to, double length, double speed = 45.0)
        {
            From = from;
            To = to;
            Length = length;
            Speed = speed;
        }

        public double TimeMins => (Length / Speed) * 60;
    }
}
