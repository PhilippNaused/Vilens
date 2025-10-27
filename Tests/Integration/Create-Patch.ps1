[CmdletBinding()]
param (
  # The name of the project to patch
  [Parameter(Mandatory)]
  [string]$TargetName,

  # Apply the current patch first
  [Parameter()]
  [switch]$Update
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 3.0

$TargetDir = Join-Path $PSScriptRoot $TargetName
$PatchFile = Join-Path $PSScriptRoot "$TargetName.patch"

if ($Update) {
  $Script = Join-Path $PSScriptRoot Apply-Patch.ps1
  & $Script $TargetName
}

git -C $TargetDir diff --binary -U2 HEAD > $PatchFile
# Delete file if empty
if (-not (Get-Content $PatchFile)) {
  Remove-Item $PatchFile
}
