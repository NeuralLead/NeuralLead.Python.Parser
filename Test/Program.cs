namespace NeuralLead.Python.Parser
{
    public static class Program
    {
        public static void Main(string[] argv)
        {
            // Leggi tutto il file Python in una stringa
            string filePath = "tuofile.py";
            string code = File.ReadAllText(filePath);

            var functionNames = Parser.GetFirstLevelFunctions(code);
            var classNames = Parser.GetFirstLevelClasses(code);
            var gVarsNames = Parser.GetGlobalVars(code);

            // Adesso functionNames contiene tutti i nomi di funzione di primo livello

            Console.WriteLine("e" + classNames.First());
        }
    }
}