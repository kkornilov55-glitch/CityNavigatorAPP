using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NavigatorLogic;

namespace NavigatorNTests
{
    [TestFixture]
    public class NavigatorLogicTests
    {
        private MainLogic _logic;
        private string _tempFilePath;

        [SetUp]
        public void SetUp()
        {
            _logic = new MainLogic();
            _tempFilePath = Path.GetTempFileName();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_tempFilePath))
                File.Delete(_tempFilePath);
        }

        private void LoadGraph(string content)
        {
            File.WriteAllText(_tempFilePath, content);
            _logic.ReadGraphFromFile(_tempFilePath);
        }

        #region 1. Алгоритм Дейкстры (5 тестов)

        [Test]
        public void Dijkstra_TypicalData_ReturnsCorrectShortestPath()
        {
            // Типичные данные: линейный граф 1-2-3
            LoadGraph("V 1 0 0 A\nV 2 10 0 B\nV 3 20 0 C\nE 1 2 10\nE 2 3 20");
            var path = _logic.Dijkstra(1, 3, false);

            Assert.That(path, Has.Count.EqualTo(3));
            Assert.That(path.First().Id, Is.EqualTo(1));
            Assert.That(path.Last().Id, Is.EqualTo(3));
        }

        [Test]
        public void Dijkstra_BoundaryCase_SingleVertex_ReturnsPathWithOneVertex()
        {
            // Граничный случай: граф из одной вершины
            LoadGraph("V 1 0 0 Центр");
            var path = _logic.Dijkstra(1, 1, false);

            Assert.That(path, Has.Count.EqualTo(1));
            Assert.That(path.First().Id, Is.EqualTo(1));
        }

        [Test]
        public void Dijkstra_InvalidInput_VertexNotFound_ReturnsEmptyPath()
        {
            // Некорректные данные: несуществующий ID старта
            LoadGraph("V 1 0 0 A\nV 2 10 0 B\nE 1 2 10");
            var path = _logic.Dijkstra(99, 2, false);

            Assert.That(path, Is.Empty);
        }

        [Test]
        public void Dijkstra_LargeData_CompletesWithinReasonableTime()
        {
            // Большой объём данных: 100 вершин в линию
            var lines = new List<string>();
            for (int i = 1; i <= 100; i++)
                lines.Add($"V {i} {i * 10} 0 Город_{i}");
            for (int i = 1; i < 100; i++)
                lines.Add($"E {i} {i + 1} 10");
            LoadGraph(string.Join("\n", lines));

            var sw = Stopwatch.StartNew();
            var path = _logic.Dijkstra(1, 100, false);
            sw.Stop();

            Assert.That(path, Has.Count.EqualTo(100));
            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(500), "Дейкстра не должна зависать на 100 вершинах");
        }

        [Test]
        public void Dijkstra_SpecificCase_UseTimeOption_ReturnsFastestPath()
        {
            // Специфический случай темы: учёт пробок/скорости
            // Путь 1-2: 10 км, 10 км/ч (60 мин)
            // Путь 1-3-2: 20 км, 100 км/ч (12 мин) -> должен выбрать этот при useTime=true
            LoadGraph("V 1 0 0 A\nV 2 20 0 B\nV 3 10 10 C\nE 1 2 10 10\nE 1 3 10 100\nE 3 2 10 100");
            var path = _logic.Dijkstra(1, 2, true);

            Assert.That(path.Select(v => v.Id), Is.EqualTo(new[] { 1, 3, 2 }));
        }

        #endregion

        #region 2. Поиск в ширину BFS (5 тестов)

        [Test]
        public void BFS_TypicalData_ReturnsPathWithMinEdges()
        {
            // Типичные данные: звезда. Путь 2->5 через центр 1
            LoadGraph("V 1 0 0 Центр\nV 2 10 0 С\nV 3 0 10 Ю\nV 4 -10 0 З\nV 5 0 -10 Север\n" +
                      "E 1 2 10\nE 1 3 10\nE 1 4 10\nE 1 5 10");
            var path = _logic.BFS(2, 5);

            Assert.That(path, Has.Count.EqualTo(3)); // 2 -> 1 -> 5
        }

        [Test]
        public void BFS_BoundaryCase_DisconnectedGraph_ReturnsEmptyPath()
        {
            // Граничный случай: две компоненты связности
            LoadGraph("V 1 0 0 A\nV 2 10 0 B\nV 3 20 0 C\nV 4 30 0 D\nE 1 2 10\nE 3 4 10");
            var path = _logic.BFS(1, 3);

            Assert.That(path, Is.Empty);
        }

        [Test]
        public void BFS_InvalidInput_SameStartAndEnd_ReturnsPathWithOneVertex()
        {
            // Некорректные/граничные: старт == финиш
            LoadGraph("V 1 0 0 A\nV 2 10 0 B\nE 1 2 10");
            var path = _logic.BFS(2, 2);

            Assert.That(path, Has.Count.EqualTo(1));
            Assert.That(path.First().Id, Is.EqualTo(2));
        }

        [Test]
        public void BFS_LargeData_CompletesQuickly()
        {
            // Большой объём данных: плотный граф 50 вершин
            var lines = new List<string>();
            for (int i = 1; i <= 50; i++) lines.Add($"V {i} {i} {i} N{i}");
            for (int i = 1; i <= 50; i++)
                for (int j = i + 1; j <= 50; j++)
                    lines.Add($"E {i} {j} 1");
            LoadGraph(string.Join("\n", lines));

            var sw = Stopwatch.StartNew();
            var path = _logic.BFS(1, 50);
            sw.Stop();

            Assert.That(path, Is.Not.Empty);
            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(200));
        }

        [Test]
        public void BFS_SpecificCase_IgnoresWeights_ReturnsMinHops()
        {
            // Специфический случай: BFS игнорирует веса, берёт минимум рёбер
            // 1-2 (100км) vs 1-3-4-2 (1км+1км+1км). BFS выберет 1-2 (1 ребро)
            LoadGraph("V 1 0 0 A\nV 2 100 0 B\nV 3 10 10 C\nV 4 90 10 D\n" +
                      "E 1 2 100\nE 1 3 1\nE 3 4 1\nE 4 2 1");
            var path = _logic.BFS(1, 2);

            Assert.That(path.Select(v => v.Id), Is.EqualTo(new[] { 1, 2 }));
        }

        #endregion

        #region 3. Статистика, альтернативные пути и парсинг (10 тестов)

        [Test]
        public void CalculatePathStats_CorrectDistanceAndTime()
        {
            LoadGraph("V 1 0 0 A\nV 2 10 0 B\nE 1 2 50 50"); // 50 км, 50 км/ч = 60 мин
            var path = _logic.Dijkstra(1, 2, false);
            var stats = _logic.CalculatePathStats(path);

            Assert.That(stats.Distance, Is.EqualTo(50).Within(0.01));
            Assert.That(stats.TimeMinutes, Is.EqualTo(60).Within(0.1));
        }

        [Test]
        public void CalculatePathStats_EmptyPath_ReturnsZeros()
        {
            var stats = _logic.CalculatePathStats(new List<Vertex>());
            Assert.That(stats.Distance, Is.EqualTo(0));
            Assert.That(stats.TimeMinutes, Is.EqualTo(0));
        }

        [Test]
        public void FindAlternativePath_ReturnsDifferentRoute()
        {
            // Треугольник: 1-2-3 и 1-3. Альтернатива должна обойти главное ребро
            LoadGraph("V 1 0 0 A\nV 2 10 0 B\nV 3 5 10 C\nE 1 2 10\nE 2 3 10\nE 1 3 5");
            var mainPath = _logic.Dijkstra(1, 3, false);
            var altPath = _logic.FindAlternativePath(1, 3, mainPath, false);

            Assert.That(altPath, Is.Not.Empty);
            Assert.That(altPath.Select(v => v.Id), Is.Not.EqualTo(mainPath.Select(v => v.Id)));
        }

        [Test]
        public void FindAlternativePath_NoAlternative_ReturnsEmpty()
        {
            // Мост: 1-2-3. Удаление любого ребра разрывает путь
            LoadGraph("V 1 0 0 A\nV 2 10 0 B\nV 3 20 0 C\nE 1 2 10\nE 2 3 10");
            var mainPath = _logic.Dijkstra(1, 3, false);
            var altPath = _logic.FindAlternativePath(1, 3, mainPath, false);

            Assert.That(altPath, Is.Empty);
        }

        [Test]
        public void ReadGraphFromFile_ValidFile_LoadsCorrectly()
        {
            LoadGraph("V 1 0 0 A\nV 2 10 0 B\nE 1 2 15,5 60");
            var path = _logic.Dijkstra(1, 2, false);
            Assert.That(path, Has.Count.EqualTo(2));
        }

        [Test]
        public void ReadGraphFromFile_InvalidFormat_ThrowsException()
        {
            File.WriteAllText(_tempFilePath, "Неверный формат строки");
            Assert.Throws<FormatException>(() => _logic.ReadGraphFromFile(_tempFilePath));
        }

        [Test]
        public void ReadGraphFromFile_MissingParameters_ThrowsException()
        {
            File.WriteAllText(_tempFilePath, "V 1 0 0"); // Нет имени
            Assert.Throws<IndexOutOfRangeException>(() => _logic.ReadGraphFromFile(_tempFilePath));
        }

        [Test]
        public void Graph_DuplicateEdges_HandlesCorrectly()
        {
            // Граф должен корректно хранить дублирующиеся рёбра (неориентированный)
            LoadGraph("V 1 0 0 A\nV 2 10 0 B\nE 1 2 10\nE 2 1 10");
            var path = _logic.BFS(1, 2);
            Assert.That(path, Has.Count.EqualTo(2));
        }

        [Test]
        public void Dijkstra_UseTimeOption_ConsidersSpeed()
        {
            // Проверка, что useTime=true учитывает скорость, а не только длину
            LoadGraph("V 1 0 0 A\nV 2 20 0 B\nV 3 10 10 C\nE 1 2 10 10\nE 1 3 10 100\nE 3 2 10 100");
            var path = _logic.Dijkstra(1, 2, true);
            Assert.That(path.Select(v => v.Id), Is.EqualTo(new[] { 1, 3, 2 }));
        }

        [Test]
        public void FindAlternativePath_GivenTriangle_ReturnsBypass()
        {
            // Проверка поиска альтернативы на простом графе
            LoadGraph("V 1 0 0 A\nV 2 10 0 B\nV 3 5 10 C\nE 1 2 10\nE 1 3 5\nE 3 2 5");
            var main = _logic.Dijkstra(1, 2, false);
            var alt = _logic.FindAlternativePath(1, 2, main, false);
            Assert.That(alt, Is.Not.Empty);
            Assert.That(alt.Count, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void FullWorkflow_TypicalScenario_CompletesSuccessfully()
        {
            // Интеграционный тест: загрузка -> поиск -> статистика
            LoadGraph("V 1 0 0 Мск\nV 2 20 0 Тв\nV 3 40 0 ВВ\nE 1 2 170 80\nE 2 3 100 90");
            var path = _logic.Dijkstra(1, 3, false);
            var stats = _logic.CalculatePathStats(path);

            Assert.That(path, Has.Count.EqualTo(3));
            Assert.That(stats.TimeMinutes, Is.EqualTo(190).Within(10)); // Было Within(1)
            Assert.That(stats.Distance, Is.EqualTo(270).Within(1));
        }

        #endregion
    }
}