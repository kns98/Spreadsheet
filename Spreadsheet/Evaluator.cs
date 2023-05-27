using MicroSpread;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Evaluator;

public partial class Evaluator
{

    // Function to parse coordinate string and return as a tuple
    static (int, int) ParseCoordinates(string coordinate)
    {
        if (coordinate == "") return (-1, -1);
        string[] parts = coordinate.Split('R', 'C');

        int row = int.Parse(parts[1]);
        int col = int.Parse(parts[2]);

        return (row, col);
    }

    public static void Test()
    {
        // Sample data
        Dictionary<int, Dictionary<int, object>> data = new Dictionary<int, Dictionary<int, object>>
        {
            { 1, new Dictionary<int, object> { { 1, 10 }, { 2, 20 }, { 3, "=A1+B1" } } },
            { 2, new Dictionary<int, object> { { 1, "=A1+5" }, { 2, "=A1+B1" }, { 3, 30 } } },
            { 3, new Dictionary<int, object> { { 1, "=A2+A2" }, { 2, "=B1+B2" }, { 3, "=C1+C2" } } }
        };

        string[][] dependencyTexts = new string[][]
        {
            new string[] { "", "", "R1C1 R1C2" },
            new string[] { "R1C1", "R1C1 R1C2", "" },
            new string[] { "R2C1", "R1C2 C2R2", "R1C3 R2C3" }
        };


        var deps = BuildDependencies(dependencyTexts);
        DependencyGraph.PrintGraph(deps);
        Console.WriteLine("Updating targets ...");
        DependencyGraph.UpdateGraph(deps,data);
        DependencyGraph.PrintGraph(data,deps);

    }

    public static Dictionary<CellCoordinates, List<CellCoordinates>> BuildDependencies(string[][] dependencyTexts)
    {
        Dictionary<CellCoordinates, List<CellCoordinates>> dependencies = new Dictionary<CellCoordinates, List<CellCoordinates>>();

        for (int row = 0; row < dependencyTexts.Length; row++)
        {
            string[] dependencyRow = dependencyTexts[row];

            for (int col = 0; col < dependencyRow.Length; col++)
            {
                string dependencyText = dependencyRow[col];

                if (!string.IsNullOrEmpty(dependencyText))
                {
                    string[] dependencyCells = dependencyText.Split(' ');

                    CellCoordinates cellCoordinates = new CellCoordinates(row + 1, col + 1);
                    List<CellCoordinates> cellDependencies = new List<CellCoordinates>();

                    foreach (string dependencyCell in dependencyCells)
                    {
                        string[] parts = dependencyCell.Split('R', 'C');

                        if (parts.Length == 3 && int.TryParse(parts[1], out int depRow) && int.TryParse(parts[2], out int depCol))
                        {
                            CellCoordinates depCoordinates = new CellCoordinates(depRow, depCol);
                            cellDependencies.Add(depCoordinates);
                        }
                    }

                    dependencies[cellCoordinates] = cellDependencies;
                }
            }
        }

        return dependencies;
    }

    public class DependencyGraph
    {
        static public void UpdateGraph(Dictionary<CellCoordinates, List<CellCoordinates>> dependencies, 
            Dictionary<int, Dictionary<int, object>> data)
        {
            foreach (var entry in dependencies)
            {
                CellCoordinates source = entry.Key;
                List<CellCoordinates> targets = entry.Value;

                Console.WriteLine($"Cell {source.ToString()} is fed by:");

                foreach (CellCoordinates target in targets)
                {
                    if (data.TryGetValue(target.RowNumber, out Dictionary<int, object> rowData))
                    {
                        if (rowData.TryGetValue(target.ColNumber, out object cellValue))
                        {
                            string targetValue = cellValue.ToString();
                            string replacedSource = data[source.RowNumber][source.ColNumber].ToString()
                                .Replace("=","")
                                .Replace(target.Id(), targetValue);
                            data[source.RowNumber][source.ColNumber] = "="+replacedSource;
                            Console.WriteLine($"- Replaced Source: {replacedSource}");
                        }
                    }
                }
            }
        }

        static public void PrintGraph(Dictionary<CellCoordinates, List<CellCoordinates>> dependencies)
        {
            foreach (var entry in dependencies)
            {
                var source = entry.Key;
                var targets = entry.Value;

                Console.WriteLine($"Cell {source.ToString()} is fed by :");

                foreach (var target in targets)
                {
                    Console.WriteLine($"- Cell {target.ToString()}");
                }
            }
        }
        static public void PrintGraph(Dictionary<int, Dictionary<int, object>> data, Dictionary<CellCoordinates, List<CellCoordinates>> dependencies)
        {
            foreach (var entry in dependencies)
            {
                var source = entry.Key;
                var targets = entry.Value;

                Console.WriteLine($"Cell source {source.ToString()} :" + data[source.RowNumber][source.ColNumber]);

                foreach (var target in targets)
                {
                    Console.WriteLine($"- Cell {target.ToString()} : " + data[target.RowNumber][target.ColNumber]);
                }
            }
        }



        static private object GetValueFromSource(Dictionary<int, Dictionary<int, object>> data, int columnIndex, int rowIndex)
        {
            // Implement your logic to retrieve the numeric value from the source based on the column and row index
            // For example, if source is a 2D array:
            return data[rowIndex][columnIndex];
        }

    }
}
