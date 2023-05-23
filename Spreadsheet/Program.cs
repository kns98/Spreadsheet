using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace MicroSpread
{

    // Class representing a cell change event
    class CellChangeEvent
    {
        public int Row { get; }
        public int Column { get; }
        public object Value { get; }

        public CellChangeEvent(int row, int col, object value)
        {
            Row = row;
            Column = col;
            Value = value;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Create the original instance of the Spreadsheet class
            Spreadsheet originalSpreadsheet = new Spreadsheet();

            // Create the second instance of the Spreadsheet class
            Spreadsheet secondSpreadsheet = new Spreadsheet();

            // Subscribe to cell change events for the original spreadsheet
            IDisposable subscription1 = originalSpreadsheet.cellChangeSubject.Subscribe(cellChangeEvent =>
            {
                // Print the cell change event details to the console
                Console.WriteLine($"Original: Cell {cellChangeEvent.Row}-{cellChangeEvent.Column} changed. New value: {cellChangeEvent.Value}");

                // Update the corresponding cell in the second spreadsheet
                secondSpreadsheet.SetCell(cellChangeEvent.Row, cellChangeEvent.Column, cellChangeEvent.Value);
            });

            // Subscribe to cell change events for the second spreadsheet
            IDisposable subscription2 = secondSpreadsheet.cellChangeSubject.Subscribe(cellChangeEvent =>
            {
                // Print the cell change event details to the console
                Console.WriteLine($"Second: Cell {cellChangeEvent.Row}-{cellChangeEvent.Column} changed. New value: {cellChangeEvent.Value}");
            });

            // Set cell values in the original spreadsheet
            originalSpreadsheet.SetCell(1, 1, "Hello");
            originalSpreadsheet.SetCell(2, 2, 42);
            originalSpreadsheet.SetCell(1, 1, "World");
            originalSpreadsheet.SetCell(3, 3, 3.14);

            // Unsubscribe from cell change events for both spreadsheets
            subscription1.Dispose();
            subscription2.Dispose();

            // Wait for user input to prevent the console from closing
            Console.ReadKey();
        }
    }
}