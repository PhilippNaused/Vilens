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
dotnet test --no-build --configuration $Configuration

$slnx = Get-Item '.\Tests\Dummies\Dummies.slnx'
dotnet build $slnx --configuration $Configuration
dotnet test --solution $slnx --no-build --configuration $Configuration
