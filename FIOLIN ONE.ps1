$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$StateDir = Join-Path $Root ".fiolin-one"
$LogsDir = Join-Path $Root "logs"
$StateFile = Join-Path $StateDir "processes.json"
$StartupLog = Join-Path $LogsDir ("startup-{0}.log" -f (Get-Date -Format "yyyyMMdd-HHmmss"))
$BackendLog = Join-Path $LogsDir "backend.log"
$BackendErrorLog = Join-Path $LogsDir "backend-error.log"
$FrontendLog = Join-Path $LogsDir "frontend.log"
$FrontendErrorLog = Join-Path $LogsDir "frontend-error.log"

New-Item -ItemType Directory -Force -Path $StateDir, $LogsDir | Out-Null

function Write-Step {
    param(
        [Parameter(Mandatory = $true)][string]$Message,
        [ConsoleColor]$Color = [ConsoleColor]::Cyan
    )

    $line = "{0} {1}" -f (Get-Date -Format "yyyy-MM-dd HH:mm:ss"), $Message
    Add-Content -Path $StartupLog -Value $line -Encoding UTF8
    Write-Host $Message -ForegroundColor $Color
}

function Fail {
    param([Parameter(Mandatory = $true)][string]$Message)
    Write-Step "HATA: $Message" Red
    Write-Host ""
    Read-Host "Cikmak icin Enter'a basin"
    exit 1
}

function Add-LocalToolPaths {
    $nodeBin = Join-Path $env:USERPROFILE ".cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin"
    if (Test-Path $nodeBin) {
        $env:Path = "$nodeBin;$env:Path"
    }
}

function Get-DotNetCommand {
    $localDotnet = Join-Path $Root ".dotnet-sdk\dotnet.exe"
    if (Test-Path $localDotnet) {
        return $localDotnet
    }

    $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($dotnet) {
        return $dotnet.Source
    }

    return $null
}

function Test-CommandExists {
    param([Parameter(Mandatory = $true)][string]$Name)
    return $null -ne (Get-Command $Name -ErrorAction SilentlyContinue)
}

function Test-Url {
    param([Parameter(Mandatory = $true)][string]$Url)

    try {
        $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 2
        return [int]$response.StatusCode -ge 200 -and [int]$response.StatusCode -lt 500
    }
    catch {
        return $false
    }
}

function Test-DockerReady {
    try {
        docker info 1>$null 2>$null
        return $LASTEXITCODE -eq 0
    }
    catch {
        return $false
    }
}

function Wait-Until {
    param(
        [Parameter(Mandatory = $true)][scriptblock]$Condition,
        [Parameter(Mandatory = $true)][int]$TimeoutSeconds,
        [Parameter(Mandatory = $true)][string]$FailureMessage
    )

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        if (& $Condition) {
            return
        }

        Start-Sleep -Seconds 2
    }

    Fail $FailureMessage
}

function Start-DockerDesktop {
    if (Test-DockerReady) {
        Write-Step "[OK] Docker hazir" Green
        return
    }

    Write-Step "Docker Desktop baslatiliyor..."
    $programFilesX86 = [Environment]::GetEnvironmentVariable("ProgramFiles(x86)")
    $dockerDesktopPaths = @(
        "$env:ProgramFiles\Docker\Docker\Docker Desktop.exe",
        "$programFilesX86\Docker\Docker\Docker Desktop.exe",
        "$env:LOCALAPPDATA\Docker\Docker Desktop.exe"
    )

    $dockerDesktop = $dockerDesktopPaths | Where-Object { Test-Path $_ } | Select-Object -First 1
    if ($dockerDesktop) {
        Start-Process -FilePath $dockerDesktop | Out-Null
    }
    else {
        Fail "Docker kurulu gorunuyor ancak Docker Desktop uygulamasi bulunamadi."
    }

    Wait-Until -TimeoutSeconds 180 -FailureMessage "Docker zamaninda hazir olmadi. Lutfen Docker Desktop uygulamasini kontrol edin." -Condition {
        Test-DockerReady
    }

    Write-Step "[OK] Docker hazir" Green
}

function Wait-Postgres {
    Write-Step "PostgreSQL hazirlaniyor..."
    docker compose up -d postgres 1>> $StartupLog 2>> $StartupLog

    Wait-Until -TimeoutSeconds 180 -FailureMessage "PostgreSQL konteyneri saglikli duruma gecmedi." -Condition {
        $status = docker inspect -f "{{.State.Health.Status}}" fiolin-one-postgres 2>$null
        return $status -eq "healthy"
    }

    Wait-Until -TimeoutSeconds 30 -FailureMessage "5432 portu uzerinden PostgreSQL'e ulasilamiyor." -Condition {
        $connection = Test-NetConnection -ComputerName localhost -Port 5432 -WarningAction SilentlyContinue
        return $connection.TcpTestSucceeded
    }

    Write-Step "[OK] PostgreSQL hazir" Green
}

