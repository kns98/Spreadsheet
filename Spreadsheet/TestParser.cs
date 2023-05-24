using System;
using System.Diagnostics;

namespace MicroSpread
{
    public class Test
    {
        public static void Test_Parser(string[] args)
        {
            Test_Expr();
            TestExpressionWithParentheses();
            TestExpressionWithMismatchedParentheses();
        }

        public static void Test_Expr()
        {
            // Create a sample expression
            string expression = "2 * (A1 + B2) - 5";

            // Create a SpreadsheetMirror object
            var spreadsheetMirror = new SpreadsheetMirror(3, 3);

            // Set cell values
            spreadsheetMirror.SetCell(0, 0, 10);  // A1 = 10
            spreadsheetMirror.SetCell(1, 1, 20);  // B2 = 20

            // Create an instance of the Parser class
            var parser = new Parser(expression);

            try
            {
                // Parse and evaluate the expression using the SpreadsheetMirror
                double result = parser.Parse(spreadsheetMirror);
                int expectedValue = 55;

                // Verify the result using assertions
                if (result != expectedValue)
                {
                    Console.WriteLine("Result does not match the expected value of " + expectedValue);
                }

                Console.WriteLine("Parser Success - Result: " + result);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during parsing or evaluation
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        public static void TestExpressionWithParentheses()
        {
            // Create a sample expression with parentheses
            string expression = "(2 + 3) * (A1 + B2) - 5";

            // Create a SpreadsheetMirror object
            var spreadsheetMirror = new SpreadsheetMirror(3, 3);

            // Set cell values
            spreadsheetMirror.SetCell(0, 0, 10);  // A1 = 10
            spreadsheetMirror.SetCell(1, 1, 20);  // B2 = 20

            // Create an instance of the Parser class
            var parser = new Parser(expression);

            try
            {
                // Parse and evaluate the expression using the SpreadsheetMirror
                double result = parser.Parse(spreadsheetMirror);
                int expectedValue = 145;

                // Verify the result using assertions
                if (result != expectedValue)
                {
                    Console.WriteLine("Result does not match the expected value of " + expectedValue);
                }

                // Print the result
                Console.WriteLine("Parser Success - Result: " + result);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during parsing or evaluation
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        public static void TestExpressionWithMismatchedParentheses()
        {
            // Create a sample expression with mismatched parentheses
            string expressionWithMismatch = "((2 + 3) * (A1 + B2) - 5";

            // Create a SpreadsheetMirror object
            var spreadsheetMirror = new SpreadsheetMirror(3, 3);

            // Set cell values
            spreadsheetMirror.SetCell(0, 0, 10);  // A1 = 10
            spreadsheetMirror.SetCell(1, 1, 20);  // B2 = 20

            // Create an instance of the Parser class
            var parserWithMismatch = new Parser(expressionWithMismatch);

            try
            {
                // Parse and evaluate the expression with mismatched parentheses using the SpreadsheetMirror
                double resultWithMismatch = parserWithMismatch.Parse(spreadsheetMirror);
            }
            catch (InvalidOperationException ex)
            {
                // Handle the exception for mismatched parentheses
                Console.WriteLine("Invalid Parenthesis Exception throw as expected");
            }
            catch (Exception ex)
            {
                // Handle the exception for mismatched parentheses
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }

}



