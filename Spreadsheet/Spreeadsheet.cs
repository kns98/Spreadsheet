
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;

namespace MicroSpread
{

    class Spreadsheet
    {
        // Dictionary to store cell values, where the key is the row number and the value is a dictionary of column numbers and cell values
        private Dictionary<int, Dictionary<int, object>> data;

        // Dictionary to store cell dependencies, where the key is the row number and the value is a dictionary of column numbers and dependent cell coordinates
        public Dictionary<int, Dictionary<int, List<(int, int)>>> dependencies;

        // Subject to publish cell change events
        public Subject<CellChangeEvent> cellChangeSubject;

        public Spreadsheet()
        {
            data = new Dictionary<int, Dictionary<int, object>>();
            dependencies = new Dictionary<int, Dictionary<int, List<(int, int)>>>();
            cellChangeSubject = new Subject<CellChangeEvent>();
        }

        // Method to set the value of a cell
        public void SetCell(int row, int col, object value)
        {
            // Create a new row dictionary if the row does not exist
            if (!data.ContainsKey(row))
            {
                data[row] = new Dictionary<int, object>();
                dependencies[row] = new Dictionary<int, List<(int, int)>>();
            }

            // Check if the cell value has changed
            if (data[row].ContainsKey(col) && data[row][col] == value)
            {
                // Cell value unchanged, no need to notify
                return;
            }

            // Update the cell value
            data[row][col] = value;

            // Update the dependencies for the cell
            dependencies[row][col] = FindDependencies(value.ToString());

            // Publish the cell change event
            cellChangeSubject.OnNext(new CellChangeEvent(row, col, value));

        }

        // Method to get the value of a cell
        public object GetCell(int row, int col)
        {
            // Check if the row and column exist in the dictionary
            if (data.ContainsKey(row) && data[row].ContainsKey(col))
            {
                // Return the cell value
                return data[row][col];
            }

            // Cell not found
            return null;
        }

        // Method to delete a cell
        public void DeleteCell(int row, int col)
        {
            // Check if the row exists
            if (data.ContainsKey(row))
            {
                // Check if the cell exists
                if (data[row].ContainsKey(col))
                {
                    // Remove the cell from the row dictionary
                    data[row].Remove(col);

                    // Remove the dependencies for the cell
                    dependencies[row][col].Clear();

                    // Publish the cell change event with null value to indicate deletion
                    cellChangeSubject.OnNext(new CellChangeEvent(row, col, null));
                }

                // Check if the row dictionary is empty after deletion
                if (data[row].Count == 0)
                {
                    // Remove the row if it has no cells
                    data.Remove(row);
                    dependencies[row] = null;
                }
            }
        }

        // Method to find cell dependencies in an expression
        private List<(int, int)> FindDependencies(string expression)
        {
            List<(int, int)> dependentCells = new List<(int, int)>();

            // Extract row and cell combinations using regular expressions
            string pattern = @"[A-Za-z]+[0-9]+";
            MatchCollection matches = Regex.Matches(expression, pattern);

            // Iterate through the matches
            foreach (Match match in matches)
            {
                if (TryParseCellCoordinate(match.Value, out int cellRow, out int cellCol))
                {
                    // Add the dependent cell coordinates to the list
                    dependentCells.Add((cellRow, cellCol));

                    // Initialize the dependent cell if it does not exist
                    if (!data.ContainsKey(cellRow) || !data[cellRow].ContainsKey(cellCol))
                    {
                        // Push events for these cells
                        SetCell(cellRow, cellCol, "");
                    }
                }
            }

            return dependentCells;
        }

        // Method to parse cell coordinates from a string (e.g., "A1" or "B2")
        private bool TryParseCellCoordinate(string cellCoordinate, out int row, out int col)
        {
            row = -1;
            col = -1;

            if (cellCoordinate.Length < 2)
            {
                return false;
            }

            char colChar = cellCoordinate[0];
            string rowStr = cellCoordinate.Substring(1);

            if (!int.TryParse(rowStr, out row))
            {
                return false;
            }

            col = colChar - 'A' + 1;

            return true;
        }


        // Method to calculate the value of a cell based on an expression
        private object CalculateCellValue(string expression)
        {
            // TODO: Implement your cell value calculation logic here
            // This is just a placeholder implementation that returns the expression itself as the cell value
            return expression;
        }
    }

}
