# My Number Library for .NET

This repository provides tools for validating and generating Japanese My Number (Social Security and Tax Number System) in .NET.

## Projects

### MyNumberNET
A .NET library for validating and generating My Number sequences.

**Features:**
- Validate a 12-digit My Number: `MyNumber.VerifyNumber(int[] number)`
- Calculate check digit for first 11 digits: `MyNumber.CalculateCheckDigits(int[] number)`
- Exception handling for malformed input

**Example Usage:**
```csharp
using MyNumberNET;
int[] number = {6,1,4,1,0,6,5,2,6,0,0,0};
bool isValid = MyNumber.VerifyNumber(number);
```

### MyNumberNET_CLI
A command-line interface for validating and generating My Numbers.

**Usage:**
```
dotnet run --project MyNumberNET_CLI [command] [arguments]
```
**Commands:**
- `generate [count]` : Generate valid My Numbers
- `check [My Number]` : Validate a given number
- `complete [first 11 digits]` : Complete a number by calculating the check digit
- `rangen [min] [max]` : Generate numbers in a numerical range
- `ranges [min] [max]` : Generate numbers in a sequential range

### MyNumberNET_Test
Unit tests for the library.

**To run tests:**
```
dotnet test MyNumberNET_Test
```

## Build Instructions

1. Clone the repository.
2. Build the solution:
   ```
   dotnet build MyNumberNET.sln
   ```
3. Run CLI or tests as shown above.

## License
See LICENSE for details.
