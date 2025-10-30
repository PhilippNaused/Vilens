#Requires -Version 7.4

[CmdletBinding()]
param (
  [Parameter()]
  [ValidateSet('Debug', 'Release')]
  [string]$Configuration = 'Debug'
)

Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

cspell lint $PSScriptRoot

dotnet build --configuration $Configuration
dotnet build .\Tests\Dummies\Dummies.slnx --configuration $Configuration

dotnet test --no-build --configuration $Configuration
