using MicroSpread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroSpread
{
    class SpreadsheetMirror
    {
        private Dictionary<(int, int), object> data;

        public SpreadsheetMirror(int numRows, int numCols)
        {
            data = new Dictionary<(int, int), object>();
        }

        public void SubscribeToSpreadsheet(Spreadsheet spreadsheet)
        {
            spreadsheet.cellChangeSubject.Subscribe(cellChangeEvent =>
            {
                SetCell(cellChangeEvent.Row, cellChangeEvent.Column, cellChangeEvent.Value);
            });
        }

        public object GetCell(int row, int col)
        {
            var key = (row, col);
            if (data.ContainsKey(key))
            {
                return data[key];
            }
            else
            {
                return null; // Or throw an exception if cell doesn't exist
            }
        }

        private void SetCell(int row, int col, object value)
        {
            var key = (row, col);
            if (!data.ContainsKey(key) || !data[key].Equals(value))
            {
                Console.WriteLine($"Mirror: Cell {row}-{col} changed. New value: {value}");
                data[key] = value;
            }
        }
    }
}
