namespace NeuralLead.Python.Model
{
    /// <summary>
    /// Represents a Python function or method argument with optional type annotation.
    /// </summary>
    public class PythonArg
    {
        /// <summary>
        /// Gets or sets the name of the argument.
        /// For special arguments, includes the star prefix (*args, **kwargs).
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the optional type annotation for the argument.
        /// Null if no type hint is specified in the Python code.
        /// </summary>
        public string? _type { get; set; }

        /// <summary>
        /// Returns a string representation of the argument in Python syntax.
        /// Format: "name" or "name: type" if type annotation is present.
        /// </summary>
        /// <returns>A string representing the argument with or without type annotation.</returns>
        public override string ToString()
        {
            if (_type is null)
                return $"{Name}";

            return $"{Name}: {_type}";
        }
    }
}
