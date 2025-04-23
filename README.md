# C# Math Expression Parser

A powerful and flexible mathematical expression parser and evaluator written in C#. This library allows you to parse and evaluate complex mathematical expressions at runtime, supporting a wide range of mathematical operations, functions, and constants.

## Features

### Operators
- Basic arithmetic: `+`, `-`, `*`, `/`, `^` (power)
- Unary operations: `-` (negation)
- Parentheses for grouping expressions

### Mathematical Constants
- π (pi)
- e (Euler's number)

### Mathematical Functions
- Trigonometric: `sin`, `cos`, `tan`
- Inverse trigonometric: `asin`, `acos`, `atan`
- Exponential and logarithmic: `exp`, `log`, `ln`
- Power functions: `sqrt`, `pow`
- Other: `abs`, `min`, `max`

## Usage

```csharp
using MathExpressions;

// Basic arithmetic
var parser = new ExpressionParser();
var expression = parser.Parse("2 + 3 * 4");
double result = expression.Evaluate(); // Returns 14

// Using constants
var withConstants = parser.Parse("2 * π");
double circleResult = withConstants.Evaluate(); // Returns 6.28318...

// Using functions
var withFunctions = parser.Parse("sin(π/2)");
double sinResult = withFunctions.Evaluate(); // Returns 1.0

// Complex expressions
var complex = parser.Parse("sqrt(pow(2, 2) + pow(3, 2))");
double pythagoras = complex.Evaluate(); // Returns 3.60555...
```

## Installation

Currently, you can include this library in your project by:
1. Cloning this repository
2. Adding the source files to your project
3. (Future: NuGet package installation instructions)

## Requirements

- .NET Standard 2.0 or higher
- NUnit (for running tests)

## Running Tests

The project includes a comprehensive suite of unit tests demonstrating all supported operations. To run them:

1. Open the solution in Visual Studio
2. Build the test project
3. Run the tests using your preferred test runner

## Contributing

Contributions are welcome! Feel free to:
- Report bugs
- Suggest new features or functions
- Submit pull requests
- Improve documentation

## License

MIT

## Author

Miika Pirtola
