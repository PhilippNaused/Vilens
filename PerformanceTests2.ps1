#Requires -Version 7.4

[CmdletBinding()]
param (
  [Parameter()]
  [switch]$MSBuild,

  [Parameter()]
  [ValidateRange(1, [int]::MaxValue)]
  [int]$Iteration = 10
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 3.0

if (Test-Path 'publish') {
  Remove-Item 'publish/*' -Recurse -Force
}

dotnet publish ./src/Vilens.MSBuild --output 'publish' -c 'Release'

$VilensTasksPath = Join-Path $PSScriptRoot 'publish\Vilens.MSBuild.dll'

$arguments = @('.\Tests\Performance\Test.proj', '-tl:off', '-v:m', '-clp:PerformanceSummary', "-p:VilensTasksPath=$VilensTasksPath")
# dotnet build $arguments # uncomment to test dotnet build

$Times = @()
for ($i = 0; $i -lt $Iteration; $i++) {
  if ($MSBuild) {
    $Text = msbuild @arguments
  }
  else {
    $Text = dotnet build $arguments
  }
  $Text | Where-Object { $_ -match '\b(\d+) ms +CommitVilens\b' }
  $Time = $Matches[1] | ForEach-Object { [int]$_ }

  $Times += $Time

  Write-Progress -Activity "Running performance tests" -Status ("Iterations remaining: {0}" -f ($Iteration - $i)) -PercentComplete (($i / $Iteration) * 100)
}
Write-Progress -Activity "Running performance tests" -Completed
$Times | Measure-Object -AllStats

# 338 ms (dotnet.exe, Windows)
# 394 ms (dotnet.exe, Linux)
# 740 ms (MSBuild.exe)
