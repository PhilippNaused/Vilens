#Requires -Version 7.4

[CmdletBinding()]
param (
  [Parameter(Mandatory, Position = 0)]
  [string]$Target,

  [Parameter()]
  [ValidateSet('Debug', 'Release')]
  [string]$Configuration = 'Debug',

  [Parameter()]
  [ValidateSet('Private', 'Internal', 'Public')]
  [string]$Scope = 'Internal',

  [Parameter()]
  [string]$Features = 'All',

  [Parameter()]
  [switch]$NoSelfObfuscate,

  [Parameter()]
  [switch]$Debugger
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 3.0

$TargetFile = Get-Item $Target
$LogFile = "$TargetFile.vilens.log"
if (Test-Path $LogFile) {
  Remove-Item $LogFile
}
$VilensConsole = .\Publish.ps1 -Configuration $Configuration -NoSelfObfuscate:$NoSelfObfuscate
$StartTime = Get-Date
$ExtraArgs = @()
if ($Debugger) {
  $ExtraArgs += '--debug'
}
& dotnet $VilensConsole $TargetFile --scope $Scope --features $Features $ExtraArgs
$Time = (Get-Date) - $StartTime
Write-Output "Time: $Time"
