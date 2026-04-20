using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavigatorLogic
{
    internal class MainLogic
    {
        public Graph ReadGraphFromFile(string filePath)
        {
            const string FORMAT_ERROR = "Неверный формат файла!";
            const string TOWN_NAME_ERROR = "Некорректное имя города!";
            const string INCORRECT_TOWN = "Некорректная запись о городе!";
            const string INCORRECT_ROAD = "Некорректная запись о дороге!";

            Graph G = new Graph();

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
                    int id; double x, y; string name;
                    try
                    {
                        if (parts.Length < 5) throw new IndexOutOfRangeException(INCORRECT_TOWN);
                        id = int.Parse(parts[1]);
                        x = double.Parse(parts[2]);
                        y = double.Parse(parts[3]);
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
                }
                else throw new FormatException(FORMAT_ERROR);
            }
            F.Close();
            return G;
        }
    }
}
