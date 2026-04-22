namespace NavigatorLogic
{
    public class Edge
    {
        public int From { get; private set; }
        public int To { get; private set; }
        public double Length { get; private set; }
        public double Speed { get; set; }
        public Edge(int from, int to, double length, double speed = 40)
        {
            From = from;
            To = to;
            Length = length;
            Speed = speed;
        }
        public double TimeMins => (Length / Speed) * 60; //Время в пути, в минутах
    }
}
