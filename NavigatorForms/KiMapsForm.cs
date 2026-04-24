using NavigatorLogic;
using System.Diagnostics;

namespace NavigatorForms
{
    public partial class KiMapsForm : Form
    {
        private MainLogic logic = new MainLogic();
        private Graph G;
        private List<Vertex> lastPath;

        // Для центрирования и масштабирования графа
        private float scale = 1f;
        private float offsetX = 0f;
        private float offsetY = 0f;
        private bool transformReady = false;

        //Для больших графов
        private const int MAX_TOWNS_TO_SHOW_LENGTHS = 15;
        private bool miniGraph = true;
        public KiMapsForm()
        {
            InitializeComponent();
            LoadMap();
            CalculateTransform();
            InitializeComboBoxes();
            DrawMap();
        }
        private void StartButton_Click(object sender, EventArgs e)
        {
            Vertex from = G.GetVertices().FirstOrDefault(v => v.Name == FromComboBox.SelectedItem.ToString());
            Vertex to = G.GetVertices().FirstOrDefault(v => v.Name == ToComboBox.SelectedItem.ToString());

            if (from == null || to == null) return;

            List<Vertex> path;
            string message;
            if (BFS_СheckBox.Checked && FastestWayCheckBox.Checked)
            {
                MessageBox.Show("Пожалуйста, выбирите одну из галочек!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!BFS_СheckBox.Checked)
            {
                //1. Находим основной путь
                var sw1 = Stopwatch.StartNew();
                path = logic.Dijkstra(from.Id, to.Id, FastestWayCheckBox.Checked);
                sw1.Stop();
                lastPath = path;
                long dijkstraTime = sw1.ElapsedMilliseconds;

                //2. Находим альтернативный путь (в обход самой длинной дороги)
                var path2 = logic.FindAlternativePath(from.Id, to.Id, path, FastestWayCheckBox.Checked);

                //3. Считаем статистику для обоих путей
                var stats1 = logic.CalculatePathStats(path);
                var stats2 = logic.CalculatePathStats(path2);

                int hours1 = (int)(stats1.TimeMinutes / 60);
                int minutes1 = (int)(stats1.TimeMinutes % 60);
                int hours2 = (int)(stats2.TimeMinutes / 60);
                int minutes2 = (int)(stats2.TimeMinutes % 60);

                //4. Выводим результат
                message = $"ОСНОВНОЙ МАРШРУТ:\n" +
                          $"Дистанция: {stats1.Distance:F1} км\n" +
                          $"Время в пути: {hours1} ч {minutes1:D2} мин\n" +
                          $"Количество перекрёсков: {path.Count}\n" +
                          $"Время расчёта: {dijkstraTime} мс\n\n" +

                          $"АЛЬТЕРНАТИВНЫЙ:\n" +
                          $"Дистанция: {stats2.Distance:F1} км\n" +
                          $"Время в пути: {hours2} ч {minutes2:D2} мин\n" +
                          $"Количество перекрёсков: {path2.Count}";
            }
            else
            {
                //1. Находим путь
                var sw = Stopwatch.StartNew();
                path = logic.BFS(from.Id, to.Id);
                sw.Stop();
                long bfsTime = sw.ElapsedMilliseconds;

                //2. Считаем статистику пути
                var stats = logic.CalculatePathStats(path);

                int hours = (int)(stats.TimeMinutes / 60);
                int minutes = (int)(stats.TimeMinutes % 60);

                //3. Результат
                message = $"Дистанция: {stats.Distance:F1} км\n" +
                          $"Время в пути: {hours} ч {minutes:D2} мин\n" +
                          $"Количество перекрёсков: {path.Count}" +
                          $"Время работы алгоритма: {bfsTime} мс";
            }

            WayRichTextBox.Text = GetPathAsString(path);
            // Отрисовываем основной путь (пока что)
            DrawMap(path);
            
            MessageBox.Show(message, "Результаты навигации", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private string GetPathAsString(List<Vertex> path)
        {
            if (path == null || path.Count == 0) return "Путь не найден";
            return string.Join(" -> ", path.Select(v => v.Name));
        }
        private void AltButton_Click(object sender, EventArgs e)
        {
            if (lastPath == null || lastPath.Count < 2)
            {
                MessageBox.Show("Сначала постройте основной маршрут!");
                return;
            }

            Vertex from = G.GetVertices().FirstOrDefault(v => v.Name == FromComboBox.SelectedItem.ToString());
            Vertex to = G.GetVertices().FirstOrDefault(v => v.Name == ToComboBox.SelectedItem.ToString());

            // Ищем альтернативу
            var altPath = logic.FindAlternativePath(from.Id, to.Id, lastPath, FastestWayCheckBox.Checked);
            WayRichTextBox.Text = string.Join(" -> ", altPath.Select(v => v.Name));

            if (altPath.Count < 2)
            {
                MessageBox.Show("Альтернативный маршрут не найден (нет объезда).");
            }
            else
            {
                DrawMap(altPath);
                MessageBox.Show("Найден альтернативный маршрут (в обход главной дороги).");
            }
        }
        private void InitializeComboBoxes()
        {
            var towns = new List<string>();
            foreach (Vertex town in G.GetVertices()) towns.Add(town.Name);

            FromComboBox.Items.Clear();
            FromComboBox.Items.AddRange(towns.ToArray());
            FromComboBox.SelectedIndex = 0;

            ToComboBox.Items.Clear();
            ToComboBox.Items.AddRange(towns.ToArray());
            ToComboBox.SelectedIndex = 1;
        }
        //МЕТОДЫ ОТРИСОВКИ КАРТЫ
        private void DrawMap(List<Vertex> path = null)
        {
            // Пересчитываем масштаб если нужно
            if (!transformReady) CalculateTransform();

            Bitmap bmp = new Bitmap(GraphPictureBox.Width, GraphPictureBox.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            List<Vertex> vertices = G.GetVertices();

            // === 1. Серые дороги (кроме тех, что в пути) ===
            Pen roadPen = new Pen(Color.LightGray, 3);
            Pen pathPen = new Pen(Color.LightBlue, 5);

            foreach (Edge edge in G.GetEdges())
            {
                var v1 = vertices.FirstOrDefault(v => v.Id == edge.From);
                var v2 = vertices.FirstOrDefault(v => v.Id == edge.To);
                if (v1 == null || v2 == null) continue;

                // Пропускаем рёбра пути — нарисуем их позже поверх
                if (path != null && EdgeInPath(path, v1, v2)) continue;

                PointF p1 = Transform(v1.X, v1.Y);
                PointF p2 = Transform(v2.X, v2.Y);
                g.DrawLine(roadPen, p1, p2);
            }

            // === 2. Синий путь поверх серых дорог ===
            if (path != null && path.Count > 1)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    var v1 = vertices.FirstOrDefault(v => v.Id == path[i].Id);
                    var v2 = vertices.FirstOrDefault(v => v.Id == path[i + 1].Id);
                    if (v1 != null && v2 != null)
                    {
                        PointF p1 = Transform(v1.X, v1.Y);
                        PointF p2 = Transform(v2.X, v2.Y);
                        g.DrawLine(pathPen, p1, p2);
                    }
                }
            }

            // === 3. Подписи длин: все если нет пути, только путь если есть ===
            Font labelFont = new Font("Arial", 8);
            foreach (Edge edge in G.GetEdges())
            {
                var v1 = vertices.FirstOrDefault(v => v.Id == edge.From);
                var v2 = vertices.FirstOrDefault(v => v.Id == edge.To);
                if (v1 == null || v2 == null) continue;

                bool showLabel = (path == null & miniGraph) || (path != null && EdgeInPath(path, v1, v2));
                if (!showLabel) continue;

                PointF mid = Transform((v1.X + v2.X) / 2f, (v1.Y + v2.Y) / 2f);
                g.DrawString($"{edge.Length:F1} км", labelFont, Brushes.Black, mid.X, mid.Y);
            }

            // === 4. Города поверх всего ===
            int radius = 5;
            foreach (Vertex v in vertices)
            {
                PointF pos = Transform(v.X, v.Y);
                Brush brush = Brushes.Black;

                if (path != null && path.Count > 0)
                {
                    if (v.Id == path[0].Id) brush = Brushes.DarkRed; // Старт
                    else if (v.Id == path[path.Count - 1].Id) brush = Brushes.DarkGreen; // Финиш
                    else if (path.Any(p => p.Id == v.Id)) brush = Brushes.LightBlue; // Путь
                }
                g.FillEllipse(brush, pos.X - radius, pos.Y - radius, radius * 2, radius * 2);
            }

            // Подписи названий городов
            Font cityFont = new Font("Arial", 9, FontStyle.Bold);
            foreach (Vertex v in vertices)
            {
                PointF pos = Transform(v.X, v.Y);
                // Текст чуть справа и выше кружка, чтобы не перекрывать
                g.DrawString(v.Name, cityFont, Brushes.Black, pos.X + 20, pos.Y - 5);
            }

            // Присваиваем изображение
            if (GraphPictureBox.Image != null) GraphPictureBox.Image.Dispose();
            GraphPictureBox.Image = bmp;
        }
        private bool EdgeInPath(List<Vertex> path, Vertex v1, Vertex v2)
        {
            for (int i = 1; i < path.Count; i++)
            {
                // Проверяем ОБА направления для неориентированного графа
                if ((path[i - 1].Id == v1.Id && path[i].Id == v2.Id) || (path[i - 1].Id == v2.Id && path[i].Id == v1.Id))
                {
                    return true;
                }
            }
            return false;
        }

        //МАCШТАБИЗАЦИЯ ГРАФА
        private void CalculateTransform()
        {
            var vertices = G.GetVertices();
            if (vertices.Count == 0) return;

            // Находим границы графа
            float minX = vertices.Min(v => v.X);
            float maxX = vertices.Max(v => v.X);
            float minY = vertices.Min(v => v.Y);
            float maxY = vertices.Max(v => v.Y);

            float graphWidth = maxX - minX;
            float graphHeight = maxY - minY;
            if (graphWidth == 0) graphWidth = 1;
            if (graphHeight == 0) graphHeight = 1;

            // Вычисляем масштаб с отступами 23%
            float padding = 0.23f;
            float scaleX = (GraphPictureBox.Width * (1 - padding)) / graphWidth;
            float scaleY = (GraphPictureBox.Height * (1 - padding)) / graphHeight;
            scale = Math.Min(scaleX, scaleY); // Единый масштаб по обоим осям

            // Центрируем
            offsetX = (GraphPictureBox.Width - graphWidth * scale) / 2 - minX * scale;
            offsetY = (GraphPictureBox.Height - graphHeight * scale) / 2 - minY * scale;

            transformReady = true;
        }
        private PointF Transform(float x, float y)
        {
            if (!transformReady) return new PointF(x, y);
            return new PointF(x * scale + offsetX, y * scale + offsetY);
        }

        private void KiMapsForm_Resize(object sender, EventArgs e)
        {
            transformReady = false;
            if (G != null) DrawMap();
        }

        private void LoadMap()
        {
            try
            {
                var OFD = new OpenFileDialog
                {
                    Title = "Выберите текстовый файл",
                    Filter = "Текстовые файлы (*.txt)|*.txt",
                    FilterIndex = 1,
                    RestoreDirectory = true,
                    CheckFileExists = true
                };
                if (OFD.ShowDialog() == DialogResult.OK)
                {
                    string filePath = OFD.FileName;
                    string ext = Path.GetExtension(filePath);
                    if (ext != ".txt")
                    {
                        MessageBox.Show("Чтение расширения выбранного файла не поддерживается!", "Неудача!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    G = logic.ReadGraphFromFile(filePath);
                    if (G.GetVertices().Count > MAX_TOWNS_TO_SHOW_LENGTHS) miniGraph = false;
                }
            }
            catch
            {
                MessageBox.Show("Ошибка, файл с графом не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
