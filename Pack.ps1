#Requires -Version 7.4

[CmdletBinding()]
param (
  [Parameter()]
  [string]$Version = "0.0.0-Dev.$(Get-Date -Format 'yyyyMMddHHmmss')",

  [Parameter()]
  [ValidateSet('Debug', 'Release')]
  [string]$Configuration = 'Release'
)

Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

if (Test-Path 'publish') {
  Remove-Item 'publish/*' -Recurse -Force
}

dotnet pack .\src\Vilens.MSBuild\ "-p:Version=$Version" --output publish -c $Configuration
