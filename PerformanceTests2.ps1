#Requires -Version 7.4

[CmdletBinding()]
param (
  [Parameter()]
  [switch]$MSBuild,

  [Parameter()]
  [ValidateRange(1, [int]::MaxValue)]
  [int]$Iterations = 25
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 3.0

if (Test-Path 'publish') {
  Remove-Item 'publish/*' -Recurse -Force
}

$TFM = $MSBuild ? 'net472' : 'net8.0'

dotnet publish ./src/Vilens.MSBuild --output 'publish' -c 'Release' -f $TFM

$VilensTasksPath = Join-Path $PSScriptRoot 'publish\Vilens.MSBuild.dll'
$StrongNamingKey = Join-Path $PSScriptRoot "Vilens.snk"

$arguments = @('.\Tests\Performance\Test.proj', '-tl:off', '-v:m', '-clp:PerformanceSummary', "-p:VilensTasksPath=$VilensTasksPath", "-p:AssemblyOriginatorKeyFile=$StrongNamingKey", '-p:SignAssembly=true')
dotnet build $arguments -v:diag -m:1 | Out-File vilens.log

$Times = @()
for ($i = 1; $i -le $Iterations; $i++) {
  if ($MSBuild) {
    $Text = msbuild @arguments
  }
  else {
    $Text = dotnet build $arguments
  }
  $Text | Where-Object { $_ -match '\b(\d+) ms +CommitVilens\b' }
  $Time = $Matches[1] | ForEach-Object { [int]$_ }

  $Times += $Time

  Write-Progress -Activity "Running performance tests" -Status ("Iteration {0}/{1}" -f $i, $Iterations) -PercentComplete (($i / $Iterations) * 100)
}
Write-Progress -Activity "Running performance tests" -Completed
$Times | Measure-Object -AllStats

# 326 ms (dotnet.exe, Windows)
# 361 ms (dotnet.exe, Linux)
# 733 ms (MSBuild.exe)
