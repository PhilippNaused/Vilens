[CmdletBinding()]
param (
  # The name of the project to update
  [Parameter(Mandatory)]
  [string]$TargetName,

  # The name of the branch to update
  [Parameter(Mandatory)]
  [string]$BranchName
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 3.0

$TargetDir = Join-Path $PSScriptRoot $TargetName

$Script = Join-Path $PSScriptRoot Apply-Patch.ps1
& $Script $TargetName -IgnoreMissingPatch -Clean

git -C $TargetDir stash push -m "Stash before update"
git -C $TargetDir checkout $BranchName
git -C $TargetDir pull --ff-only
git -C $TargetDir stash pop

$Script = Join-Path $PSScriptRoot Create-Patch.ps1
& $Script $TargetName
