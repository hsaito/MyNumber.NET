# Copilot Instructions for MyNumber.NET

## Project Overview
MyNumber.NET is a .NET 8.0 library for validating and generating Japanese My Number (Social Security/Tax ID). The solution contains four projects:
- **MyNumberNET**: Core validation/generation library
- **MyNumberNET_ApiServer**: ASP.NET Core REST API wrapper
- **MyNumberNET_CLI**: Command-line interface with NLog logging
- **MyNumberNET_Test**: xUnit unit tests

## Architecture & Key Patterns

### Core Validation Logic
The `MyNumber` static class in [MyNumberNET/MyNumber.cs](MyNumberNET/MyNumber.cs) implements the My Number algorithm:
- **VerifyNumber()**: Validates 12-digit arrays using check digit algorithm (positions 1-6 use weight 2-7, positions 7-11 use weight 1-6)
- **CalculateCheckDigits()**: Computes 12th digit from first 11 (modulo 11 formula: if sum%11 ≤ 1, checkdigit=0; else checkdigit=11-sum%11)
- Throws `MyNumberMalformedException` for invalid input (null, wrong length, non-digits)

### MyNumberValue - Type-Safe Wrapper
`MyNumberValue` readonly struct (in [MyNumberNET/MyNumber.cs](MyNumberNET/MyNumber.cs)) wraps validated numbers:
- Immutable and always valid on construction
- Multiple constructors: from int[], from 12 individual digits, or `FromFirstElevenDigits()` (auto-calculates check digit)
- `TryParse(string)` handles separator formats (spaces, hyphens, underscores)
- `ToString(format)` supports: "N" (plain), "S" (spaced), "H" (hyphenated), "G" (grouped)

### API Layer Pattern
[MyNumberNET_ApiServer/Controllers/MyNumberController.cs](MyNumberNET_ApiServer/Controllers/MyNumberController.cs) duplicates input validation before calling business logic - this is intentional to prevent malformed requests from reaching the library.

## Critical Workflows

### Building & Testing
```powershell
dotnet build                              # Build solution
dotnet test MyNumberNET_Test              # Run tests
dotnet run --project MyNumberNET_ApiServer # Start API on port 5000+
dotnet run --project MyNumberNET_CLI -- generate 10  # CLI with args
```

### CLI Commands
Located in [MyNumberNET_CLI/Program.cs](MyNumberNET_CLI/Program.cs) switch statement:
- `generate [count]` - Random valid My Numbers
- `check [number]` - Validate single number
- `complete [11digits]` - Calculate check digit
- `rangen [min] [max]` - Numerical range generation
- `ranges [min] [max]` - Sequential range generation

### Version Management
All versions defined in [Directory.Build.props](Directory.Build.props):
- Update `MyNumberNETVersion`, `MyNumberNETCliVersion`, `MyNumberNETTestVersion` for releases
- Target framework: .NET 8.0 (specified in [global.json](global.json))

## Integration Points & Dependencies

### Exception Hierarchy
- Base: `MyNumber.MyNumberException(message)`
- Specific: `MyNumber.MyNumberMalformedException(message)` - thrown on validation failure
- API returns `BadRequest` + error message on exceptions; never propagates exceptions

### NLog Configuration
CLI project uses [MyNumberNET_CLI/nlog.config](MyNumberNET_CLI/nlog.config) for logging. Logger accessed via `LogManager.GetCurrentClassLogger()`.

### No External Dependencies
Core library only uses System.* namespaces - no NuGet dependencies. Keep it minimal.

## Code Patterns

### Error Handling
1. Validate inputs early (null, length, digit range)
2. Throw `MyNumberMalformedException` with descriptive messages
3. In API: catch exceptions and return `BadRequest(ex.Message)`

### Digit Array Conventions
- 12-digit arrays: indices 0-10 are data, 11 is check digit
- Use `Truncate()` private method to extract first 11 digits
- Always clone arrays when storing (immutability)

### Testing Strategy
Test files: [MyNumberNET_Test/MyNumberValueTests.cs](MyNumberNET_Test/MyNumberValueTests.cs), [MyNumberNET_Test/MyNumberControllerTests.cs](MyNumberNET_Test/MyNumberControllerTests.cs)
- Test both valid and malformed inputs
- Verify exception messages and types
- Test all format options for ToString()## Containerization & Deployment

### Docker
[MyNumberNET_ApiServer/Dockerfile](MyNumberNET_ApiServer/Dockerfile) uses multi-stage build:
1. **Build stage**: Restores, builds, and publishes using .NET 8.0 SDK
2. **Runtime stage**: Runs on .NET 8.0 ASP.NET Core runtime (smaller image)
3. **Port**: Exposes 5000 (configurable via `ASPNETCORE_URLS` env var)
4. **Health check**: Built-in liveness probe every 30 seconds

Local Docker usage:
```bash
docker build -f MyNumberNET_ApiServer/Dockerfile -t mynumberapi:latest .
docker run -p 5000:5000 mynumberapi:latest
```

### GitHub Container Registry
Images pushed to `ghcr.io/hsaito/mynumberapi` on main branch pushes and tags (v* releases).
Tags: branch name, semantic version (v1.0.0), git SHA.

## CI/CD

### GitHub Actions Workflows
- **[.github/workflows/dotnet-ci.yml](.github/workflows/dotnet-ci.yml)**: Cross-platform build/test (Windows, Linux, macOS)
- **[.github/workflows/docker-build.yml](.github/workflows/docker-build.yml)**: Docker image build and push to ghcr.io
  - Automatically tags releases (semantic versioning supported)
  - PR builds test image without pushing
  - Uses Buildx for efficient multi-stage builds

### Legacy CI
[Jenkinsfile](Jenkinsfile) provides additional build/test/package orchestration. Deterministic builds enabled via [Directory.Build.props](Directory.Build.props) for reproducible artifacts.
