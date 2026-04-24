# Self-contained publish for Windows x64 + ZIP for GitHub Releases.
$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
$csproj = Join-Path $here 'InvoiceGeneratorPro.csproj'
$outDir = Join-Path $here 'publish\win-x64'
$distDir = Join-Path $here 'dist'

if (-not (Test-Path $csproj)) { throw "Project not found: $csproj" }

if (Test-Path $outDir) { Remove-Item $outDir -Recurse -Force }
New-Item -ItemType Directory -Path $outDir -Force | Out-Null
if (-not (Test-Path $distDir)) { New-Item -ItemType Directory -Path $distDir -Force | Out-Null }

dotnet publish $csproj -c Release -r win-x64 --self-contained true -o $outDir
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$zipName = 'InvoiceGeneratorPro-win-x64.zip'
$zipPath = Join-Path $distDir $zipName
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }

Compress-Archive -Path (Join-Path $outDir '*') -DestinationPath $zipPath -CompressionLevel Optimal
Write-Host "Published to: $outDir"
Write-Host "ZIP: $zipPath"
