using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroSpread
{
    public class Parser
    {
        private string expression;
        private int position;

        public Parser(string expression)
        {
            this.expression = expression;
            this.position = 0;
        }

        public double Parse(SpreadsheetMirror s)
        {
            var tokens = Tokenize();
            var parsedExpression = ParseExpression(tokens);
            return Evaluate(parsedExpression, s);
        }

        private List<string> Tokenize()
        {
            var tokens = new List<string>();
            while (position < expression.Length)
            {
                if (char.IsWhiteSpace(expression[position]))
                {
                    position++;
                    continue;
                }

                if (char.IsDigit(expression[position]))
                {
                    var token = "";
                    while (position < expression.Length && char.IsDigit(expression[position]))
                    {
                        token += expression[position];
                        position++;
                    }
                    tokens.Add(token);
                }
                else if (char.IsLetter(expression[position]))
                {
                    var token = "";
                    while (position < expression.Length && char.IsLetterOrDigit(expression[position]))
                    {
                        token += expression[position];
                        position++;
                    }
                    tokens.Add(token);
                }
                else
                {
                    tokens.Add(expression[position].ToString());
                    position++;
                }
            }

            return tokens;
        }

        private List<string> ParseExpression(List<string> tokens)
        {
            var parsedExpression = new List<string>();
            var operatorStack = new Stack<string>();

            foreach (var token in tokens)
            {
                if (IsNumber(token))
                {
                    parsedExpression.Add(token);
                }
                else if (IsOperator(token))
                {
                    while (operatorStack.Count > 0 && IsOperator(operatorStack.Peek()) &&
                           GetPrecedence(token) <= GetPrecedence(operatorStack.Peek()))
                    {
                        parsedExpression.Add(operatorStack.Pop());
                    }

                    operatorStack.Push(token);
                }
                else if (token == "(")
                {
                    operatorStack.Push(token);
                }
                else if (token == ")")
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                    {
                        parsedExpression.Add(operatorStack.Pop());
                    }

                    if (operatorStack.Count == 0 || operatorStack.Peek() != "(")
                    {
                        throw new InvalidOperationException("Mismatched parentheses.");
                    }

                    operatorStack.Pop();
                }
            }

            while (operatorStack.Count > 0)
            {
                if (operatorStack.Peek() == "(" || operatorStack.Peek() == ")")
                {
                    throw new InvalidOperationException("Mismatched parentheses.");
                }

                parsedExpression.Add(operatorStack.Pop());
            }

            return parsedExpression;
        }

        private double Evaluate(List<string> parsedExpression, SpreadsheetMirror s)
        {
            var valueStack = new Stack<double>();

            foreach (var token in parsedExpression)
            {
                if (IsNumber(token))
                {
                    if (IsVariable(token))
                    {
                        var variableValue = EvaluateVariable(token, s);
                        valueStack.Push(variableValue);
                    }
                    else
                    {
                        valueStack.Push(double.Parse(token));
                    }
                }
                else if (IsOperator(token))
                {
                    if (valueStack.Count < 2)
                    {
                        throw new InvalidOperationException("Invalid expression.");
                    }

                    var operand2 = valueStack.Pop();
                    var operand1 = valueStack.Pop();

                    double result;
                    switch (token)
                    {
                        case "+":
                            result = operand1 + operand2;
                            break;
                        case "-":
                            result = operand1 - operand2;
                            break;
                        case "*":
                            result = operand1 * operand2;
                            break;
                        case "/":
                            result = operand1 / operand2;
                            break;
                        default:
                            throw new InvalidOperationException("Invalid operator.");
                    }

                    valueStack.Push(result);
                }
            }

            if (valueStack.Count != 1)
            {
                //throw new InvalidOperationException("Invalid expression.");
                return 0;
            }

            return valueStack.Pop();
        }

        private bool IsNumber(string token)
        {
            double result;
            return double.TryParse(token, out result) || IsVariable(token);
        }

        private bool IsVariable(string token)
        {
            return token.Length == 2 && char.IsLetter(token[0]) && char.IsDigit(token[1]);
        }

        private bool IsOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/";
        }

        private int GetPrecedence(string token)
        {
            switch (token)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                default:
                    return 0;
            }
        }

        private double EvaluateVariable(string token, SpreadsheetMirror s)
        {
            var letter = (int)token[0] - (int)'A';
            var number = int.Parse(token[1].ToString()) - 1;

            var data = s.data;

            if (data.ContainsKey((letter, number)))
            {
                var value = data[(letter, number)];

                if (value is int)
                {
                    return (int)value;
                }
                else if (value is string)
                {
                    var expression = (string)value;
                    // Evaluate the expression if needed and return the result
                    return new Parser(expression).Parse(s);
                }
                else
                {
                    throw new InvalidOperationException("Invalid value type: " + value.GetType().Name);
                }
            }
            else
            {
                return 0;
                //throw new InvalidOperationException("Unknown variable: " + token);
            }
        }
    }
}