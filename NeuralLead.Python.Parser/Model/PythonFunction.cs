using NeuralLead.Python.Model;

namespace NeuralLead.Python.Parser.Model
{
    /// <summary>
    /// Represents a Python function definition with its name and arguments.
    /// </summary>
    public class PythonFunction
    {
        /// <summary>
        /// Gets or sets the name of the function.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of arguments for this function.
        /// Includes regular parameters, type hints, default values, *args, and **kwargs.
        /// </summary>
        public IEnumerable<PythonArg> Args { get; set; }

        /// <summary>
        /// Returns a string representation of the function in Python syntax.
        /// Format: def name(arg1, arg2, ...):
        /// </summary>
        /// <returns>A string representing the function signature.</returns>
        public override string ToString()
        {
            return $"def {Name}({string.Join(",", Args)}):";
        }
    }
}
