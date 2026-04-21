using NavigatorLogic;

namespace NavigatorForms
{
    public partial class KiMapsForm : Form
    {
        private Graph G;
        public KiMapsForm()
        {
            InitializeComponent();
            MainLogic logic = new MainLogic();
            G = logic.ReadGraphFromFile("C:\\Main\\Works\\Labs\\2_term\\AL_AND_PROG\\Курсач 17 тема\\CityNavigator\\Graphs\\testGraph.txt");
            InitializeComboBoxes();
            DrawMap(G);
        }

        private void InitializeComboBoxes()
        {
            var towns = new List<string>();
            foreach(Vertex town in G.GetVertices()) towns.Add(town.Name);

            FromComboBox.Items.Clear();
            FromComboBox.Items.AddRange(towns.ToArray());
            FromComboBox.SelectedIndex = 0;

            ToComboBox.Items.Clear();
            ToComboBox.Items.AddRange(towns.ToArray());
            ToComboBox.SelectedIndex = 1;
        }
        //МЕТОДЫ ОТРИСОВКИ КАРТЫ
        private void DrawMap(Graph graph, List<int> path = null)
        {
            Bitmap bmp = new Bitmap(GraphPictureBox.Width, GraphPictureBox.Height);
            Graphics g = Graphics.FromImage(bmp);
            //Отчищаем после прошлой отрисовки
            g.Clear(Color.White);


            List<Vertex> Vertices = graph.GetVertices();
            //Рисуем дороги
            Pen roadPen = new Pen(Color.LightGray, 5);
            foreach (Edge E in graph.GetEdges())
            {
                var v1 = Vertices.FirstOrDefault(v => v.Id == E.From);
                var v2 = Vertices.FirstOrDefault(v => v.Id == E.To);

                if (v1 != null && v2 != null)
                {
                    g.DrawLine(roadPen, v1.X, v1.Y, v2.X, v2.Y);
                }
            }
            //Рисуем города
            foreach (Vertex V in Vertices)
            {
                int D = 15;
                g.FillEllipse(Brushes.Black, V.X - D/2, V.Y - D/2, D, D);
            }
            GraphPictureBox.Image = bmp;
        }
    }
}
