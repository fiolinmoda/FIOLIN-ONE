$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$nodeBin = Join-Path $env:USERPROFILE ".cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin"
$dotnet = Join-Path $root ".dotnet-sdk\dotnet.exe"

if (Test-Path $nodeBin) {
    $env:Path = "$nodeBin;$env:Path"
}

Write-Host "FIOLIN ONE yerel geliştirme ortamı başlatılıyor..." -ForegroundColor Cyan

Push-Location $root
try {
    Write-Host "PostgreSQL başlatılıyor..." -ForegroundColor Cyan
    docker compose up -d postgres

    Write-Host "Backend başlatılıyor: http://localhost:5000" -ForegroundColor Cyan
    Start-Process powershell -WindowStyle Hidden -WorkingDirectory $root -ArgumentList @(
        "-NoExit",
        "-Command",
        "& '$dotnet' run --project backend\src\FiolinOne.Api\FiolinOne.Api.csproj"
    )

    Write-Host "Frontend başlatılıyor: http://localhost:5173" -ForegroundColor Cyan
    Start-Process powershell -WindowStyle Hidden -WorkingDirectory (Join-Path $root "frontend") -ArgumentList @(
        "-NoExit",
        "-Command",
        "npm run dev -- --host localhost --port 5173"
    )

    Write-Host ""
    Write-Host "Hazır. Birkaç saniye sonra şu adresleri açabilirsiniz:" -ForegroundColor Green
    Write-Host "Frontend: http://localhost:5173"
    Write-Host "Swagger:  http://localhost:5000/swagger"
    Write-Host "Health:   http://localhost:5000/health"
}
finally {
    Pop-Location
}
