using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace MicroSpread
{


    class Program
    {
        static void Main2(string[] args)
        {
            // Uncomment to run tests
            // Test.Test_Parser(new string[] { });

            // Create a new instance of the Spreadsheet class
            Spreadsheet spreadsheet = new Spreadsheet();

            // Create a new instance of the SpreadsheetMirror class
            SpreadsheetMirror mirror = new SpreadsheetMirror(3, 3);

            // Subscribe to cell change events in the spreadsheet
            IDisposable subscription1 = spreadsheet.cellChangeSubject.Subscribe(cellChangeEvent =>
            {
                // Print the cell change event details to the console
                Console.WriteLine($"Spreadsheet: Cell {cellChangeEvent.Row}-{cellChangeEvent.Column} changed. New value: {cellChangeEvent.Value}");

                //
                //TODO: Locking for concurrency. Cell could be edited before the list is retrieved.
                //
                // Check if the cell has dependencies
                if (spreadsheet.dependencies.ContainsKey(cellChangeEvent.Row) &&
                    spreadsheet.dependencies[cellChangeEvent.Row].ContainsKey(cellChangeEvent.Column))
                {
                    // Get the dependent cell coordinates
                    List<(int, int)> dependencies = spreadsheet.dependencies[cellChangeEvent.Row][cellChangeEvent.Column];

                    // Print the dependencies
                    Console.Write("Dependencies: ");
                    foreach ((int depRow, int depCol) in dependencies)
                    {
                        Console.Write($"({depRow}-{depCol}) ");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("Value: " + new Parser(cellChangeEvent.Value.ToString().Replace("=","")).Parse(mirror));
            });


            // Set cell values in the spreadsheet
            spreadsheet.SetCell(1, 1, "=B2"); // Cell A1 references cell B2
            spreadsheet.SetCell(2, 2, 42);
            spreadsheet.SetCell(3, 3, "=A1 + B2"); // Cell C3 references cells A1 and B2

        

            // Unsubscribe from cell change events in the spreadsheet and the mirror
            subscription1.Dispose();


            // Wait for user input to prevent the console from closing
            Console.ReadKey();
        }
    }
}
