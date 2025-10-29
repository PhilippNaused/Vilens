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
  [switch]$NoSelfObfuscate,

  [Parameter()]
  [switch]$Aot,

  [ValidateSet('net9.0', 'net10.0')]
  [Parameter()]
  [string]$Framework = 'net9.0',

  [Parameter()]
  [ValidateRange(2, [int]::MaxValue)]
  [int]$Iteration = 20
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

$VilensConsole = .\Publish.ps1 -Configuration $Configuration -NoSelfObfuscate:$NoSelfObfuscate -Aot:$Aot -Framework $Framework

$Times = @()
for ($i = 0; $i -lt $Iteration; $i++) {
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
  if ($i -gt 0) {
    $Times += ((Get-Date) - $StartTime)
  }
  Write-Progress -Activity "Running performance tests" -Status ("Iterations remaining: {0}" -f ($Iteration - $i)) -PercentComplete (($i / $Iteration) * 100)
}
Write-Progress -Activity "Running performance tests" -Completed
$Times | Measure-Object -AllStats -Property TotalMilliseconds # ~355 ms (~35 AOT)
