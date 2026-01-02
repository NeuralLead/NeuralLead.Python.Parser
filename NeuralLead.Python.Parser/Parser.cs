using NeuralLead.Python.Model;
using NeuralLead.Python.Parser.Model;
using System.Text.RegularExpressions;

namespace NeuralLead.Python.Parser
{
    /// <summary>
    /// Provides static methods for parsing Python source code and extracting metadata
    /// about functions, classes, and global variables using regular expressions.
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Regular expression to match top-level function definitions.
        /// Pattern: ^def <name>(<parameters>):
        /// </summary>
        static Regex regexFunction = new Regex(
            @"^def\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*\(([^)]*)\)\s*:",
            RegexOptions.Multiline
        );

        /// <summary>
        /// Regular expression to match top-level class definitions with their body.
        /// Captures class name, base classes (if any), and the class body.
        /// Pattern: ^class <name>(<base_classes>):\n<indented_body>
        /// </summary>
        static Regex regexClass = new Regex(
            @"^class\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*(?:\(([^)]*)\))?\s*:\s*(?:#.*)?\n((?:^[ \t]+.*\n?)*)",
            RegexOptions.Multiline
        );

        /// <summary>
        /// Regular expression to match __init__ method within a class body.
        /// Pattern: <indent>def __init__(<parameters>):
        /// </summary>
        static Regex regexInit = new Regex(
            @"^[ \t]+def\s+__init__\s*\(([^)]*)\)\s*:",
            RegexOptions.Multiline
        );

        /// <summary>
        /// Regular expression to match global variable assignments.
        /// Pattern: ^<name> [: <type>] = <value>
        /// </summary>
        static Regex regexGlobalVar = new Regex(
            @"^([a-zA-Z_][a-zA-Z0-9_]*)\s*(?::\s*([a-zA-Z0-9_\[\], \.]+))?\s*=\s*(.+)$",
            RegexOptions.Multiline
        );

        /// <summary>
        /// Finds a class by its name from a collection of classes.
        /// </summary>
        /// <param name="className">The name of the class to find.</param>
        /// <param name="classes">The collection of classes to search.</param>
        /// <returns>The matching PythonClass object, or null if not found.</returns>
        public static PythonClass? GetClassByName(string className, IEnumerable<PythonClass> classes)
        {
            return classes.FirstOrDefault(_class => _class.Name == className);
        }

        /// <summary>
        /// Finds all classes that inherit from a specific base class.
        /// </summary>
        /// <param name="baseClassName">The name of the base class to search for.</param>
        /// <param name="classes">The collection of classes to search.</param>
        /// <returns>An enumerable collection of classes that inherit from the specified base class.</returns>
        public static IEnumerable<PythonClass> GetClassWithBaseClassName(string baseClassName, IEnumerable<PythonClass> classes)
        {
            return classes.Where(_class => _class.BaseClass.Any(x=>x == baseClassName));
        }

        /// <summary>
        /// Finds a function by its name from a collection of functions.
        /// </summary>
        /// <param name="functionName">The name of the function to find.</param>
        /// <param name="functions">The collection of functions to search.</param>
        /// <returns>The matching PythonFunction object, or null if not found.</returns>
        public static PythonFunction? GetFunctionByName(string functionName, IEnumerable<PythonFunction> functions)
        {
            return functions.FirstOrDefault(_function => _function.Name == functionName);
        }

        /// <summary>
        /// Extracts all top-level (first-level) function definitions from Python source code.
        /// Only functions defined at module level are captured, nested functions are ignored.
        /// </summary>
        /// <param name="code">The Python source code to parse.</param>
        /// <returns>An enumerable collection of PythonFunction objects representing the parsed functions.</returns>
        public static IEnumerable<PythonFunction> GetFirstLevelFunctions(string code)
        {
            return regexFunction.Matches(code)
                .Cast<Match>()
                .Select(m => new PythonFunction
                {
                    Name = m.Groups[1].Value,
                    Args = ParsePythonArgs(m.Groups[2].Value)
                });
        }

