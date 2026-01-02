namespace NeuralLead.Python.Parser
{
    /// <summary>
    /// Entry point for the test application demonstrating the NeuralLead.Python.Parser library.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main method that demonstrates how to use the Parser to extract Python code metadata.
        /// </summary>
        /// <param name="argv">Command line arguments (not currently used).</param>
        public static void Main(string[] argv)
        {
            // Read the entire Python file into a string
            string filePath = "tuofile.py";
            string code = File.ReadAllText(filePath);

            // Extract all top-level functions from the Python code
            var functionNames = Parser.GetFirstLevelFunctions(code);
            
            // Extract all top-level classes from the Python code
            var classNames = Parser.GetFirstLevelClasses(code);
            
            // Extract all global variables from the Python code
            var gVarsNames = Parser.GetGlobalVars(code);

            // Now functionNames contains all top-level function names

            // Output the first class name (example usage)
            Console.WriteLine("e" + classNames.First());
        }
    }
}