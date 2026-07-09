$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$StateDir = Join-Path $Root ".fiolin-one"
$LogsDir = Join-Path $Root "logs"
$StateFile = Join-Path $StateDir "processes.json"
$ShutdownLog = Join-Path $LogsDir ("shutdown-{0}.log" -f (Get-Date -Format "yyyyMMdd-HHmmss"))

New-Item -ItemType Directory -Force -Path $StateDir, $LogsDir | Out-Null

function Write-Step {
    param(
        [Parameter(Mandatory = $true)][string]$Message,
        [ConsoleColor]$Color = [ConsoleColor]::Cyan
    )

    $line = "{0} {1}" -f (Get-Date -Format "yyyy-MM-dd HH:mm:ss"), $Message
    Add-Content -Path $ShutdownLog -Value $line -Encoding UTF8
    Write-Host $Message -ForegroundColor $Color
}

function Stop-ProcessTree {
    param([Nullable[int]]$ProcessId)

    if (-not $ProcessId) {
        return
    }

    $process = Get-Process -Id $ProcessId -ErrorAction SilentlyContinue
    if (-not $process) {
        return
    }

    taskkill /PID $ProcessId /T /F 1>> $ShutdownLog 2>> $ShutdownLog
}

Write-Host ""
Write-Step "FIOLIN ONE kapatiliyor..."

if (Test-Path $StateFile) {
    $state = Get-Content -Raw $StateFile | ConvertFrom-Json
    Stop-ProcessTree -ProcessId $state.FrontendPid
    Stop-ProcessTree -ProcessId $state.BackendPid
    Remove-Item -Path $StateFile -Force
}
else {
    Write-Step "Kayitli calisan uygulama sureci bulunamadi." Yellow
}

if ($args -contains "--docker") {
    Write-Step "Docker konteynerleri durduruluyor..."
    Push-Location $Root
    try {
        docker compose stop 1>> $ShutdownLog 2>> $ShutdownLog
    }
    finally {
        Pop-Location
    }
}
else {
    Write-Step "PostgreSQL konteyneri calisir durumda birakildi. Docker'i da durdurmak icin: FIOLIN ONE Kapat.cmd --docker" Yellow
}

Write-Step "[OK] FIOLIN ONE kapatildi" Green

