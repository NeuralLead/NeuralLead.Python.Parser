using NeuralLead.Python.Model;
using NeuralLead.Python.Parser.Model;
using System.Text.RegularExpressions;

namespace NeuralLead.Python.Parser
{
    public class Parser
    {
        // ^def <nome> ( <parametri> ):
        static Regex regexFunction = new Regex(
            @"^def\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*\(([^)]*)\)\s*:",
            RegexOptions.Multiline
        );

        // Classi di primo livello (con corpo)
        static Regex regexClass = new Regex(
            @"^class\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*(?:\(([^)]*)\))?\s*:\s*(?:#.*)?\n((?:^[ \t]+.*\n?)*)",
            RegexOptions.Multiline
        );

        // __init__ all'interno del corpo della classe
        static Regex regexInit = new Regex(
            @"^[ \t]+def\s+__init__\s*\(([^)]*)\)\s*:",
            RegexOptions.Multiline
        );

        // ^<nome> [ : <tipo> ] = <valore>
        static Regex regexGlobalVar = new Regex(
            @"^([a-zA-Z_][a-zA-Z0-9_]*)\s*(?::\s*([a-zA-Z0-9_\[\], \.]+))?\s*=\s*(.+)$",
            RegexOptions.Multiline
        );

        public static PythonClass? GetClassByName(string className, IEnumerable<PythonClass> classes)
        {
            return classes.FirstOrDefault(_class => _class.Name == className);
        }

        public static IEnumerable<PythonClass> GetClassWithBaseClassName(string baseClassName, IEnumerable<PythonClass> classes)
        {
            return classes.Where(_class => _class.BaseClass.Any(x=>x == baseClassName));
        }

        public static PythonFunction? GetFunctionByName(string functionName, IEnumerable<PythonFunction> functions)
        {
            return functions.FirstOrDefault(_function => _function.Name == functionName);
        }

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

        public static IEnumerable<PythonClass> GetFirstLevelClasses(string code)
        {
            var result = new List<PythonClass>();

            foreach (Match m in regexClass.Matches(code))
            {
                string className = m.Groups[1].Value;
                string[] baseClass = m.Groups[2].Success
                    ? m.Groups[2].Value
                        .Split(',')
                        .Select(y => y.Trim())
                        .Where(y => !string.IsNullOrEmpty(y))
                        .ToArray()
                    : Array.Empty<string>();
                string classBody = m.Groups[3].Value;

                var initArgs = Array.Empty<PythonArg>();

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

        public static PythonArg[] ParsePythonArgs(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
                return Array.Empty<PythonArg>();

            var result = new List<PythonArg>();

            // Split sugli argomenti, tenendo conto che possono esserci spazi
            // Non gestisce argomenti multilinea o con virgole nelle default (caso raro)
            foreach (var arg in args.Split(','))
            {
                var trimmed = arg.Trim();
                if (string.IsNullOrEmpty(trimmed))
                    continue;

                // Regex: nome [ : tipo ] [ = default ]
                // Esempi: a, b: int, c=3, d: float = 2.0, *args, **kwargs
                var match = Regex.Match(trimmed, @"^(?<star>\*{0,2})\s*(?<name>[a-zA-Z_][a-zA-Z0-9_]*)\s*(?::\s*(?<type>[^=]+?))?\s*(?:=\s*.+)?$");
                if (!match.Success)
                {
                    // fallback: solo nome
                    result.Add(new PythonArg { Name = trimmed });
                    continue;
                }

                // match.Success
                string star = match.Groups["star"].Value;
                string name = match.Groups["name"].Value;
                string? type = match.Groups["type"].Success ? match.Groups["type"].Value.Trim() : null;

                // *args, **kwargs
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

        public static IEnumerable<PythonGlobalVar> GetGlobalVars(string code)
        {
            // Escludi righe che iniziano con "def", "class", "@" (decoratori), indentate (cioè dentro funzioni/classi)
            // e commenti/pure whitespace
            var lines = code.Split('\n');
            var filtered = string.Join("\n",
                lines.Where(line =>
                    !line.TrimStart().StartsWith("def") &&
                    !line.TrimStart().StartsWith("class") &&
                    !line.TrimStart().StartsWith("@") &&
                    !string.IsNullOrWhiteSpace(line) &&
                    !char.IsWhiteSpace(line, 0) // non indentata
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
