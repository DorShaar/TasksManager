using System;
using System.Text;
using Tasker.Extensions;

namespace Tasker.UserInputs
{
    internal sealed class UserInput
    {
        public string InputString { get; private set; }
        public string[] GetArguments()
        {
            return ParseInputString();
        }

        public void GetUserInput()
        {
            InputString = Console.ReadLine();
        }

        private string[] ParseInputString()
        {
            string[] arguments = new string[10];
            StringBuilder stringBuilder = new StringBuilder();

            int argumentsSize = 0;
            bool isInsideQuotationMarks = false;
            bool isLastCharWasWhiteSpace = false;
            foreach (char ch in InputString)
            {
                if (ch == '"')
                {
                    isInsideQuotationMarks = !isInsideQuotationMarks;
                    continue;
                }

                if (char.IsWhiteSpace(ch))
                {
                    if (isLastCharWasWhiteSpace)
                        continue;

                    if (!isInsideQuotationMarks)
                    {
                        arguments[argumentsSize] = stringBuilder.ToString();
                        stringBuilder.Clear();
                        argumentsSize++;
                        isLastCharWasWhiteSpace = true;

                        continue;
                    }

                    stringBuilder.Append(ch);
                }
                else
                {
                    isLastCharWasWhiteSpace = false;
                    stringBuilder.Append(ch);
                }
            }

            arguments[argumentsSize] = stringBuilder.ToString();
            argumentsSize++;
            stringBuilder.Clear();

            return arguments.Slice(0, argumentsSize);
        }
    }
}