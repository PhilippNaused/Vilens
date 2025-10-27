#Requires -Version 7.4

[CmdletBinding()]
param (
  [Parameter()]
  [ValidateSet('Debug', 'Release')]
  [string]$Configuration = 'Release',

  [Parameter()]
  [string[]]$Features = @('All'),

  [Parameter()]
  [switch]$NoSelfObfuscate,

  [Parameter()]
  [switch]$Aot,

  [ValidateSet('net9.0', 'net10.0')]
  [Parameter()]
  [string]$Framework = 'net9.0'
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 3.0

if (Test-Path 'Publish') {
  Remove-Item 'Publish/*' -Recurse -Force
}

if ($Aot) {
  $AotCommand = "--use-current-runtime", "-p:Aot=true"
}
else {
  $AotCommand = $null
}

dotnet publish ./src/Vilens.Tool --output 'Publish' -c $Configuration -f $Framework $AotCommand | Out-Host

if ($Aot) {
  if ($IsWindows) {
    $Vilens = Get-Item '.\Publish\Vilens.Tool.exe'
  }
  else {
    $Vilens = Get-Item '.\Publish\Vilens.Tool'
  }
}
else {
  $Vilens = Get-Item '.\Publish\Vilens.Tool.dll'
}

if (-not $NoSelfObfuscate -and (-not $Aot)) {
  $VilensDll = Get-Item '.\Publish\Vilens.dll'
  $VilensToolDll = Get-Item '.\Publish\Vilens.Tool.dll'
  dotnet run --project ./src/Vilens.Tool -c $Configuration -f $Framework -- $VilensDll --scope 'Auto' --features $Features | Out-Host
  dotnet run --project ./src/Vilens.Tool -c $Configuration -f $Framework -- $VilensToolDll --scope 'Public' --features $Features | Out-Host
}

Write-Output $Vilens
