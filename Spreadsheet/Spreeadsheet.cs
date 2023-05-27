using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;

namespace MicroSpread
{

    public class Spreadsheet
    {
        // Dictionary to store cell values, where the key is the cell coordinates and the value is the cell value
        private Dictionary<CellCoordinates, object> data;

        // Dictionary to store cell dependencies, where the key is the cell coordinates and the value is a list of dependent cell coordinates
        public Dictionary<CellCoordinates, List<CellCoordinates>> dependencies;

        // Subject to publish cell change events
        public Subject<CellChangeEvent> cellChangeSubject;

        public Spreadsheet()
        {
            data = new Dictionary<CellCoordinates, object>();
            dependencies = new Dictionary<CellCoordinates, List<CellCoordinates>>();
            cellChangeSubject = new Subject<CellChangeEvent>();
        }

        // Method to set the value of a cell
        public void SetCell(CellCoordinates coordinates, object value)
        {
            // Create a new row dictionary if the cell coordinates do not exist
            if (!data.ContainsKey(coordinates))
            {
                data[coordinates] = null;
                dependencies[coordinates] = new List<CellCoordinates>();
            }

            // Check if the cell value has changed
            if (data[coordinates] == value || (data[coordinates] != null && data[coordinates].Equals(value)))
            {
                // Cell value unchanged, no need to notify
                return;
            }

            // Update the cell value
            data[coordinates] = value;

            // Update the dependencies for the cell
            dependencies[coordinates] = FindDependencies(value.ToString());

            // Publish the cell change event
            cellChangeSubject.OnNext(new CellChangeEvent(coordinates, value));
        }

        // Method to get the value of a cell
        public object GetCell(CellCoordinates coordinates)
        {
            // Check if the cell coordinates exist in the dictionary
            if (data.ContainsKey(coordinates))
            {
                // Return the cell value
                return data[coordinates];
            }

            // Cell not found
            return null;
        }

        // Method to delete a cell
        public void DeleteCell(CellCoordinates coordinates)
        {
            // Check if the cell coordinates exist
            if (data.ContainsKey(coordinates))
            {
                // Remove the cell from the dictionary
                data.Remove(coordinates);
                dependencies.Remove(coordinates);

                // Publish the cell change event with null value to indicate deletion
                cellChangeSubject.OnNext(new CellChangeEvent(coordinates, null));
            }
        }

        // Method to find cell dependencies in an expression
        private List<CellCoordinates> FindDependencies(string expression)
        {
            List<CellCoordinates> dependentCells = new List<CellCoordinates>();

            // Extract row and cell combinations using regular expressions
            string pattern = @"[A-Za-z]+[0-9]+";
            MatchCollection matches = Regex.Matches(expression, pattern);

            // Iterate through the matches
            foreach (Match match in matches)
            {
                if (TryParseCellCoordinate(match.Value, out CellCoordinates coordinates))
                {
                    // Add the dependent cell coordinates to the list
                    dependentCells.Add(coordinates);

                    // Initialize the dependent cell if it does not exist
                    if (!data.ContainsKey(coordinates))
                    {
                        // Push events for these cells
                        SetCell(coordinates, "");
                    }
                }
            }

            return dependentCells;
        }

        // Method to parse cell coordinates from a string (e.g., "A1" or "B2")
        private bool TryParseCellCoordinate(string cellCoordinate, out CellCoordinates coordinates)
        {
            coordinates = null;

            if (cellCoordinate.Length < 2)
            {
                return false;
            }

            char colChar = cellCoordinate[0];
            string rowStr = cellCoordinate.Substring(1);

            if (!int.TryParse(rowStr, out int row))
            {
                return false;
            }

            int col = colChar - 'A' + 1;

            coordinates = new CellCoordinates(row, col);
            return true;
        }


        // Method to calculate the value of a cell based on an expression
        private object CalculateCellValue(CellCoordinates coordinates)
        {
            // TODO: Implement your cell value calculation logic here
            // This is just a placeholder implementation that returns the expression itself as the cell value
            return coordinates.Id();
        }

        // Accessor method to provide access to the data as Dictionary<int, Dictionary<int, object>>
        public Dictionary<int, Dictionary<int, object>> GetData()
        {
            // Create a new dictionary to hold the data
            Dictionary<int, Dictionary<int, object>> result = new Dictionary<int, Dictionary<int, object>>();

            foreach (var entry in data)
            {
                CellCoordinates coordinates = entry.Key;
                object value = entry.Value;

                // Get the row number and column number from the cell coordinates
                int rowNumber = coordinates.RowNumber;
                int colNumber = coordinates.ColNumber;

                // Check if the row exists in the result dictionary
                if (!result.ContainsKey(rowNumber))
                {
                    result[rowNumber] = new Dictionary<int, object>();
                }

                // Add the cell value to the corresponding row and column in the result dictionary
                result[rowNumber][colNumber] = value;
            }

            return result;
        }
    }
}


