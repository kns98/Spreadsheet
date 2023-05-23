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

        public double Parse()
        {
            var tokens = Tokenize();
            var parsedExpression = ParseExpression(tokens);
            return Evaluate(parsedExpression);
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

        private double Evaluate(List<string> parsedExpression)
        {
            var valueStack = new Stack<double>();

            foreach (var token in parsedExpression)
            {
                if (IsNumber(token))
                {
                    if (IsVariable(token))
                    {
                        var variableValue = EvaluateVariable(token);
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
                throw new InvalidOperationException("Invalid expression.");
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

        private double EvaluateVariable(string token)
        {
            var letter = token[0];
            var number = int.Parse(token[1].ToString());

            var variableValues = new Dictionary<char, double>
        {
            { 'A', 10 },
            { 'B', 20 },
            { 'C', 30 }
        };

            if (variableValues.ContainsKey(letter))
            {
                return variableValues[letter] * number;
            }
            else
            {
                throw new InvalidOperationException("Unknown variable: " + token);
            }
        }
    }
}