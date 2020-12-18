# ExpressionParser
A simple lib in C# for parsing and evaluating expressions. To get it install with nuget:
```
Install-Package Soukoku.ExpressionParser
```

## Supports
This lib supports most common operators.

* `+` addition
* `-` subtraction/negative
* `*` multiplication
* `/` division
* `%` modulus
* `&` bitwise and
* `|` bitwise or
* `!` logical negation
* `&&` logical and
* `||` logical or
* `<` less than
* `<=` less than or equals
* `>` greater than
* `>=` greater than or equals
* `==` equals
* `!=` not equals
* `++` preincrement
* `--` predecrement
* basic functions (`pow`, `cos`, `sin`, `tan`)


## Usage
Create an instance of the `Evaluator` and call `EvalueateInfix`.
Expressions can contain simple math or functions.
The result can be retrieved with a `ToString()`. If an expression is not supported
you will get a **NotSupportedException**.

```csharp
var context = new EvaluationContext();
var evaluator = new Evaluator(context);
var result = evaluator.Evaluate("33 + 55");
Console.WriteLine(result.ToString()); // should be "88"

result = evaluator.Evaluate("pow(2,8)");
Console.WriteLine(result.ToString()); // should be "256"
```


## How it works
The evaluator does its work in stages:

1. Parse input string into tokens
2. Arrange tokens into Reverse Polish notation
3. Perform the evaluation

The logical operations will result in a number with 1 being true and 0 being false.

## Custom functions
The lib only comes with some built-in functions. It's possible to add
your own by registering a custom routine.

```csharp
var context = new EvaluationContext();

// This custom function has no parameters and always returns 5.
// The return value of a function is an ExpressToken with string value.
var myFunc = new FunctionRoutine(0, (ctx, parameters) => new ExpressionToken("5"));
context.RegisterFunction("always5", myFunc);

var evaluator = new Evaluator(context);
var result = evaluator.Evaluate("10 + always5()");
Console.WriteLine(result.ToString()); // should be "15"
```
Because the use of functions, floating-point numbers can only use period as the decimal separator 
(i.e. only "2.5" is supported, not "2,5").
