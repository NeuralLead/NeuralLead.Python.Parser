namespace NeuralLead.Python.Parser.Model
{
    /// <summary>
    /// Represents a Python global variable with its name, optional type annotation, and assigned value.
    /// </summary>
    public class PythonGlobalVar
    {
        /// <summary>
        /// Gets or sets the name of the global variable.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the optional type annotation for the variable.
        /// Null if no type hint is specified in the Python code.
        /// </summary>
        public string? Type { get; set; }
        
        /// <summary>
        /// Gets or sets the value assigned to the variable as a string.
        /// Contains the right-hand side of the assignment expression.
        /// </summary>
        public string Value { get; set; }
    }
}
