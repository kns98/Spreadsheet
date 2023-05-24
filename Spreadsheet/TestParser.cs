using System;
using System.Diagnostics;

namespace MicroSpread
{
    public class Test
    {
        public static void Test_Parser(string[] args)
        {
            // Create a sample expression
            string expression = "2 * (A1 + B2) - 5";

            // Create a SpreadsheetMirror object
            var spreadsheetMirror = new SpreadsheetMirror(3,3);

            // Set cell values
            spreadsheetMirror.SetCell(0, 0, 10);  // A1 = 10
            spreadsheetMirror.SetCell(1, 1, 20);  // B2 = 20

            // Create an instance of the Parser class
            var parser = new Parser(expression);

            try
            {
                // Parse and evaluate the expression using the SpreadsheetMirror
                double result = parser.Parse(spreadsheetMirror);
                Debug.Assert(result == 55.0, "Result does not match the expected value.");
                // Print the result
                Console.WriteLine("Result: " + result);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during parsing or evaluation
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }

   
}
