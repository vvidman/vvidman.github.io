param(
    [int]$Port = 8080
)

$ErrorActionPreference = "Stop"

Write-Host "🔨 Building blog..."
dotnet run --project .\blog-build\blog-build.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed."
    exit 1
}

Write-Host "📦 Build completed."

$docsPath = Join-Path $PSScriptRoot "docs"

if (-not (Test-Path $docsPath)) {
    Write-Error "docs directory not found: $docsPath"
    exit 1
}

Write-Host "🌍 Starting local server at http://localhost:$Port"
Write-Host "Press Ctrl+C to stop."

Push-Location $docsPath

# Prefer Python, fallback to dotnet
try {
    python -m http.server $Port
}
catch {
    Write-Warning "Python not found, using dotnet serve"

    dotnet tool install --global dotnet-serve --version 1.11.0
    dotnet serve -p $Port
}

Pop-Location