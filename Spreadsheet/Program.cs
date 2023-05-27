using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace MicroSpread
{
    class Program
    {
        static void Main(string[] args)
        {
            // Uncomment to run tests
            // Test.Test_Parser(new string[] { });

            // Create a new instance of the Spreadsheet class
            Spreadsheet spreadsheet = new Spreadsheet();

            // Create a new instance of the SpreadsheetMirror class
            SpreadsheetMirror mirror = new SpreadsheetMirror(4, 4);

            // Subscribe to cell change events in the spreadsheet
            IDisposable subscription1 = spreadsheet.cellChangeSubject.Subscribe(cellChangeEvent =>
            {
                // Print the cell change event details to the console
                Console.WriteLine($"Spreadsheet: Cell {cellChangeEvent.Coordinates.RowNumber}-{cellChangeEvent.Coordinates.ColNumber} changed. New value: {cellChangeEvent.Value}");

                //
                //TODO: Locking for concurrency. Cell could be edited before the list is retrieved.
                //
                // Check if the cell has dependencies
                if (spreadsheet.dependencies.ContainsKey(cellChangeEvent.Coordinates))
                {
                    // Get the dependent cell coordinates
                    List<CellCoordinates> dependencies = spreadsheet.dependencies[cellChangeEvent.Coordinates];

                    // Print the dependencies
                    Console.Write("Dependencies: ");
                    foreach (CellCoordinates dependency in dependencies)
                    {
                        Console.Write($"({dependency.RowNumber}-{dependency.ColNumber}) ");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("Value: " + new Parser(cellChangeEvent.Value.ToString().Replace("=", "")).Parse(mirror));
            });


            // Set cell values in the spreadsheet
            spreadsheet.SetCell(new CellCoordinates(1, 1), "=B2"); // Cell A1 references cell B2
            spreadsheet.SetCell(new CellCoordinates(2, 2), "=D3");
            spreadsheet.SetCell(new CellCoordinates(3, 4), "42");
            spreadsheet.SetCell(new CellCoordinates(3, 3), "=A1 + B2"); // Cell C3 references cells A1 and B2

            Evaluator evaluator = new Evaluator();
            evaluator.EvalSheet(spreadsheet);

            // Unsubscribe from cell change events in the spreadsheet and the mirror
            subscription1.Dispose();

            // Wait for user input to prevent the console from closing
            Console.ReadKey();
        }
    }
}
