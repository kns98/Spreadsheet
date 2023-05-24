using System;
using System.Collections.Generic;

public class Evaluator
{

    /// <summary>
    /// Builds the cell dependencies based on the dependency texts.
    /// </summary>
    /// <param name="dependencyTexts">The array of dependency texts.</param>
    /// <param name="data">The data containing cell values.</param>
    /// <returns>A dictionary of cell dependencies.</returns>
    static Dictionary<int, Dictionary<int, List<(int, int)>>> BuildDependencies(string[][] dependencyTexts, Dictionary<int, Dictionary<int, object>> data)
    {
        Dictionary<int, Dictionary<int, List<(int, int)>>> dependencies = new Dictionary<int, Dictionary<int, List<(int, int)>>>();

        for (int row = 1; row <= dependencyTexts.Length; row++)
        {
            Dictionary<int, List<(int, int)>> rowDependencies = new Dictionary<int, List<(int, int)>>();

            string[] colDependencies = dependencyTexts[row - 1];

            for (int col = 1; col <= colDependencies.Length; col++)
            {
                string coordinate = colDependencies[col - 1];

                if (coordinate.Length > 0)
                {
                    (int, int) parsedCoordinate = ParseCoordinates(coordinate);

                    if (!rowDependencies.ContainsKey(col))
                    {
                        rowDependencies[col] = new List<(int, int)>();
                    }

                    rowDependencies[col].Add(parsedCoordinate);
                }
            }

            dependencies[row] = rowDependencies;
        }

        return dependencies;
    }


    // Function to parse coordinate string and return as a tuple
    static (int, int) ParseCoordinates(string coordinate)
    {
        if (coordinate=="") return (-1, -1);
        string[] parts = coordinate.Split('R', 'C');

        int row = int.Parse(parts[1]);
        int col = int.Parse(parts[2]);

        return (row, col);
    }

    public static void Main()
    {
        // Sample data
        Dictionary<int, Dictionary<int, object>> data = new Dictionary<int, Dictionary<int, object>>
        {
            { 1, new Dictionary<int, object> { { 1, 10 }, { 2, 20 }, { 3, "=A1+B1" } } },
            { 2, new Dictionary<int, object> { { 1, "=A1+5" }, { 2, "=A1+B1" }, { 3, 30 } } },
            { 3, new Dictionary<int, object> { { 1, "=A2+A3" }, { 2, "=B1+B2" }, { 3, "=C1+C2" } } }
        };

        string[][] dependencyTexts = new string[][]
        {
            new string[] { "", "", "R1C1 R1C2" },
            new string[] { "R1C1", "R1C1 R1C2", "" },
            new string[] { "R2C1 R3C1", "R1C2 C2R2", "R1C3 R2C3" }
        };


        Dictionary<int, Dictionary<int, List<(int, int)>>> deps = BuildDependencies(dependencyTexts, data);

        // Create an instance of the DataParser
        DataParser parser = new DataParser();

        // Parse the data and dependencies
        parser.ParseData(data, deps);

        parser.dependencyGraph.PrintGraph();

        // Evaluate the spreadsheet
        //Dictionary<CellCoordinates, object> evaluatedCells = parser.EvaluateSpreadsheet();
        // Display the evaluated values
        //foreach (var cell in evaluatedCells)
        //{
        //    Console.WriteLine($"Cell ({cell.Key.RowNumber}, {cell.Key.ColNumber}): {cell.Value}");
        //}
    }
}


public class CellCoordinates
{
    public int RowNumber { get; }
    public int ColNumber { get; }

    public CellCoordinates(int rowNumber, int colNumber)
    {
        RowNumber = rowNumber;
        ColNumber = colNumber;
    }

    public override string ToString()
    {
        return $"Row {RowNumber}, Column {ColNumber}";
    }
}


public class DataParser
{
    private Dictionary<CellCoordinates, object> cellValues;
    public DependencyGraph dependencyGraph;

    public DataParser()
    {
        cellValues = new Dictionary<CellCoordinates, object>();
        dependencyGraph = new DependencyGraph();
    }

    public void ParseData(Dictionary<int, Dictionary<int, object>> data, Dictionary<int, Dictionary<int, List<(int, int)>>> dependencies)
    {
        foreach (var row in data)
        {
            int rowNumber = row.Key;
            var rowData = row.Value;

            foreach (var cell in rowData)
            {
                int colNumber = cell.Key;
                object value = cell.Value;

                // Update cell values dictionary
                cellValues[new CellCoordinates(rowNumber, colNumber)] = value;

                // Update dependency graph
                UpdateDependencyGraph(new CellCoordinates(rowNumber, colNumber), value.ToString(), dependencies);
            }
        }
    }

