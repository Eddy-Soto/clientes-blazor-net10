param(
    [string]$ApiUrl = "http://localhost:5092/",
    [string]$WebUrl = "http://localhost:5080",
    [switch]$ClavesLocalesDePrueba
)
$ErrorActionPreference = "Stop"
$apiUri = [Uri]$ApiUrl
$webUri = [Uri]$WebUrl
if (-not $apiUri.IsAbsoluteUri -or $apiUri.Scheme -notin @("http", "https")) { throw "ApiUrl debe ser una URL HTTP o HTTPS absoluta." }
if (-not $webUri.IsAbsoluteUri -or $webUri.Scheme -notin @("http", "https")) { throw "WebUrl debe ser una URL HTTP o HTTPS absoluta." }
if ($apiUri.Authority -eq $webUri.Authority) { throw "La API y Blazor necesitan puertos o servidores diferentes." }
$previousApi = $env:Api__BaseUrl
$previousKeys = $env:DataProtection__KeyPath
Push-Location $PSScriptRoot
try {
    $env:Api__BaseUrl = $ApiUrl.TrimEnd("/") + "/"
    if ($ClavesLocalesDePrueba) {
        $env:DataProtection__KeyPath = Join-Path $PSScriptRoot ".local/keys"
        New-Item -ItemType Directory -Force -Path $env:DataProtection__KeyPath | Out-Null
    }
    Write-Host "Blazor: $($WebUrl.TrimEnd('/'))/clientes"
    Write-Host "API: $env:Api__BaseUrl"
    dotnet run --project Clientes.Web --launch-profile http --urls $WebUrl
    if ($LASTEXITCODE -ne 0) { throw "No se pudo iniciar Blazor." }
}
finally {
    $env:Api__BaseUrl = $previousApi
    $env:DataProtection__KeyPath = $previousKeys
    Pop-Location
}

