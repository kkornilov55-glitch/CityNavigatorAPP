using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavigatorLogic
{
    public class MainLogic
    {
        private Graph G = new Graph();
        private Dictionary<int, int> Parents;
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
                    int from, to; 
                    double length; 
                    double speed = 45.0;
                    try
                    {
                        if (parts.Length < 4) throw new IndexOutOfRangeException(INCORRECT_ROAD);
                        from = int.Parse(parts[1]);
                        to = int.Parse(parts[2]);
                        length = double.Parse(parts[3]);

                        if (parts.Length >= 5) speed = double.Parse(parts[4]);
                    }
                    catch (FormatException)
                    {
                        throw new FormatException(FORMAT_ERROR);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    G.AddEdge(from, to, length, speed);
                    G.AddEdge(to, from, length, speed);
                }
                else throw new FormatException(FORMAT_ERROR);
            }
            F.Close();
            return G;
        }
        /// <summary>
        /// Алгоритм Дейкстры. Принимает id старта и финиша для восстановления пути.
        /// </summary>
        public List<Vertex> Dijkstra(int from, int to, bool useTime, Edge edgeToIgnore = null)
        {
            var visited = new Dictionary<int, bool>();
            var dist = new Dictionary<int, double>();
            Parents = new Dictionary<int, int>();

            // Инициализация всех вершин
            foreach (Vertex v in G.GetVertices())
            {
                visited[v.Id] = false;
                dist[v.Id] = double.MaxValue;
                Parents[v.Id] = -1;
            }

            dist[from] = 0;
            var pq = new PriorityQueue<int, double>();
            pq.Enqueue(from, 0);

            while (pq.Count > 0)
            {
                int u = pq.Dequeue();

                if (visited[u]) continue;
                visited[u] = true;
                if (u == to) break;

                // Перебор всех рёбер в поиске соседей вершины u
                foreach (Edge e in G.GetEdges())
                {
                    //  БЛОКИРОВКА: Если это та дорога, которую нужно объехать — пропускаем её
                    if (edgeToIgnore != null)
                    {
                        bool isBlocked = (e.From == edgeToIgnore.From && e.To == edgeToIgnore.To) ||
                                         (e.From == edgeToIgnore.To && e.To == edgeToIgnore.From);
                        if (isBlocked) continue;
                    }

                    int v = -1;
                    if (e.From == u) v = e.To;
                    else if (e.To == u) v = e.From;

                    if (v != -1 && !visited[v])
                    {
                        double weight = useTime ? e.TimeMins : e.Length;

                        double newDist = dist[u] + weight;
                        if (newDist < dist[v])
                        {
                            dist[v] = newDist;
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
            var path = new List<Vertex>();

            // Если родителей нет или путь не найден
            if (!Parents.ContainsKey(to) || Parents[to] == -1 && from != to)
                return path;

            int current = to;
            while (current != -1 && Parents.ContainsKey(current))
            {
                var vertex = G.GetVertices().FirstOrDefault(v => v.Id == current);
                if (vertex == null) break;
                path.Add(vertex);
                current = Parents[current];
            }

            path.Reverse();
            return path;
        }
        /// <summary>
        /// Находит альтернативный путь, блокируя самый длинный участок основного маршрута.
        /// </summary>
        public List<Vertex> FindAlternativePath(int from, int to, List<Vertex> originalPath, bool useTime)
        {
            if (originalPath == null || originalPath.Count < 2) return new List<Vertex>();

            Edge longestEdge = null;
            double maxLen = -1;

            // 1. Ищем самую длинную дорогу на текущем пути
            for (int i = 0; i < originalPath.Count - 1; i++)
            {
                int u = originalPath[i].Id;
                int v = originalPath[i + 1].Id;

                // Находим объект ребра в графе
                var edge = G.GetEdges().FirstOrDefault(e =>
                    (e.From == u && e.To == v) || (e.From == v && e.To == u));

                double weight = useTime ? edge.TimeMins : edge.Length;

                if (edge != null && weight > maxLen)
                {
                    maxLen = weight;
                    longestEdge = edge;
                }
            }

            // 2. Если нашли что блокировать — строим путь в обход
            if (longestEdge != null)
            {
                return Dijkstra(from, to, useTime, longestEdge);
            }

            return new List<Vertex>();
        }
        /// <summary>
        /// Поиск в ширину (BFS). Находит путь с минимальным количеством перекрёстков.
        /// </summary>
        public List<Vertex> BFS(int from, int to)
        {
            var visited = new Dictionary<int, bool>();
            Parents = new Dictionary<int, int>();
            var queue = new Queue<int>();

            foreach (var v in G.GetVertices())
            {
                visited[v.Id] = false;
                Parents[v.Id] = -1;
            }

            visited[from] = true;
            queue.Enqueue(from);

            while (queue.Count > 0)
            {
                int u = queue.Dequeue();
                if (u == to) break; // Путь найден

                foreach (var e in G.GetEdges())
                {
                    int v = -1;
                    if (e.From == u) v = e.To;
                    else if (e.To == u) v = e.From;

                    if (v != -1 && !visited[v])
                    {
                        visited[v] = true;
                        Parents[v] = u;
                        queue.Enqueue(v);
                    }
                }
            }

            return ReconstructPath(from, to);
        }
    }
}
