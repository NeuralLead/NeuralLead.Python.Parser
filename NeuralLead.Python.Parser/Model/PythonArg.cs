namespace NeuralLead.Python.Model
{
    public class PythonArg
    {
        public string Name { get; set; }
        public string? _type { get; set; }

        public override string ToString()
        {
            if (_type is null)
                return $"{Name}";

            return $"{Name}: {_type}";
        }
    }
}
