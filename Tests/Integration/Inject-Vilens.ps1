#Requires -Version 7.4

[CmdletBinding()]
param (
  [Parameter(Mandatory)]
  [System.IO.FileInfo]$TargetFile,

  [Parameter(Mandatory)]
  [string]$Features
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 3.0

$InjectedXml = @"
  <ItemGroup>
    <VilensFeatures Include="$Features" />
  </ItemGroup>
"@

[xml]$xml = [xml]::new()
$xml.PreserveWhitespace = $true
$xml.Load($TargetFile)
$xml.Project.InnerXml += $InjectedXml
$xml.Project.InnerXml += "`n`n"
$xml.Save($TargetFile)