function Start-Backend {
    param([Parameter(Mandatory = $true)][string]$DotNet)

    if ((Test-Url "http://localhost:5000/health")) {
        Write-Step "[OK] Backend hazir" Green
        return $null
    }

    Write-Step "Backend baslatiliyor..."
    $backendArgs = @(
        "run",
        "--project",
        "backend\src\FiolinOne.Api\FiolinOne.Api.csproj",
        "--urls",
        "http://localhost:5000"
    )

    $process = Start-Process -FilePath $DotNet `
        -ArgumentList $backendArgs `
        -WorkingDirectory $Root `
        -WindowStyle Hidden `
        -RedirectStandardOutput $BackendLog `
        -RedirectStandardError $BackendErrorLog `
        -PassThru

    Wait-Until -TimeoutSeconds 120 -FailureMessage "Backend zamaninda hazir olmadi. Ayrintilar icin logs\backend.log dosyasina bakin." -Condition {
        Test-Url "http://localhost:5000/health"
    }

    Write-Step "[OK] Backend hazir" Green
    return $process.Id
}

function Find-FrontendUrl {
    for ($port = 5173; $port -le 5190; $port++) {
        $url = "http://localhost:{0}" -f $port
        if ((Test-Url $url)) {
            return $url
        }
    }

    return $null
}

function Start-Frontend {
    $existingUrl = Find-FrontendUrl
    if ($existingUrl) {
        Write-Step "[OK] Frontend hazir ($existingUrl)" Green
        return @{ ProcessId = $null; Url = $existingUrl }
    }

    Write-Step "Frontend baslatiliyor..."
    $npm = Get-Command npm.cmd -ErrorAction SilentlyContinue
    if (-not $npm) {
        $npm = Get-Command npm -ErrorAction SilentlyContinue
    }

    if (-not $npm) {
        Fail "NPM bulunamadi. Node.js/NPM kurulumu gerekli."
    }

    $process = Start-Process -FilePath $npm.Source `
        -ArgumentList @("run", "dev", "--", "--host", "localhost", "--port", "5173") `
        -WorkingDirectory (Join-Path $Root "frontend") `
        -WindowStyle Hidden `
        -RedirectStandardOutput $FrontendLog `
        -RedirectStandardError $FrontendErrorLog `
        -PassThru

    $script:frontendUrl = $null
    Wait-Until -TimeoutSeconds 120 -FailureMessage "Frontend zamaninda hazir olmadi. Ayrintilar icin logs\frontend.log dosyasina bakin." -Condition {
        $script:frontendUrl = Find-FrontendUrl
        return $null -ne $script:frontendUrl
    }

    Write-Step "[OK] Frontend hazir ($script:frontendUrl)" Green
    return @{ ProcessId = $process.Id; Url = $script:frontendUrl }
}

Write-Host ""
Write-Step "FIOLIN ONE baslatiliyor..." Cyan

Add-LocalToolPaths

if (-not (Test-CommandExists "docker")) {
    Fail "Docker bulunamadi. Lutfen Docker Desktop kurun."
}

$dotnet = Get-DotNetCommand
if (-not $dotnet) {
    Fail ".NET SDK bulunamadi."
}

if (-not (Test-CommandExists "node")) {
    Fail "Node.js bulunamadi."
}

if (-not (Test-CommandExists "npm") -and -not (Test-CommandExists "npm.cmd")) {
    Fail "NPM bulunamadi."
}

Push-Location $Root
try {
    Start-DockerDesktop
    Wait-Postgres
    $backendPid = Start-Backend -DotNet $dotnet
    $frontend = Start-Frontend

    $state = [ordered]@{
        StartedAt = (Get-Date).ToString("o")
        BackendPid = $backendPid
        FrontendPid = $frontend.ProcessId
        FrontendUrl = $frontend.Url
        BackendUrl = "http://localhost:5000"
        SwaggerUrl = "http://localhost:5000/swagger"
    }

    $state | ConvertTo-Json | Set-Content -Path $StateFile -Encoding UTF8

    Write-Step "Tarayici aciliyor..." Cyan
    Start-Process $frontend.Url | Out-Null

    Write-Host ""
    Write-Step "Sistem hazir." Green
    Write-Host "Uygulama: $($frontend.Url)" -ForegroundColor Green
    Write-Host "Swagger:  http://localhost:5000/swagger" -ForegroundColor Green
    Write-Host "Kapatmak icin: FIOLIN ONE Kapat.cmd" -ForegroundColor Yellow
}
finally {
    Pop-Location
}

