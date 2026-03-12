using System;
using System.Collections.Generic;
using System.Linq;

namespace Ae.Engine.Interrogation
{
    /// <summary>
    /// Represents a command definition for interrogation operations, including its name, parameters, and associated
    /// function key.
    /// </summary>
    /// <remarks>This class encapsulates the metadata and parameter values for a specific interrogation
    /// command. It provides access to the command's name, a collection of parameters, and a generated function key used
    /// for handler identification. Use this type to manage and retrieve parameter values for interrogation commands in
    /// a structured manner.</remarks>
    internal class InterrogationCommand
    {
        public string Name { get; set; }
        public List<InterrogationCommandParameter> Parameters { get; private set; } = new();

        public string PhysicalFunctionKey => $"CommandHandler_{Name}".Replace('-', '_').Replace("__", "_").ToLower();

        public T ParameterValue<T>(string parameterName, T defaultValue)
        {
            var parameter = Parameters.Where(o => o.Prototype.Name.ToLower() == parameterName.ToLower()).FirstOrDefault();
            if (parameter == null)
            {
                throw new Exception($"Parameter '{parameterName}' was not found.");
            }

            if (parameter.RawValue == null)
            {
                if (parameter.Prototype.IsRequired)
                {
                    throw new Exception($"Parameter '{parameter.Prototype.Name}' is not optional.");
                }
                return defaultValue;
            }

            return (T)Convert.ChangeType(parameter.RawValue, typeof(T));
        }

        public T? ParameterValue<T>(string parameterName)
        {
            var parameter = Parameters.Where(o => o.Prototype.Name.ToLower() == parameterName.ToLower()).FirstOrDefault();
            if (parameter == null)
            {
                throw new Exception($"Parameter '{parameterName}' was not found.");
            }

            if (parameter.RawValue == null)
            {
                if (parameter.Prototype.IsRequired)
                {
                    throw new Exception($"Parameter '{parameter.Prototype.Name}' is not optional.");
                }

                if (parameter.Prototype.DefaultValue != null)
                {
                    return (T)Convert.ChangeType(parameter.Prototype.DefaultValue, typeof(T));
                }

                return default;
            }

            return (T)Convert.ChangeType(parameter.RawValue, typeof(T));
        }

        public InterrogationCommand(string name)
        {
            Name = name;
        }
    }
}
