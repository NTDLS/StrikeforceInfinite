namespace Ae.Engine.Interrogation
{
    /// <summary>
    /// Represents the definition of a parameter for an interrogation command, including its name, type, requirement
    /// status, and default value.
    /// </summary>
    /// <remarks>Use this class to describe the expected parameters for a debug interrogation command. Each
    /// instance specifies the parameter's characteristics, which can be used for validation or command construction.
    /// The parameter type is defined by the DebugCommandParameterType enumeration.</remarks>
    public class InterrogationCommandParameterPrototype
    {
        /// <summary>
        /// Specifies the supported parameter types for debug commands.
        /// </summary>
        /// <remarks>Use this enumeration to indicate the expected data type for a command parameter when
        /// defining or processing debug commands. The values correspond to common parameter types such as string,
        /// numeric, boolean, and criterion, allowing for type-safe handling of command input.</remarks>
        public enum DebugCommandParameterType
        {
            /// <summary>
            /// String.
            /// </summary>
            String,
            /// <summary>
            /// Numeric (int, float, etc.)
            /// </summary>
            Numeric,
            /// <summary>
            /// Boolean.
            /// </summary>
            Boolean,
            /// <summary>
            /// Represents a condition or rule used to evaluate or filter data within a query or operation.
            /// </summary>
            Criterion
        }

        /// <summary>
        /// Gets the name associated with this instance.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of parameter used by the debug command.
        /// </summary>
        public DebugCommandParameterType CommandParameterType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the associated item is required.
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// Gets the default value associated with the property, if one is specified.
        /// </summary>
        public string? DefaultValue { get; private set; }

        /// <summary>
        /// Initializes a new instance of the InterrogationCommandParameterPrototype class with the specified parameter
        /// name, requirement status, default value, and parameter type.
        /// </summary>
        /// <param name="name">The name of the command parameter. Cannot be null.</param>
        /// <param name="isRequired">A value indicating whether the parameter is required for the command.</param>
        /// <param name="defaultValue">The default value assigned to the parameter if none is provided. Can be null if no default is specified.</param>
        /// <param name="commandParameterType">The type of the command parameter, specifying how the parameter is interpreted.</param>
        public InterrogationCommandParameterPrototype(string name, bool isRequired, string? defaultValue, DebugCommandParameterType commandParameterType)
        {
            Name = name;
            IsRequired = isRequired;
            DefaultValue = defaultValue;
            CommandParameterType = commandParameterType;
        }
    }
}
