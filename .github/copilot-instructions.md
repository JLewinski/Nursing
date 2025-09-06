# Nursing Time Tracking Application

A multi-platform nursing/feeding time tracking application built with .NET 8.0 and TypeScript. The application includes an ASP.NET Core Web API backend, Svelte frontend, and comprehensive test suite. It tracks feeding sessions with left/right breast timers for nursing mothers.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Prerequisites and Environment Setup
- Ensure .NET 8.0 SDK is installed (current version: 8.0.119)
- Ensure Node.js v20.19.4+ and npm 10.8.2+ are installed
- Docker 28.0.4+ is available for containerization
- PostgreSQL database is required for API runtime (not for building/testing)

### Building the .NET Solution
Run these commands in sequence from the repository root:

```bash
cd /home/runner/work/Nursing/Nursing
dotnet workload restore    # Takes ~13 seconds
dotnet restore            # Takes ~24 seconds - installs all NuGet packages
dotnet build --no-restore # Takes ~12 seconds - builds all 3 active projects
```

**NEVER CANCEL: Build commands complete in under 45 seconds total. Set timeout to 120+ seconds.**

### Testing the Application
```bash
dotnet test --no-build    # Takes ~4 seconds - runs 11 xUnit tests in Nursing.API.Tests
```

**NEVER CANCEL: Tests complete in under 10 seconds. Set timeout to 30+ seconds.**

All tests pass and use in-memory Entity Framework for testing without requiring PostgreSQL.

### Running the Blazor WebAssembly Frontend (DEPRECATED)
**Note: The Blazor project is no longer being actively developed.**

```bash
cd Nursing.Blazor
dotnet run --no-build     # Starts on http://localhost:5006
```

The Blazor frontend is still present in the solution but is not being actively maintained or developed.

### Running the Svelte Frontend
```bash
cd Nursing.Svelte
npm install              # Takes ~7 seconds - installs dependencies (shows 7 vulnerabilities - expected)
npm run build           # Takes ~12 seconds - builds production version
npm run dev             # Starts development server on http://localhost:5173
```

**NEVER CANCEL: npm install takes ~10 seconds, build takes ~15 seconds. Set timeout to 60+ seconds.**

### Running the API (Requires PostgreSQL)
The API cannot run without PostgreSQL database connection. To run the API:

1. Set up PostgreSQL database with connection string in `appsettings.json`
2. Update connection string in configuration
3. Run: `cd Nursing.API && dotnet run --no-build`

The API will fail with connection error if PostgreSQL is not available: 
"Failed to connect to 127.0.0.1:5432 - Connection refused"

### Docker Support
Docker build is available but the provided Dockerfile has a syntax error (typo). Fix the Dockerfile before using:
```bash
docker build -f Nursing.API/Dockerfile -t nursing-api .
```

## Project Structure

### Core Projects
- **Nursing.Core** (.NET 8.0 class library) - Shared models and DTOs
- **Nursing.API** (.NET 8.0 Web API) - REST API with PostgreSQL, Entity Framework, JWT authentication
- **Nursing.Blazor** (.NET 8.0 WebAssembly) - **DEPRECATED** - Blazor frontend (no longer actively developed)
- **Nursing.Svelte** (TypeScript/Svelte 5) - Primary frontend with Vite, Bootstrap, Chart.js
- **Nursing.API.Tests** (.NET 8.0 xUnit) - Comprehensive test suite with in-memory database

### Key Features
- Dual timer system for left/right breast feeding tracking
- Session management with start/end times and totals
- User authentication and group-based data sharing
- Offline-first architecture with sync capabilities
- PWA support with service worker
- Mobile-responsive design

## Validation and Testing

### Manual Validation Scenarios
After making changes, always test these core user workflows:

1. **Timer Functionality**: Start left timer, switch to right timer, verify totals update correctly
2. **Session Management**: Create new session, finish session, verify session data persists
3. **Frontend Connectivity**: Verify Svelte frontend starts and renders correctly
4. **API Tests**: Run full test suite to verify business logic integrity

### CI/CD Pipeline
The GitHub Actions workflow (`.github/workflows/buildTest.yml`) requires:
- .NET 9.0 preview SDK
- Workload restoration before build

**Note**: The CI pipeline still includes Java 17 and Android SDK setup for legacy compatibility with the deprecated Blazor project, but these are not required for active development.

Always run these commands before committing:
```bash
dotnet workload restore   # Required for CI compatibility
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

### Code Quality
- Use `dotnet format` for code formatting (.NET projects)
- No specific linting tools configured for Svelte project
- Follow existing code patterns and conventions
- Tests use in-memory Entity Framework for isolation

## Development Workflows

### Adding API Features
1. Update models in `Nursing.Core/Models/`
2. Add/modify controllers in `Nursing.API/Controllers/`
3. Update services in `Nursing.API/Services/`
4. Add tests in `Nursing.API.Tests/`
5. Run `dotnet build && dotnet test` to verify

### Frontend Development
**Primary Frontend (Svelte):**
- Modify components in `Nursing.Svelte/src/`
- Run `npm run dev` from `Nursing.Svelte/` to test
- Use `npm run build` to create production build

**Deprecated Frontend (Blazor):**
- The Blazor project (`Nursing.Blazor/`) is no longer being actively developed
- Components are in `Nursing.Blazor/Pages/` or `Nursing.Blazor/Components/`
- Can still run with `dotnet run` from `Nursing.Blazor/` if needed for legacy purposes

### Database Changes
- Entity Framework migrations are in `Nursing.API/Migrations/`
- Models are defined in `Nursing.Core/Models/`
- Database context is in `Nursing.API/Services/PostgresContext.cs`
- Tests use in-memory database via `TestDbContext.Create()`

## Common Commands Reference

### Repository Root Commands
```bash
# Full build and test cycle
dotnet workload restore && dotnet restore && dotnet build --no-restore && dotnet test --no-build

# Check project structure
ls -la  # Shows: Nursing.sln, docker-compose.yml, privacy.md, 4 project folders (3 active + 1 deprecated)

# Solution info
dotnet sln list  # Shows all 4 projects in solution (Blazor is deprecated)
```

### Package Information
```bash
# View main solution structure
cat Nursing.sln  # 4 projects: Core, API, Blazor (deprecated), API.Tests

# Check API dependencies  
cat Nursing.API/Nursing.API.csproj  # Entity Framework, PostgreSQL, JWT, Swagger

# Check Svelte dependencies
cat Nursing.Svelte/package.json  # Svelte 5, Vite, Bootstrap, Chart.js, TypeScript
```

## Time Expectations
- **dotnet workload restore**: ~13 seconds
- **dotnet restore**: ~24 seconds  
- **dotnet build**: ~12 seconds
- **dotnet test**: ~4 seconds
- **npm install** (Svelte): ~7 seconds
- **npm run build** (Svelte): ~12 seconds
- **Complete .NET build cycle**: ~45 seconds total
- **Complete Svelte build cycle**: ~20 seconds total

**CRITICAL**: NEVER CANCEL these operations. They complete quickly but set timeouts to 120+ seconds for build operations and 30+ seconds for tests to handle any system variations.