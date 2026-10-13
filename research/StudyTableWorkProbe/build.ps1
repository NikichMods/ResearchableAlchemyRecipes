# SPDX-License-Identifier: MPL-2.0
param(
    [Parameter(Mandatory=$true)][string]$GameRoot,
    [Parameter(Mandatory=$true)][string]$DotnetRoot,
    [Parameter(Mandatory=$true)][string]$ExpectedSource,
    [switch]$CheckOnly
)
$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$sourceHead = (& git -C $repoRoot rev-parse HEAD).Trim()
if ($sourceHead -ne $ExpectedSource) { throw 'Unexpected source commit' }
if (!$CheckOnly -and (& git -C $repoRoot status --porcelain --untracked-files=normal)) { throw 'Source tree must be clean for a handoff build' }
$managed = Join-Path $GameRoot 'Graveyard Keeper_Data/Managed'
$core = Join-Path $GameRoot 'BepInEx/core'
$compiler = Join-Path $DotnetRoot 'sdk/6.0.406/Roslyn/bincore/csc.dll'
$dotnet = Join-Path $DotnetRoot 'dotnet.exe'
$versionMatch = [regex]::Match((Get-Content -LiteralPath (Join-Path $PSScriptRoot 'StudyTableWorkProbe.cs') -Raw), 'public const string Version = "([0-9]+\.[0-9]+\.[0-9]+)";')
if (!$versionMatch.Success) { throw 'Missing canonical probe version' }
$probeVersion = $versionMatch.Groups[1].Value
$outDir = Join-Path $repoRoot ('artifacts/StudyTableWorkProbe-'+$probeVersion)
New-Item -ItemType Directory -Path $outDir -Force | Out-Null
$output = Join-Path $outDir $(if ($CheckOnly) { 'compile-check.dll' } else { 'StudyTableWorkProbe-'+$probeVersion+'.dll' })
if (!$CheckOnly -and (Test-Path -LiteralPath $output)) { throw 'Numbered output already exists; do not overwrite it' }
$refs = @('mscorlib.dll','System.dll','System.Core.dll','Assembly-CSharp.dll','Assembly-CSharp-firstpass.dll','UnityEngine.dll','UnityEngine.CoreModule.dll','UnityEngine.Physics2DModule.dll','UnityEngine.InputLegacyModule.dll','UnityEngine.ImageConversionModule.dll') | ForEach-Object { Join-Path $managed $_ }
$refs += @('BepInEx.dll','0Harmony.dll') | ForEach-Object { Join-Path $core $_ }
$compilerArgs = @('/nologo','/target:library','/nostdlib+','/noconfig','/langversion:7.3','/optimize+','/deterministic+','/warnaserror+',('/out:'+$output))
$compilerArgs += $refs | ForEach-Object { if (!(Test-Path -LiteralPath $_)) { throw "Missing local reference: $_" }; '/reference:'+$_ }
$compilerArgs += @(Get-ChildItem -LiteralPath $PSScriptRoot -Filter '*.cs' | Sort-Object Name | ForEach-Object FullName)
$artwork = @('MaterialStudy','RecipeResearch') | ForEach-Object { [ordered]@{ name=$_; path=(Join-Path $PSScriptRoot ('assets/'+$_+'-icon-40x26.png')) } }
$compilerArgs += $artwork | ForEach-Object { if (!(Test-Path -LiteralPath $_.path)) { throw 'Missing original tab artwork' }; '/resource:'+($_.path)+',RAR.'+($_.name)+'.png' }
& $dotnet $compiler @compilerArgs
if ($LASTEXITCODE -ne 0) { throw "Compilation failed: $LASTEXITCODE" }
if ($CheckOnly) { Write-Output 'Compile check passed (development source, not a handoff artifact)'; return }
$version = [Reflection.AssemblyName]::GetAssemblyName($output).Version.ToString()
if ($version -ne ($probeVersion+'.0')) { throw 'Artifact version mismatch' }
$fileVersion = [Diagnostics.FileVersionInfo]::GetVersionInfo($output).FileVersion
if ($fileVersion -ne ($probeVersion+'.0')) { throw 'File version mismatch' }
$hash = (Get-FileHash -LiteralPath $output -Algorithm SHA256).Hash.ToLowerInvariant()
$manifest = [ordered]@{ version=$probeVersion; source=$sourceHead; compiler='SDK 6.0.406 Roslyn'; artifact=(Split-Path $output -Leaf); sha256=$hash; artwork=@($artwork | ForEach-Object { [ordered]@{ resource=('RAR.'+$_.name+'.png'); sha256=(Get-FileHash -LiteralPath $_.path -Algorithm SHA256).Hash.ToLowerInvariant() } }); references=@($refs | ForEach-Object { [ordered]@{ name=(Split-Path $_ -Leaf); sha256=(Get-FileHash -LiteralPath $_ -Algorithm SHA256).Hash.ToLowerInvariant() } }) }
$manifest | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $outDir 'build-identity.json') -Encoding utf8
Write-Output "Built $output SHA256=$hash source=$sourceHead"
