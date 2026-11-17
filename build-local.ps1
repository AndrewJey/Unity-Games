param([switch]$Test, [switch]$RestoreUnityReferences, [string]$UnityManagedDirectory, [string]$DotNetPath)
$ErrorActionPreference = 'Stop'
if ($RestoreUnityReferences) {
    $restored = & (Join-Path $PSScriptRoot 'Restore-UnityReferences.ps1')
    if (-not $UnityManagedDirectory) { $UnityManagedDirectory = $restored }
}
if (-not $UnityManagedDirectory) { $UnityManagedDirectory = Join-Path $PSScriptRoot '..\..\_tools\unity-5.0.0f4\extracted\Editor\Data\Managed' }
if (-not (Test-Path -LiteralPath (Join-Path $UnityManagedDirectory 'UnityEngine.dll'))) { throw 'Falta la DLL oficial UnityEngine.dll. Ejecuta .\build-local.ps1 -RestoreUnityReferences o indica -UnityManagedDirectory.' }
$UnityManagedDirectory = (Resolve-Path -LiteralPath $UnityManagedDirectory).Path
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$msbuild = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\Current\Bin\MSBuild.exe' | Select-Object -First 1
if (-not $msbuild) { throw 'Requiere MSBuild de Visual Studio 2022.' }
foreach ($name in @('Assembly-CSharp.csproj', 'Assembly-CSharp-vs.csproj')) {
    $project = Join-Path $PSScriptRoot "MyfirstUnityProject\$name"
    & $msbuild $project /nologo /m:1 /v:minimal /restore /t:Build /p:Configuration=Debug /p:DeployOnBuild=false /p:RunPostBuildEvent=Never /p:PreBuildEvent= /p:PostBuildEvent= "/p:UnityManagedDirectory=$UnityManagedDirectory"
    if ($LASTEXITCODE -ne 0) { throw "No compila: $name" }
}
if ($Test) {
    if (-not $DotNetPath) {
        $portable = Join-Path $PSScriptRoot '..\..\_tools\dotnet10\dotnet.exe'
        if (Test-Path -LiteralPath $portable) { $DotNetPath = $portable }
        else { $DotNetPath = (Get-Command dotnet.exe -ErrorAction Stop).Source }
    }
    & $DotNetPath run --project (Join-Path $PSScriptRoot 'tests\ViewportChecks\ViewportChecks.csproj')
    if ($LASTEXITCODE -ne 0) { throw 'Fallo de pruebas de viewport. Requiere SDK .NET 10 por terminal.' }
}
Write-Output 'Ambos proyectos C# compilados. Este comando no arranca Unity ni exporta un juego.'
