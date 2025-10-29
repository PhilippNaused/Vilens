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
dotnet test --configuration $Configuration

dotnet test --solution .\Tests\Dummies\Dummies.slnx --configuration $Configuration
