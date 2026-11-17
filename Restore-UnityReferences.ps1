param([string]$SevenZipPath, [string]$CacheRoot)
$ErrorActionPreference = 'Stop'
function Get-UnitySha256([string]$Path) {
    $stream = [IO.File]::OpenRead($Path)
    $algorithm = [Security.Cryptography.SHA256]::Create()
    try { return [BitConverter]::ToString($algorithm.ComputeHash($stream)).Replace('-', '').ToLowerInvariant() }
    finally { $algorithm.Dispose(); $stream.Dispose() }
}
if (-not $CacheRoot) { $CacheRoot = Join-Path $PSScriptRoot '..\..\_tools\unity-5.0.0f4' }
$CacheRoot = [IO.Path]::GetFullPath($CacheRoot)
$managed = Join-Path $CacheRoot 'extracted\Editor\Data\Managed'
$engine = Join-Path $managed 'UnityEngine.dll'
$provenance = Get-Content -Raw -LiteralPath (Join-Path $PSScriptRoot 'tools\unity5-reference-provenance.json') | ConvertFrom-Json
if (Test-Path -LiteralPath $engine) {
    if ((Get-UnitySha256 $engine) -ne $provenance.engineSha256) { throw 'UnityEngine.dll no coincide con la referencia comprobada.' }
    Write-Output $managed
    return
}
if (-not $SevenZipPath) {
    $command = Get-Command 7z.exe -ErrorAction SilentlyContinue
    if ($command) { $SevenZipPath = $command.Source }
}
if (-not $SevenZipPath) {
    foreach ($candidate in @('C:\Program Files\NVIDIA Corporation\NVIDIA app\7z.exe', 'C:\Program Files\7-Zip\7z.exe', 'D:\Program Files\7-Zip\7z.exe', 'D:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\Extensions\Xamarin.VisualStudio\7-Zip\7z.exe')) {
        if (Test-Path -LiteralPath $candidate) { $SevenZipPath = $candidate; break }
    }
}
if (-not $SevenZipPath) { throw 'Se necesita 7-Zip completo para extraer el SDK. Instala 7-Zip desde https://www.7-zip.org/ o indica -SevenZipPath.' }
New-Item -ItemType Directory -Path $CacheRoot -Force | Out-Null
$installer = Join-Path $CacheRoot 'UnitySetup64-5.0.0f4.exe'
if (-not (Test-Path -LiteralPath $installer)) {
    Write-Host 'Descargando el archivo oficial Unity 5 (aproximadamente 1.2 GB); se extrae una referencia, no se ejecuta ni instala el editor.'
    $partial = Join-Path $CacheRoot 'UnitySetup64-5.0.0f4.download'
    $ProgressPreference = 'SilentlyContinue'
    $client = [Net.WebClient]::new()
    try { $client.DownloadFile([string]$provenance.downloadUrl, $partial) }
    finally { $client.Dispose() }
    if ((Get-UnitySha256 $partial) -ne $provenance.installerSha256) { throw 'La descarga no coincide con la huella comprobada; se conserva para revisión.' }
    Move-Item -LiteralPath $partial -Destination $installer
}
if ((Get-UnitySha256 $installer) -ne $provenance.installerSha256) { throw 'El instalador en cache no coincide con la huella comprobada.' }
$extractRoot = Join-Path $CacheRoot 'extracted'
New-Item -ItemType Directory -Path $extractRoot -Force | Out-Null
& $SevenZipPath x $installer "-o$extractRoot" 'Editor\Data\Managed\UnityEngine.dll' -y -bso0 -bsp0 | Out-Host
if ($LASTEXITCODE -ne 0 -or -not (Test-Path -LiteralPath $engine)) { throw 'No se pudo extraer la referencia UnityEngine.dll del archivo oficial.' }
if ((Get-UnitySha256 $engine) -ne $provenance.engineSha256) { throw 'La DLL extraida no coincide con la referencia comprobada.' }
Write-Output $managed
