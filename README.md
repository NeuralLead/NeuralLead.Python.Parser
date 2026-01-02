# NeuralLead.Python.Parser

A lightweight and efficient .NET library for parsing Python source code and extracting metadata about classes, functions, and global variables using regular expressions.

## Features

- **Extract First-Level Functions**: Parse top-level function definitions with their parameters and type hints
- **Extract First-Level Classes**: Parse class definitions including base classes and `__init__` method parameters
- **Extract Global Variables**: Parse global variable declarations with optional type hints
- **Type Hints Support**: Handles Python type annotations for arguments and variables
- **Special Arguments**: Supports `*args` and `**kwargs` in function and method signatures
- **No External Dependencies**: Uses only .NET's built-in regular expressions, no Python runtime required

## Installation

### NuGet Package (Coming Soon)
```bash
dotnet add package NeuralLead.Python.Parser
```

### Build from Source
```bash
git clone https://github.com/NeuralLead/NeuralLead.Python.Parser.git
cd NeuralLead.Python.Parser
dotnet build
```

## Requirements

- .NET 8.0 or higher
- C# 12.0

## Usage

### Basic Example

```csharp
using NeuralLead.Python.Parser;

// Read Python source code
string pythonCode = File.ReadAllText("example.py");

// Extract functions
var functions = Parser.GetFirstLevelFunctions(pythonCode);
foreach (var func in functions)
{
    Console.WriteLine($"Function: {func.Name}");
    foreach (var arg in func.Args)
    {
        Console.WriteLine($"  - {arg.Name}: {arg._type ?? "no type"}");
    }
}

// Extract classes
var classes = Parser.GetFirstLevelClasses(pythonCode);
foreach (var cls in classes)
{
    Console.WriteLine($"Class: {cls.Name}");
    Console.WriteLine($"  Base Classes: {string.Join(", ", cls.BaseClass)}");
    foreach (var arg in cls.InitArgs)
    {
        Console.WriteLine($"  Init Arg: {arg.Name}: {arg._type ?? "no type"}");
    }
}

// Extract global variables
var globalVars = Parser.GetGlobalVars(pythonCode);
foreach (var gvar in globalVars)
{
    Console.WriteLine($"Global: {gvar.Name} = {gvar.Value}");
    if (gvar.Type != null)
        Console.WriteLine($"  Type: {gvar.Type}");
}
```

### Example Python File

```python
# Global variables
GlobalVar1 = None
GlobalVar2: str = 'hello'

# Top-level function
def process_data(data: list[str], verbose: bool = False, **kwargs):
    pass

# Class with inheritance and typed __init__
class DataProcessor(BaseProcessor, LoggerMixin):
    def __init__(self, config: str, timeout: float = 30.0):
        pass
```

### Output

```
Function: process_data
  - data: list[str]
  - verbose: bool
  - **kwargs: no type

Class: DataProcessor
  Base Classes: BaseProcessor, LoggerMixin
  Init Arg: config: str
  Init Arg: timeout: float

Global: GlobalVar1 = None
Global: GlobalVar2 = 'hello'
  Type: str
```

## API Reference

### Parser Class

#### Static Methods

##### `GetFirstLevelFunctions(string code)`
Extracts all top-level function definitions from Python code.

**Parameters:**
- `code` (string): Python source code

**Returns:** `IEnumerable<PythonFunction>` - Collection of parsed functions

---

##### `GetFirstLevelClasses(string code)`
Extracts all top-level class definitions from Python code, including their base classes and `__init__` parameters.

**Parameters:**
- `code` (string): Python source code

**Returns:** `IEnumerable<PythonClass>` - Collection of parsed classes

---

##### `GetGlobalVars(string code)`
Extracts all global variable assignments from Python code.

**Parameters:**
- `code` (string): Python source code

**Returns:** `IEnumerable<PythonGlobalVar>` - Collection of parsed global variables

---

##### `GetFunctionByName(string functionName, IEnumerable<PythonFunction> functions)`
Finds a function by its name from a collection of functions.

**Parameters:**
- `functionName` (string): Name of the function to find
- `functions` (IEnumerable<PythonFunction>): Collection of functions to search

**Returns:** `PythonFunction?` - The matching function or null

---

##### `GetClassByName(string className, IEnumerable<PythonClass> classes)`
Finds a class by its name from a collection of classes.

**Parameters:**
- `className` (string): Name of the class to find
- `classes` (IEnumerable<PythonClass>): Collection of classes to search

**Returns:** `PythonClass?` - The matching class or null

---

##### `GetClassWithBaseClassName(string baseClassName, IEnumerable<PythonClass> classes)`
Finds all classes that inherit from a specific base class.

**Parameters:**
- `baseClassName` (string): Name of the base class
- `classes` (IEnumerable<PythonClass>): Collection of classes to search

**Returns:** `IEnumerable<PythonClass>` - All classes inheriting from the base class

---

##### `ParsePythonArgs(string args)`
Parses a string of Python function/method arguments.

**Parameters:**
- `args` (string): Comma-separated argument string

**Returns:** `PythonArg[]` - Array of parsed arguments

---

### Model Classes

#### `PythonFunction`
Represents a Python function definition.

**Properties:**
- `Name` (string): Function name
- `Args` (IEnumerable<PythonArg>): Function arguments

---

#### `PythonClass`
Represents a Python class definition.

**Properties:**
- `Name` (string): Class name
- `BaseClass` (string[]): Array of base class names
- `InitArgs` (IEnumerable<PythonArg>): Arguments of the `__init__` method

---

#### `PythonArg`
Represents a Python function/method argument.

**Properties:**
- `Name` (string): Argument name (includes `*` or `**` for special args)
- `_type` (string?): Optional type annotation

---

#### `PythonGlobalVar`
Represents a Python global variable.

**Properties:**
- `Name` (string): Variable name
- `Type` (string?): Optional type annotation
- `Value` (string): Variable value/assignment

---

## Limitations

- **Top-Level Only**: Only parses first-level (non-nested) functions and classes
- **Regex-Based**: Uses regular expressions instead of AST parsing, so some complex edge cases may not be handled
- **No Multiline Support**: Default values containing line breaks may not parse correctly
- **Limited Nested Function Detection**: Nested functions within top-level functions are not extracted
- **Type Annotation Parsing**: Complex generic types may have limited support

## Use Cases

- **Code Analysis**: Analyze Python codebases without requiring Python runtime
- **Documentation Generation**: Extract metadata for automatic documentation
- **Code Generation**: Generate wrapper code or bindings for Python libraries
- **Static Analysis**: Build lightweight analysis tools for Python projects
- **IDE Features**: Implement basic code intelligence features
- **Migration Tools**: Help in porting Python code to other languages

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is open source. Please check the LICENSE file for more information.

## Project Structure

```
NeuralLead.Python.Parser/
- NeuralLead.Python.Parser/
   - Parser.cs                 # Main parser class
   - Model/
       - PythonFunction.cs     # Function model
       - PythonClass.cs        # Class model
       - PythonArg.cs          # Argument model
       - PythonGlobalVar.cs    # Global variable model
- Test/
    - Program.cs                # Example usage
    - tuofile.py                # Sample Python file
```

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/NeuralLead/NeuralLead.Python.Parser).

## Authors

- **NeuralLead** - [GitHub Profile](https://github.com/NeuralLead)

---

**Note**: This library is designed for lightweight parsing and metadata extraction. For comprehensive Python code analysis, consider using full Python AST parsers.