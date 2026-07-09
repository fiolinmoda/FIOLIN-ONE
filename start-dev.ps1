$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $root "FIOLIN ONE.ps1")