    private void UpdateDependencyGraph(CellCoordinates cell, string value, Dictionary<int, Dictionary<int, List<(int, int)>>> dependencies)
    {
        if (dependencies.ContainsKey(cell.RowNumber) && dependencies[cell.RowNumber].ContainsKey(cell.ColNumber))
        {
            // Get dependencies from the provided dictionary
            List<(int, int)> cellDependencies = dependencies[cell.RowNumber][cell.ColNumber];

            // Remove old dependencies for the cell
            List<CellCoordinates> oldDependencies = dependencyGraph.GetDependencies(cell);
            foreach (var dependency in oldDependencies)
            {
                dependencyGraph.RemoveDependency(dependency, cell);
            }

            // Add new dependencies for the cell
            foreach (var dependency in cellDependencies)
            {
                dependencyGraph.AddDependency(new CellCoordinates(dependency.Item1, dependency.Item2), cell);
            }
        }
        else
        {
            // No dependencies provided for the cell
            List<CellCoordinates> emptyDependencies = new List<CellCoordinates>();

            // Remove old dependencies for the cell
            List<CellCoordinates> oldDependencies = dependencyGraph.GetDependencies(cell);
            foreach (var dependency in oldDependencies)
            {
                dependencyGraph.RemoveDependency(dependency, cell);
            }

            // Add empty dependencies for the cell
            foreach (var dependency in emptyDependencies)
            {
                dependencyGraph.AddDependency(dependency, cell);
            }
        }
    }

    public Dictionary<CellCoordinates, object> EvaluateSpreadsheet()
    {
        var visited = new HashSet<CellCoordinates>();
        var evaluated = new Dictionary<CellCoordinates, object>();

        foreach (var cell in cellValues.Keys)
        {
            EvaluateCell(cell, visited, evaluated);
        }

        return evaluated;
    }

    private object EvaluateCell(CellCoordinates cell, HashSet<CellCoordinates> visited, Dictionary<CellCoordinates, object> evaluated)
    {
        if (evaluated.ContainsKey(cell))
        {
            // Cell already evaluated, return the value
            return evaluated[cell];
        }

        if (visited.Contains(cell))
        {
            // Circular dependency detected, return null or appropriate error value
            return null;
        }

        visited.Add(cell);

        var dependencies = dependencyGraph.GetDependencies(cell);
        object value = cellValues[cell];

        if (dependencies.Count > 0)
        {
            // Evaluate dependencies
            var dependencyValues = new List<object>();
            foreach (var dependency in dependencies)
            {
                var dependencyValue = EvaluateCell(dependency, visited, evaluated);
                dependencyValues.Add(dependencyValue);
            }

            // Evaluate the current cell's value based on the dependencies
            value = EvaluateCellFormula(value.ToString(), dependencyValues);
        }

        evaluated[cell] = value;
        visited.Remove(cell);

        return value;
    }

    private object EvaluateCellFormula(string formula, List<object> dependencies)
    {
        // Custom logic to evaluate the cell formula based on the dependencies
        // Implement your own formula evaluation logic here
        // This is just a placeholder

        // For example, assume the formula is a sum of dependencies
        int sum = 0;
        foreach (var dependency in dependencies)
        {
            if (dependency is int)
            {
                sum += (int)dependency;
            }
        }

        return sum;
    }
}

public class DependencyGraph
{
    private Dictionary<CellCoordinates, List<CellCoordinates>> dependencies;

    public void PrintGraph()
    {
        foreach (var entry in dependencies)
        {
            var source = entry.Key;
            var targets = entry.Value;

            Console.WriteLine($"Cell {source.ToString()} feeds :");

            foreach (var target in targets)
            {
                Console.WriteLine($"- Cell {target.ToString()}");
            }
        }
    }

    public DependencyGraph()
    {
        dependencies = new Dictionary<CellCoordinates, List<CellCoordinates>>();
    }

    public void AddDependency(CellCoordinates source, CellCoordinates target)
    {
        if (dependencies.ContainsKey(source))
        {
            dependencies[source].Add(target);
        }
        else
        {
            dependencies[source] = new List<CellCoordinates> { target };
        }
    }

    public void RemoveDependency(CellCoordinates source, CellCoordinates target)
    {
        if (dependencies.ContainsKey(source))
        {
            dependencies[source].Remove(target);
            if (dependencies[source].Count == 0)
            {
                dependencies.Remove(source);
            }
        }
    }

    public List<CellCoordinates> GetDependencies(CellCoordinates cell)
    {
        if (dependencies.ContainsKey(cell))
        {
            return dependencies[cell];
        }

        return new List<CellCoordinates>();
    }
}
