#Requires -Version 7.4

[CmdletBinding()]
param (
  # The name of the project to test
  [Parameter(Position = 0)]
  [ValidateSet('Moq', 'Autofac', 'Serilog', 'NUnit')]
  [string[]]$Targets = @('Moq', 'Autofac', 'Serilog', 'NUnit'),

  [Parameter()]
  [ValidateSet('Debug', 'Release')]
  [string]$Configuration = 'Debug',

  [Parameter()]
  [ValidateSet('Debug', 'Release')]
  [string]$TargetConfiguration = 'Release',

  [Parameter()]
  [string]$Features = 'All',

  [Parameter()]
  [switch]$NoCleanup
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 3.0

if (-Not $Targets) {
  throw "No targets specified"
}

$Version = "0.0.0-Dev.$(Get-Date -Format 'yyyyMMddHHmmss')"

.\Pack.ps1 -Configuration $Configuration -Version $Version

try {
  $Time = Get-Date

  foreach ($Target in $Targets) {
    .\Tests\Integration\Run-Test.ps1 -Target $Target -Version $Version -Configuration $TargetConfiguration -Features $Features -NoCleanup:$NoCleanup
  }

  $Time = (Get-Date) - $Time
  Write-Output "Tests completed in $Time"
}
finally {
  $GlobalNuGetCache = nuget locals global-packages -List -ForceEnglishOutput
  $GlobalNuGetCache = Get-Item $GlobalNuGetCache.Substring("global-packages: ".Length)
  $PackageDir = Join-Path $GlobalNuGetCache "vilens.msbuild"
  if (Test-Path $PackageDir) {
    Remove-Item $PackageDir -Recurse -Force
  }
}
