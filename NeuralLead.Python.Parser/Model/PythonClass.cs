using NeuralLead.Python.Model;

namespace NeuralLead.Python.Parser.Model
{
    public class PythonClass
    {
        public string Name { get; set; }
        public string[] BaseClass { get; set; }
        public IEnumerable<PythonArg> InitArgs { get; set; }

        int GetArgIndex(string argName)
        {
            for (int i = 0; i < InitArgs.Count(); i++) // se InitArgs è array, oppure .Count per lista
                if (InitArgs.ElementAt(i).Name == argName)
                    return i;
            
            return -1;
        }

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
