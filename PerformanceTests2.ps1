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

$PublishDir = Join-Path $PSScriptRoot 'publish'

if (Test-Path $PublishDir) {
  Remove-Item $PublishDir/* -Recurse -Force
}

dotnet build ./src/Vilens.MSBuild -c 'Release' -t:PublishAll -p:PublishDir=$PublishDir

$StrongNamingKey = Join-Path $PSScriptRoot "Vilens.snk"

if ($MSBuild) {
  $MSBuildVersion = msbuild -version -noLogo
  Write-Output "Using MSBuild version: $MSBuildVersion"
}

$Project = Get-Item '.\Tests\Performance\Test.proj'

if ($MSBuild) {
  msbuild $Project -t:Restore
}
else {
  dotnet restore $Project
}

$arguments = @($Project, '-tl:off', '-v:m', '-m:1', '-clp:PerformanceSummary', "-p:AssemblyOriginatorKeyFile=$StrongNamingKey", '-p:SignAssembly=true', '-p:Configuration=Release')
# dotnet build $arguments -v:diag | Out-File vilens.log

$Times = @()
for ($i = 1; $i -le $Iterations; $i++) {
  if ($MSBuild) {
    $Text = msbuild @arguments
  }
  else {
    $Text = dotnet build $arguments --no-restore
  }
  $Text | Where-Object { $_ -match '\b(\d+) ms +CommitVilens\b' }
  $Time = $Matches[1] | ForEach-Object { [int]$_ }

  $Times += $Time

  $Stats = $Times | Measure-Object -AllStats
  Write-Progress -Activity "Running performance tests" -Status ("{0}±{1} ms" -f [int]$Stats.Average, [int]$Stats.StandardDeviation) -PercentComplete (($i / $Iterations) * 100)
}
Write-Progress -Activity "Running performance tests" -Completed
$Times | Measure-Object -AllStats

# 314 ms (dotnet.exe, Windows)
# 345 ms (dotnet.exe, Linux)
# 661 ms (MSBuild.exe)
