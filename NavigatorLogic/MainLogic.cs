using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavigatorLogic
{
    public class MainLogic
    {
        private const int INF = int.MaxValue;
        private Graph G = new Graph();
        private int[] Parents;
        public Graph ReadGraphFromFile(string filePath)
        {
            const string FORMAT_ERROR = "Неверный формат файла!";
            const string TOWN_NAME_ERROR = "Некорректное имя города!";
            const string INCORRECT_TOWN = "Некорректная запись о городе!";
            const string INCORRECT_ROAD = "Некорректная запись о дороге!";

            string textLine;
            StreamReader F = new StreamReader(filePath);

            while ((textLine = F.ReadLine()) != null)
            {
                //textLine = F.ReadLine();
                //if (string.IsNullOrEmpty(textLine)) break; //Файл прочитан до конца
                textLine = textLine.Trim();
                //Скипаем комментарии
                if (textLine.StartsWith("#"))
                    continue;

                string[] parts = textLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue; //Пустая строка
                //Обрабатываем вершины(Города)
                if (parts[0] == "V")
                {
                    int id; float x, y; string name;
                    try
                    {
                        if (parts.Length < 5) throw new IndexOutOfRangeException(INCORRECT_TOWN);
                        id = int.Parse(parts[1]);
                        x = float.Parse(parts[2]);
                        y = float.Parse(parts[3]);
                        //Получаем имя города и сразу проводим валидацию
                        name = parts[4];
                        if (string.IsNullOrEmpty(name))
                            throw new ArgumentNullException(TOWN_NAME_ERROR);
                    }
                    catch (FormatException)
                    {
                        throw new FormatException(FORMAT_ERROR);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    G.AddVertex(id, x, y, name);
                }
                //Обрабатываем ребра(Дороги между городами)
                else if (parts[0] == "E")
                {
                    int from, to; double length;
                    try
                    {
                        if (parts.Length < 4) throw new IndexOutOfRangeException(INCORRECT_ROAD);
                        from = int.Parse(parts[1]);
                        to = int.Parse(parts[2]);
                        length = double.Parse(parts[3]);
                    }
                    catch (FormatException)
                    {
                        throw new FormatException(FORMAT_ERROR);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    G.AddEdge(from, to, length);
                    G.AddEdge(to, from, length);
                }
                else throw new FormatException(FORMAT_ERROR);
            }
            F.Close();
            return G;
        }
        /// <summary>
        /// Алгоритм Дейкстры. Принимает id старта и финиша для восстановления пути.
        /// </summary>
        public List<Vertex> Dijkstra(int from, int to)
        {
            int N = G.GetVertices().Count;
            bool[] Visited = new bool[N];
            double[] Dist = new double[N];
            Parents = new int[N];

            foreach (Vertex V in G.GetVertices())
            {
                Visited[V.Id] = false;
                Dist[V.Id] = double.MaxValue;
                Parents[V.Id] = -1;
            }

            Dist[from] = 0;
            var pq = new PriorityQueue<int, double>();
            pq.Enqueue(from, 0);

            while (pq.Count > 0)
            {
                int u = pq.Dequeue();

                if (Visited[u]) continue;
                Visited[u] = true;
                if (u == to) break; // Кратчайший путь до цели найден

                // Перебор всех рёбер в поиске соседей вершины u
                foreach (Edge E in G.GetEdges())
                {
                    int v = -1;
                    if (E.From == u) v = E.To;
                    else if (E.To == u) v = E.From;

                    if (v != -1 && !Visited[v])
                    {
                        double newDist = Dist[u] + E.Length;
                        if (newDist < Dist[v])
                        {
                            Dist[v] = newDist;
                            Parents[v] = u;
                            pq.Enqueue(v, newDist);
                        }
                    }
                }
            }

            return ReconstructPath(from, to);
        }
        private List<Vertex> ReconstructPath(int from, int to)
        {
            List<Vertex> path = new List<Vertex>();
            if (Parents[to] == -1 && from != to) return path; // Путь не найден

            int current = to;
            while (current != -1)
            {
                path.Add(G.GetVertices().FirstOrDefault(v => v.Id == current));
                current = Parents[current];
            }
            path.Reverse();
            return path;
        }
    }
}
