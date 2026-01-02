using NeuralLead.Python.Model;

namespace NeuralLead.Python.Parser.Model
{
    public class PythonFunction
    {
        public string Name { get; set; }
        public IEnumerable<PythonArg> Args { get; set; }

        public override string ToString()
        {
            return $"def {Name}({string.Join(",", Args)}):";
        }
    }
}
