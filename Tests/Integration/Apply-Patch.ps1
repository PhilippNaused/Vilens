<#
.SYNOPSIS
  Applies a git patch to a project
#>
[CmdletBinding()]
param (
  # The name of the project to patch
  [Parameter(Mandatory)]
  [string]$TargetName,

  [Parameter()]
  [switch]$IgnoreMissingPatch,

  [Parameter()]
  [switch]$Clean
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 3.0

$TargetDir = Join-Path $PSScriptRoot $TargetName
$PatchFile = Join-Path $PSScriptRoot "$TargetName.patch"

if (-not (Test-Path $PatchFile)) {
  if ($IgnoreMissingPatch) {
    return
  }

  throw "Patch file '$PatchFile' does not exist"
}

if ($Clean) {
  git -C $TargetDir clean -xdf
  git -C $TargetDir restore .
}
Write-Output "Applying $PatchFile"
git -C $TargetDir apply --ignore-space-change --binary $PatchFile