        /// <summary>
        /// Extracts all top-level (first-level) class definitions from Python source code.
        /// Captures class name, base classes, and __init__ method parameters if present.
        /// </summary>
        /// <param name="code">The Python source code to parse.</param>
        /// <returns>An enumerable collection of PythonClass objects representing the parsed classes.</returns>
        public static IEnumerable<PythonClass> GetFirstLevelClasses(string code)
        {
            var result = new List<PythonClass>();

            foreach (Match m in regexClass.Matches(code))
            {
                // Extract class name
                string className = m.Groups[1].Value;
                
                // Extract base classes (if any)
                string[] baseClass = m.Groups[2].Success
                    ? m.Groups[2].Value
                        .Split(',')
                        .Select(y => y.Trim())
                        .Where(y => !string.IsNullOrEmpty(y))
                        .ToArray()
                    : Array.Empty<string>();
                
                // Extract class body
                string classBody = m.Groups[3].Value;

                // Initialize empty args array
                var initArgs = Array.Empty<PythonArg>();

                // Look for __init__ method in class body
                var initMatch = regexInit.Match(classBody);
                if (initMatch.Success)
                    initArgs = ParsePythonArgs(initMatch.Groups[1].Value);

                result.Add(new PythonClass
                {
                    Name = className,
                    BaseClass = baseClass,
                    InitArgs = initArgs
                });
            }

            return result.ToArray();
        }

        /// <summary>
        /// Parses a string of Python function/method arguments into structured argument objects.
        /// Handles regular arguments, type hints, default values, *args, and **kwargs.
        /// </summary>
        /// <param name="args">Comma-separated string of Python arguments.</param>
        /// <returns>An array of PythonArg objects representing the parsed arguments.</returns>
        public static PythonArg[] ParsePythonArgs(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
                return Array.Empty<PythonArg>();

            var result = new List<PythonArg>();

            // Split on commas (note: doesn't handle commas in default values or multiline args - rare cases)
            foreach (var arg in args.Split(','))
            {
                var trimmed = arg.Trim();
                if (string.IsNullOrEmpty(trimmed))
                    continue;

                // Regex pattern: [*/**/empty]<name> [: <type>] [= <default>]
                // Examples: a, b: int, c=3, d: float = 2.0, *args, **kwargs
                var match = Regex.Match(trimmed, @"^(?<star>\*{0,2})\s*(?<name>[a-zA-Z_][a-zA-Z0-9_]*)\s*(?::\s*(?<type>[^=]+?))?\s*(?:=\s*.+)?$");
                if (!match.Success)
                {
                    // Fallback: treat entire string as argument name
                    result.Add(new PythonArg { Name = trimmed });
                    continue;
                }

                // Extract matched groups
                string star = match.Groups["star"].Value;
                string name = match.Groups["name"].Value;
                string? type = match.Groups["type"].Success ? match.Groups["type"].Value.Trim() : null;

                // Include *args, **kwargs stars in the name
                if (!string.IsNullOrEmpty(star))
                    name = star + name;

                result.Add(new PythonArg
                {
                    Name = name,
                    _type = type
                });
            }
            return result.ToArray();
        }

        /// <summary>
        /// Extracts all global variable assignments from Python source code.
        /// Filters out variables inside functions, classes, and decorated statements.
        /// Only captures top-level, non-indented variable assignments.
        /// </summary>
        /// <param name="code">The Python source code to parse.</param>
        /// <returns>An enumerable collection of PythonGlobalVar objects representing the parsed global variables.</returns>
        public static IEnumerable<PythonGlobalVar> GetGlobalVars(string code)
        {
            // Exclude lines that start with "def", "class", "@" (decorators), 
            // indented lines (inside functions/classes), comments, and pure whitespace
            var lines = code.Split('\n');
            var filtered = string.Join("\n",
                lines.Where(line =>
                    !line.TrimStart().StartsWith("def") &&
                    !line.TrimStart().StartsWith("class") &&
                    !line.TrimStart().StartsWith("@") &&
                    !string.IsNullOrWhiteSpace(line) &&
                    !char.IsWhiteSpace(line, 0) // Must not be indented
                )
            );

            foreach (Match m in regexGlobalVar.Matches(filtered))
            {
                yield return new PythonGlobalVar
                {
                    Name = m.Groups[1].Value,
                    Type = m.Groups[2].Success ? m.Groups[2].Value : null,
                    Value = m.Groups[3].Value.Trim()
                };
            }
        }
    }
}
