using NeuralLead.Python.Model;

namespace NeuralLead.Python.Parser.Model
{
    /// <summary>
    /// Represents a Python class definition with its name, base classes, and __init__ parameters.
    /// </summary>
    public class PythonClass
    {
        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the array of base class names this class inherits from.
        /// Empty array if the class has no explicit base classes.
        /// </summary>
        public string[] BaseClass { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of arguments for the __init__ method.
        /// Empty collection if no __init__ method is defined.
        /// </summary>
        public IEnumerable<PythonArg> InitArgs { get; set; }

        /// <summary>
        /// Finds the index of an argument by name in the InitArgs collection.
        /// </summary>
        /// <param name="argName">The name of the argument to find.</param>
        /// <returns>The zero-based index of the argument, or -1 if not found.</returns>
        int GetArgIndex(string argName)
        {
            for (int i = 0; i < InitArgs.Count(); i++)
                if (InitArgs.ElementAt(i).Name == argName)
                    return i;
            
            return -1;
        }

        /// <summary>
        /// Returns a string representation of the class in Python syntax.
        /// Includes class name, base classes (if any), and __init__ method signature.
        /// </summary>
        /// <returns>A string representing the class definition.</returns>
        public override string ToString()
        {
            string ina = InitArgs is null ? string.Empty : string.Join(", ", InitArgs);
            string defInit = $"def __init__({ina}):{Environment.NewLine}    pass";

            if(BaseClass is null || BaseClass.Length == 0)
                return $"class {Name}:";
            return $"class {Name}({string.Join(", ", BaseClass)}):{Environment.NewLine}  {defInit}";
        }
    }
}
