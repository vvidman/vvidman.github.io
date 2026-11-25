<#
.SYNOPSIS
  Exports infographic HTML sources to PNG with headless Edge (Chrome fallback).
.EXAMPLE
  ./infographics/export.ps1 -Id inferrouter-01
  ./infographics/export.ps1 -All
  ./infographics/export.ps1 -Id _example
#>
param(
    [string]$Id,
    [switch]$All
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$root   = $PSScriptRoot
$srcDir = Join-Path $root 'src'
$outDir = Join-Path (Split-Path $root -Parent) 'docs/assets/images/infographics'
New-Item -ItemType Directory -Force $outDir | Out-Null

function Find-Browser {
    $candidates = @(
        "$env:ProgramFiles\Microsoft\Edge\Application\msedge.exe",
        "${env:ProgramFiles(x86)}\Microsoft\Edge\Application\msedge.exe"
    )
    foreach ($base in @("${env:ProgramFiles(x86)}\Microsoft\EdgeCore", "$env:ProgramFiles\Microsoft\EdgeCore")) {
        if (Test-Path $base) {
            $candidates += Get-ChildItem $base -Recurse -Filter msedge.exe -ErrorAction SilentlyContinue |
                Sort-Object FullName -Descending | ForEach-Object FullName
        }
    }
    $candidates += "$env:ProgramFiles\Google\Chrome\Application\chrome.exe",
                   "${env:ProgramFiles(x86)}\Google\Chrome\Application\chrome.exe"
    foreach ($c in $candidates) { if ($c -and (Test-Path $c)) { return $c } }
    throw 'Neither Edge nor Chrome found.'
}

$browser = Find-Browser

function Export-One([string]$name, [int]$w, [int]$h) {
    $html = Join-Path $srcDir "$name.html"
    if (-not (Test-Path $html)) { throw "Missing source: $html" }
    $png = Join-Path $outDir ($name + '.png')
    if (Test-Path $png) { Remove-Item $png -Force }
    $url = ([System.Uri](Resolve-Path $html).Path).AbsoluteUri
    $profileDir = Join-Path ([System.IO.Path]::GetTempPath()) ('ig-export-' + [guid]::NewGuid().ToString('N'))
    try {
        & $browser --headless=new --disable-gpu --hide-scrollbars --force-device-scale-factor=1 `
            "--user-data-dir=$profileDir" "--window-size=$w,$h" "--screenshot=$png" `
            --virtual-time-budget=3000 $url 2>&1 | Out-Null
    } finally {
        if (Test-Path $profileDir) { Remove-Item $profileDir -Recurse -Force -ErrorAction SilentlyContinue }
    }
    if (-not (Test-Path $png)) { throw "Export failed: $name" }
    $img = [System.Drawing.Image]::FromFile($png)
    try { $size = "$($img.Width)x$($img.Height)"; $ok = ($img.Width -eq $w -and $img.Height -eq $h) } finally { $img.Dispose() }
    if (-not $ok) { throw "$name has size $size, expected ${w}x${h}" }
    Write-Host "OK  $png  ($size)"
}

if ($All) {
    $ids = Get-ChildItem $srcDir -Filter '*-square.html' | Where-Object { $_.Name -notlike '_*' } |
        ForEach-Object { $_.BaseName -replace '-square$', '' }
} elseif ($Id) {
    $ids = @($Id)
} else {
    throw 'Specify -Id <id> or -All.'
}

foreach ($i in $ids) {
    Export-One "$i-square" 1200 1200
    Export-One "$i-og" 1200 630
}
