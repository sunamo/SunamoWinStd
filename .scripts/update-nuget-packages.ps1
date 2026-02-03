# Update all NuGet packages to latest versions in all projects

$projectFiles = @(
    "SunamoWinStd\SunamoWinStd.csproj",
    "SunamoWinStd.Tests\SunamoWinStd.Tests.csproj",
    "RunnerWinStd\RunnerWinStd.csproj"
)

foreach ($projectFile in $projectFiles) {
    Write-Host "Processing project: $projectFile" -ForegroundColor Cyan

    # Get all package references
    $xml = [xml](Get-Content $projectFile)
    $packages = $xml.Project.ItemGroup.PackageReference | Where-Object { $_.Include -and $_.Version -ne '*' }

    foreach ($package in $packages) {
        $packageName = $package.Include
        if ($packageName) {
            Write-Host "  Updating package: $packageName" -ForegroundColor Yellow
            dotnet add $projectFile package $packageName
        }
    }
}

Write-Host "All packages updated!" -ForegroundColor Green
