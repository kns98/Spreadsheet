using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace MicroSpread
{
    class Spreadsheet
    {
        // Dictionary to store cell values, where the key is the row number and the value is a dictionary of column numbers and cell values
        private Dictionary<int, Dictionary<int, object>> data;

        // Subject to publish cell change events
        public Subject<CellChangeEvent> cellChangeSubject;

        public Spreadsheet()
        {
            data = new Dictionary<int, Dictionary<int, object>>();
            cellChangeSubject = new Subject<CellChangeEvent>();
        }

        // Method to set the value of a cell
        public void SetCell(int row, int col, object value)
        {
            // Create a new row dictionary if the row does not exist
            if (!data.ContainsKey(row))
            {
                data[row] = new Dictionary<int, object>();
            }

            // Check if the cell value has changed
            if (data[row].ContainsKey(col) && data[row][col] == value)
            {
                // Cell value unchanged, no need to notify
                return;
            }

            // Update the cell value
            data[row][col] = value;

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

                    // Publish the cell change event with null value to indicate deletion
                    cellChangeSubject.OnNext(new CellChangeEvent(row, col, null));
                }

                // Check if the row dictionary is empty after deletion
                if (data[row].Count == 0)
                {
                    // Remove the row if it has no cells
                    data.Remove(row);
                }
            }
        }


    }
}




