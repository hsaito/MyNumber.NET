# My Number Library for .NET

This repository provides tools for validating and generating Japanese My Number (Social Security and Tax Number System) in .NET 8.0.

## Project Structure

- **MyNumberNET/**: Core .NET 8.0 library for My Number validation and generation.
- **MyNumberNET_ApiServer/**: ASP.NET Core Web API for My Number validation and generation.
- **MyNumberNET_CLI/**: Command-line interface for interacting with the library. Uses NLog for logging.
- **MyNumberNET_Test/**: Unit tests for the library and API server.
- **Jenkinsfile**: CI/CD pipeline configuration.
- **global.json**: Specifies the .NET SDK version.
- **MyNumberNET.sln**: Solution file for managing all projects.

## Projects

### MyNumberNET
A .NET 8.0 library for validating and generating My Number sequences.

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

### MyNumberNET_ApiServer
ASP.NET Core Web API for validating and generating My Numbers.

**Usage:**
```
dotnet run --project MyNumberNET_ApiServer
```
The API exposes endpoints for validation and generation. See `Controllers/MyNumberController.cs` for details.

### MyNumberNET_CLI
A command-line interface for validating and generating My Numbers. Uses NLog for logging.

**Usage:**
```
dotnet run --project MyNumberNET_CLI [command] [arguments]
```
Or run the built executable directly:
```
MyNumberNET_CLI\bin\Debug\net8.0\MyNumberNET_CLI.exe [command] [arguments]
```
**Commands:**
- `generate [count]` : Generate valid My Numbers
- `check [My Number]` : Validate a given number
- `complete [first 11 digits]` : Complete a number by calculating the check digit
- `rangen [min] [max]` : Generate numbers in a numerical range
- `ranges [min] [max]` : Generate numbers in a sequential range

### MyNumberNET_Test
Unit tests for the library and API server.

**Run tests:**
```
dotnet test MyNumberNET_Test
```

## Solution Management

Use the solution file to build and manage all projects:
```
dotnet build MyNumberNET.sln
```

## .NET Version

This repository uses .NET 8.0. The required SDK version is specified in `global.json`.
