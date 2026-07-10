$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$LogsDir = Join-Path $Root "logs"
$UpdateLog = Join-Path $LogsDir ("update-{0}.log" -f (Get-Date -Format "yyyyMMdd-HHmmss"))

New-Item -ItemType Directory -Force -Path $LogsDir | Out-Null

function Write-Step {
    param(
        [Parameter(Mandatory = $true)][string]$Message,
        [ConsoleColor]$Color = [ConsoleColor]::Cyan
    )

    $line = "{0} {1}" -f (Get-Date -Format "yyyy-MM-dd HH:mm:ss"), $Message
    Add-Content -Path $UpdateLog -Value $line -Encoding UTF8
    Write-Host $Message -ForegroundColor $Color
}

function Write-LogOutput {
    param(
        [Parameter(ValueFromPipeline = $true)]$InputObject,
        [Parameter(Mandatory = $true)][string]$LogPath
    )

    process {
        if ($null -ne $InputObject) {
            Add-Content -Path $LogPath -Value ([string]$InputObject) -Encoding UTF8
        }
    }
}

function Invoke-LoggedCommand {
    param(
        [Parameter(Mandatory = $true)][scriptblock]$Command,
        [Parameter(Mandatory = $true)][string]$LogPath
    )

    $previousErrorActionPreference = $ErrorActionPreference
    try {
        $ErrorActionPreference = "Continue"
        & $Command 2>&1 | Write-LogOutput -LogPath $LogPath
    }
    finally {
        $ErrorActionPreference = $previousErrorActionPreference
    }
}

function Run-Step {
    param(
        [Parameter(Mandatory = $true)][string]$Message,
        [Parameter(Mandatory = $true)][scriptblock]$Command
    )

    Write-Step $Message
    Invoke-LoggedCommand -LogPath $UpdateLog -Command $Command
    if ($LASTEXITCODE -ne 0) {
        throw "$Message basarisiz oldu. Ayrintilar icin logs klasorundeki update log dosyasina bakin."
    }
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

    throw ".NET SDK bulunamadi."
}

Write-Host ""
Write-Step "FIOLIN ONE guncelleniyor..."

Add-LocalToolPaths
$dotnet = Get-DotNetCommand

Push-Location $Root
try {
    Run-Step "Mevcut uygulama surecleri kapatiliyor..." {
        powershell -ExecutionPolicy Bypass -File (Join-Path $Root "FIOLIN ONE Kapat.ps1")
    }

    Run-Step "Kaynak kod guncelleniyor..." {
        git pull
    }

    Run-Step "PostgreSQL baslatiliyor..." {
        docker compose up -d postgres
    }

    Run-Step "Backend paketleri hazirlaniyor..." {
        & $dotnet restore "backend\FIOLIN-ONE.sln"
    }

    if (-not (Test-Path (Join-Path $Root "frontend\node_modules"))) {
        Run-Step "Frontend paketleri kuruluyor..." {
            npm install --prefix frontend
        }
    }
    else {
        Write-Step "[OK] Frontend paketleri zaten kurulu" Green
    }

    Write-Step "Migration kontrolu uygulama acilisinda otomatik yapilacak." Green
}
finally {
    Pop-Location
}

Write-Step "Guncelleme tamamlandi. FIOLIN ONE baslatiliyor..." Green
powershell -ExecutionPolicy Bypass -File (Join-Path $Root "FIOLIN ONE.ps1")

