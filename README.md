# C# Math Expression Parser

A compact, easy to use and flexible mathematical expression parser and evaluator written in C#. This library allows you to parse and evaluate complex mathematical expressions at runtime, supporting the basics of mathematical operations, functions, and constants.

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
## Usage

```csharp
using MathExpressions;

// Parse a simple arithmetic expression
var parser = new ExpressionParser();
var expression = parser.Parse("2 + 3 * 4");
ExpressionTools.LogExpression(expression)
// └── Arithmetic_Addition
//     ├── Number: 2
//     └── Arithmetic_Multiplication
//         ├── Number: 3
//         └── Number: 4

// Using mathematical constants
var withConstants = parser.Parse("2 * π");
ExpressionTools.LogExpression(withConstants)
// └── Arithmetic_Multiplication
//     ├── Number: 2
//     └── Constant_Pi

// Using functions
var withFunctions = parser.Parse("sin(π/2)");
ExpressionTools.LogExpression(withFunctions)
// └── Function_Sin
//     └── Arithmetic_Division
//         ├── Constant_Pi
//         └── Number: 2

// Complex expression with multiple operations
var complex = parser.Parse("(log(e) + sin(pi / 2)) max(1, 2 ^ 3) = sqrt(49)");
ExpressionTools.LogExpression(complex)
// └── Comparison_Equal
//     ├── Arithmetic_Multiplication
//     │   ├── Arithmetic_Addition
//     │   │   ├── Function_Log
//     │   │   │   └── Constant_E
//     │   │   └── Function_Sin
//     │   │       └── Arithmetic_Division
//     │   │           ├── Constant_Pi
//     │   │           └── Number: 2
//     │   └── Function_Max
//     │       ├── Number: 1
//     │       └── Arithmetic_Exponent
//     │           ├── Number: 2
//     │           └── Number: 3
//     └── Function_Sqrt
//         └── Number: 49
```

The parser constructs an expression tree that represents the mathematical expression. Each node in the tree is an `Expression` object with a specific type (arithmetic operation, function, constant, etc.) and may have sub-expressions.


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
