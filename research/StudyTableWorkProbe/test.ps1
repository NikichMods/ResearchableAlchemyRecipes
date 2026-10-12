# SPDX-License-Identifier: MPL-2.0
param(
    [Parameter(Mandatory=$true)][string]$GameRoot,
    [Parameter(Mandatory=$true)][string]$DotnetRoot
)
$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
& (Join-Path $PSScriptRoot 'build.ps1') -GameRoot $GameRoot -DotnetRoot $DotnetRoot -ExpectedSource (& git -C $repoRoot rev-parse HEAD) -CheckOnly
$version = [regex]::Match((Get-Content (Join-Path $PSScriptRoot 'StudyTableWorkProbe.cs') -Raw), 'public const string Version = "([0-9.]+)";').Groups[1].Value
$outDir = Join-Path $repoRoot ('artifacts/StudyTableWorkProbe-'+$version)
$managed = Join-Path $GameRoot 'Graveyard Keeper_Data/Managed'
$core = Join-Path $GameRoot 'BepInEx/core'
$foreign = Join-Path $GameRoot 'BepInEx/plugins/MaxButtonsRedux'
$refs = @('mscorlib.dll','System.dll','System.Core.dll','Assembly-CSharp.dll','Assembly-CSharp-firstpass.dll','UnityEngine.dll','UnityEngine.CoreModule.dll') | ForEach-Object { Join-Path $managed $_ }
$refs += @('BepInEx.dll','0Harmony.dll') | ForEach-Object { Join-Path $core $_ }
foreach ($testName in @('KnowledgeStateTests','QuantityCompatibilityTests','SourceTopicTests')) {
    $output = Join-Path $outDir ($testName+'.exe')
    $compilerArgs = @('/nologo','/target:exe','/nostdlib+','/noconfig','/langversion:7.3','/warnaserror+',('/out:'+$output))
    $compilerArgs += $refs | ForEach-Object { '/reference:'+$_ }
    $compilerArgs += Join-Path $PSScriptRoot ('tests/'+$testName+'.cs')
    & (Join-Path $DotnetRoot 'dotnet.exe') (Join-Path $DotnetRoot 'sdk/6.0.406/Roslyn/bincore/csc.dll') @compilerArgs
    if ($LASTEXITCODE -ne 0) { throw "Test compilation failed: $testName" }
    & $output $outDir $managed $core $foreign
    if ($LASTEXITCODE -ne 0) { throw "Test failed: $testName" }
}
