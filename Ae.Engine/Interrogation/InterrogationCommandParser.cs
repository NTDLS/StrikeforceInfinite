using System;
using System.Collections.Generic;
using System.Linq;
using static Ae.Engine.Interrogation.InterrogationCommandParameterPrototype;

namespace Ae.Engine.Interrogation
{
    /// <summary>
    /// Provides functionality for parsing interrogation command prototypes and constructing command definitions from
    /// prototype strings.
    /// </summary>
    /// <remarks>This class is typically used to initialize a set of supported interrogation commands from
    /// prototype definitions. Each prototype string must follow the expected format; otherwise, an exception is thrown
    /// during initialization. The parser supports commands with required and optional parameters, and maintains a
    /// collection of parsed command prototypes for later use.</remarks>
    public class InterrogationCommandParser
    {
        /// <summary>
        /// Gets the collection of interrogation command prototypes associated with this instance.
        /// </summary>
        public List<InterrogationCommandPrototype> Commands { get; private set; } = new();

        /// <summary>
        /// Initializes a new instance of the InterrogationCommandParser class using the specified command prototypes.
        /// </summary>
        /// <remarks>Each command prototype string must follow the format:
        /// 'CommandName|ParameterList|Description'. ParameterList is a comma-separated list of parameters, each
        /// formatted as 'ParameterName:RequiredOrOptional[=DefaultValue]:ParameterType'.</remarks>
        /// <param name="prototypes">An array of strings representing command prototypes. Each prototype must be formatted as a pipe-delimited
        /// string with three segments: command name, parameters, and description.</param>
        /// <exception cref="Exception">Thrown if any command prototype or parameter is malformed and does not conform to the expected format.</exception>
        public InterrogationCommandParser(string[] prototypes)
        {
            foreach (var prototype in prototypes)
            {
                var commandParts = prototype.Split('|');

                if (commandParts.Length != 3)
                {
                    throw new Exception("Malformed debug command prototype.");
                }

                var command = new InterrogationCommandPrototype(commandParts[0], commandParts[2]);

                var commandParameters = commandParts[1].Split(",");

                foreach (var commandParameter in commandParameters)
                {
                    if (string.IsNullOrEmpty(commandParameter) == false)
                    {
                        var commandParameterParts = commandParameter.Split(":");

                        if (commandParameterParts.Length != 3)
                        {
                            throw new Exception("Malformed debug command prototype parameter.");
                        }

                        bool isRequired = commandParameterParts[1].Trim().ToLower() == "required";
                        string? defaultValue = null;

                        if (isRequired == false && commandParameterParts[1].Contains('='))
                        {
                            int indexOfEqual = commandParameterParts[1].IndexOf('=');
                            defaultValue = commandParameterParts[1].Substring(indexOfEqual + 1).Trim();
                        }

                        command.Parameters.Add(new InterrogationCommandParameterPrototype(
                            commandParameterParts[0], isRequired, defaultValue,
                            Enum.Parse<DebugCommandParameterType>(commandParameterParts[2], true)
                        ));
                    }
                }

                Commands.Add(command);
            }
        }

        /// <summary>
        /// Parses the specified command text and constructs an interrogation command with its parameters.
        /// </summary>
        /// <remarks>Parameter values are assigned in order as specified in the command text. Optional
        /// parameters not supplied are set to null. The parsing is case-insensitive for command names.</remarks>
        /// <param name="commandText">The command text to parse. Must contain a valid command name and, optionally, a comma-separated list of
        /// parameters.</param>
        /// <returns>An instance of InterrogationCommand representing the parsed command and its parameters.</returns>
        /// <exception cref="Exception">Thrown if the command name is unknown, if too many parameters are supplied, or if a required parameter is
        /// missing.</exception>
        internal InterrogationCommand Parse(string commandText)
        {
            int paramStartIndex = commandText.IndexOf(' ');

            string commandName;

            if (paramStartIndex > 0)
            {
                commandName = commandText.Substring(0, paramStartIndex).Trim(); //We have parameters
                commandText = commandText.Substring(paramStartIndex).Trim();
            }
            else
            {
                commandName = commandText.Trim(); //We have no parameters.
                commandText = string.Empty;
            }

            var commandPrototype = Commands.Where(o => o.Name.ToLower() == commandName.ToLower()).FirstOrDefault();
            if (commandPrototype == null)
            {
                throw new Exception($"Unknown command '{commandName}'.");
            }

            var commandParameters = commandText.Split(',', StringSplitOptions.RemoveEmptyEntries);

            //If the supplied parameter count is more than we expect.
            if (commandParameters.Count() > commandPrototype.Parameters.Count)
            {
                throw new Exception($"Too many parameters supplied to '{commandName}'.");
            }

            var parsedCommand = new InterrogationCommand(commandName);

            int paramIndex = 0;

            //Loop though the supplied parameters:
            for (; paramIndex < commandParameters.Length; paramIndex++)
            {
                var paramPrototype = commandPrototype.Parameters[paramIndex];
                parsedCommand.Parameters.Add(new InterrogationCommandParameter(paramPrototype, commandParameters[paramIndex]));
            }

            //Loop through the not-supplied parameters:
            for (; paramIndex < commandPrototype.Parameters.Count; paramIndex++)
            {
                var paramPrototype = commandPrototype.Parameters[paramIndex];
                if (paramPrototype.IsRequired)
                {
                    throw new Exception($"Command '{parsedCommand.Name}' parameter '{paramPrototype.Name}' is not optional.");
                }

                parsedCommand.Parameters.Add(new InterrogationCommandParameter(paramPrototype, null));
            }

            return parsedCommand;
        }
    }
}
