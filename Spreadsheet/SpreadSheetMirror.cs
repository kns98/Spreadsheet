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
        private object[,] data;

        public SpreadsheetMirror(int numRows, int numCols)
        {
            data = new object[numRows, numCols];
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
            return data[row - 1, col - 1];
        }

        private void SetCell(int row, int col, object value)
        {
            if (data[row - 1, col - 1] != value)
            {
                Console.WriteLine($"Mirror: Cell {row}-{col} changed. New value: {value}");
                data[row - 1, col - 1] = value;
            }
        }

    }
}