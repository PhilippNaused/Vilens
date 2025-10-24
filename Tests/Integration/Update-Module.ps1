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

git -C $TargetDir switch $BranchName --merge
git -C $TargetDir pull --ff-only --autostash
git -C $TargetDir add .

$Script = Join-Path $PSScriptRoot Create-Patch.ps1
& $Script $TargetName
