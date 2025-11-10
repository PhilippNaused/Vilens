#Requires -Version 7.4

[CmdletBinding()]
param (
  [Parameter()]
  [ValidateSet('Debug', 'Release')]
  [string]$Configuration = 'Release',

  [Parameter()]
  [ValidateSet('Private', 'Internal', 'Public')]
  [string]$Scope = 'Internal',

  [Parameter()]
  [string]$Features = 'All',

  [Parameter()]
  [switch]$Aot,

  [ValidateSet('net9.0', 'net10.0')]
  [Parameter()]
  [string]$Framework = 'net9.0',

  [Parameter()]
  [ValidateRange(2, [int]::MaxValue)]
  [int]$Iterations = 20
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 3.0

$Target = 'Bshox'
$SourceFile = Join-Path $PSScriptRoot ".\Tests\Performance\TestAssemblies\$Target.dll"
$SourcePdb = Join-Path $PSScriptRoot ".\Tests\Performance\TestAssemblies\$Target.pdb"
$StrongNamingKey = Join-Path $PSScriptRoot "Vilens.snk"

$TempFolder = Join-Path $PSScriptRoot ".\Tests\Performance\Temp"
if (-not (Test-Path $TempFolder)) {
  New-Item $TempFolder -ItemType Directory
}
else {
  Remove-Item $TempFolder\* -Recurse
}

$VilensConsole = .\Publish.ps1 -Configuration $Configuration -NoSelfObfuscate -Aot:$Aot -Framework $Framework

$Times = @()
for ($i = 1; $i -le $Iterations; $i++) {
  $TargetFile = Copy-Item $SourceFile -Destination $TempFolder -PassThru -Force
  if (Test-Path $SourcePdb) {
    Copy-Item $SourcePdb -Destination $TempFolder -Force | Out-Null
  }
  $StartTime = Get-Date
  if ($Aot) {
    & $VilensConsole $TargetFile --scope $Scope --features $Features --strongNamingKey $StrongNamingKey | Out-Null
  }
  else {
    & dotnet $VilensConsole $TargetFile --scope $Scope --features $Features --strongNamingKey $StrongNamingKey | Out-Null
  }
  # skip the first iteration
  if ($i -eq 0) {
    continue
  }
  $Times += ((Get-Date) - $StartTime)
  $Stats = $Times | Measure-Object -AllStats -Property TotalMilliseconds
  Write-Progress -Activity "Running performance tests" -Status ("{0}±{1} ms" -f [int]$Stats.Average, [int]$Stats.StandardDeviation) -PercentComplete (($i / $Iterations) * 100)
}
Write-Progress -Activity "Running performance tests" -Completed
$Times | Measure-Object -AllStats -Property TotalMilliseconds
# Windows: ~325 ms (~32 AOT)
# Linux: ~363 ms (~64 AOT)
