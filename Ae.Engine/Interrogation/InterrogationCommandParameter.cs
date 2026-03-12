using System;
using System.Text.RegularExpressions;
using static Ae.Engine.Interrogation.InterrogationCommandParameterPrototype;

namespace Ae.Engine.Interrogation
{
    /// <summary>
    /// Represents a parameter for an interrogation command, including its prototype and parsed value.
    /// </summary>
    /// <remarks>This class is intended for internal use in command processing scenarios. It encapsulates the
    /// parameter's definition and its value, which may be converted to a specific type based on the prototype. The
    /// parameter type determines how the raw value is parsed and validated.</remarks>
    internal class InterrogationCommandParameter
    {
        public InterrogationCommandParameterPrototype Prototype { get; private set; }
        public object? RawValue { get; private set; }

        public InterrogationCommandParameter(InterrogationCommandParameterPrototype prototype, string? value)
        {
            Prototype = prototype;
            RawValue = value;

            if (value != null)
            {
                if (prototype.CommandParameterType == DebugCommandParameterType.String)
                {
                    RawValue = value; //No validation required.
                }
                else if (prototype.CommandParameterType == DebugCommandParameterType.Numeric)
                {
                    if (double.TryParse(value, out var validatedValue) == false)
                    {
                        throw new Exception($"Could not convert value '{value}' for '{prototype.Name}' to numeric.");
                    }
                    RawValue = validatedValue;
                }
                else if (prototype.CommandParameterType == DebugCommandParameterType.Boolean)
                {
                    if (bool.TryParse(value, out var validatedValue) == false)
                    {
                        throw new Exception($"Could not convert value '{value}' for '{prototype.Name}' to boolean.");
                    }
                    RawValue = validatedValue;
                }
                else if (prototype.CommandParameterType == DebugCommandParameterType.Criterion)
                {
                    RawValue = new InterrogationCommandParameterCriterion(value);
                }
                else
                {
                    throw new Exception($"Unknown parameter type '{prototype.CommandParameterType}' for '{prototype.Name}'.");
                }
            }
        }

        public static bool IsMatchLike(string input, string pattern, bool isNotLike = false)
        {
            if (input == null || pattern == null)
            {
                return false;
            }

            input = input.ToLower();
            pattern = pattern.ToLower();

            string regexPattern = "^" + Regex.Escape(pattern).Replace("%", ".*").Replace("_", ".") + "$";
            var result = Regex.IsMatch(input, regexPattern);
            return isNotLike ? !result : result;
        }
    }
}
