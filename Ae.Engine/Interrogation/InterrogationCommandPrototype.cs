using System.Collections.Generic;

namespace Ae.Engine.Interrogation
{
    /// <summary>
    /// Represents the definition of an interrogation command, including its name, description, and parameters.
    /// </summary>
    /// <remarks>Use this class to describe the metadata and structure of a command that can be executed
    /// during an interrogation process. The command's name, description, and parameter prototypes are provided to
    /// support command discovery and validation scenarios.</remarks>
    public class InterrogationCommandPrototype
    {
        /// <summary>
        /// Gets the name value converted to lowercase using the current culture.
        /// </summary>
        public string NameLowered { get; private set; }

        /// <summary>
        /// Gets the name associated with this instance.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the descriptive text associated with the object.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the collection of parameter prototypes associated with the interrogation command.
        /// </summary>
        /// <remarks>The collection is read-only and is initialized to an empty list. Use this property to
        /// access the parameters required or supported by the command.</remarks>
        public List<InterrogationCommandParameterPrototype> Parameters { get; private set; } = new();

        /// <summary>
        /// Initializes a new instance of the InterrogationCommandPrototype class with the specified command name and
        /// description.
        /// </summary>
        /// <param name="name">The name of the command. Cannot be null or empty.</param>
        /// <param name="description">The description of the command. Provides additional information about the command's purpose.</param>
        public InterrogationCommandPrototype(string name, string description)
        {
            Name = name;
            NameLowered = name.ToLower();
            Description = description;
        }
    }
}
