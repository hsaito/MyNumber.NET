# My Number Library for .NET

This repository provides tools for validating and generating Japanese My Number (Social Security and Tax Number System) in .NET 8.0.

## Project Structure

- **MyNumberNET/**: Core .NET 8.0 library for My Number validation and generation.
- **MyNumberNET_CLI/**: Command-line interface for interacting with the library. Uses NLog for logging.
- **MyNumberNET_Test/**: Unit tests for the library.
- **Dockerfile**: Containerization support for CLI or library usage.
- **Jenkinsfile**: CI/CD pipeline configuration.

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

## Docker Support

To build and run the CLI in a container:
```
docker build -t mynumbernet-cli .
docker run --rm mynumbernet-cli [command] [arguments]
```

## CI/CD

Automated builds and tests are configured via Jenkinsfile.

## Requirements
- .NET 8.0 SDK
- (Optional) Docker for containerization

## License
See LICENSE for details.
