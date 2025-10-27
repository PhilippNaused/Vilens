#Requires -Version 7.4

[CmdletBinding()]
param (
  # The name of file to test
  [Parameter()]
  [ValidateSet('nunit.framework')]
  [string]$Target = 'nunit.framework',

  [Parameter()]
  [ValidateSet('Debug', 'Release')]
  [string]$Configuration = 'Release',

  [Parameter()]
  [ValidateSet('Private', 'Internal', 'Public')]
  [string]$Scope = 'Internal',

  [Parameter()]
  [string]$Features = 'All',

  [Parameter()]
  [switch]$NoSelfObfuscate,

  [Parameter()]
  [switch]$Aot,

  [ValidateSet('net9.0', 'net10.0')]
  [Parameter()]
  [string]$Framework = 'net10.0',

  [Parameter()]
  [switch]$Coverage
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 3.0

$ReportDir = '.\coverage-report'

if ($Coverage) {
  # stop the build server if it is running
  dotnet build-server shutdown
  # restore dotnet-coverage and ReportGenerator
  dotnet tool restore
  # remove old coverage data and reports
  Remove-Item .\coverage -Recurse -ErrorAction Ignore
  Remove-Item $ReportDir -Recurse -ErrorAction Ignore

  $NoSelfObfuscate = $true
  $Configuration = 'Debug'
}

$SourceFile = Join-Path $PSScriptRoot ".\Tests\Performance\TestAssemblies\$Target.dll"
$SourcePdb = Join-Path $PSScriptRoot ".\Tests\Performance\TestAssemblies\$Target.pdb"

$TempFolder = Join-Path $PSScriptRoot ".\Tests\Performance\Temp"
if (-not (Test-Path $TempFolder)) {
  New-Item $TempFolder -ItemType Directory
}
else {
  Remove-Item $TempFolder\* -Recurse
}

$VilensConsole = .\Publish.ps1 -Configuration $Configuration -NoSelfObfuscate:$NoSelfObfuscate -Aot:$Aot -Framework $Framework

if ($Coverage) {
  $SessionId = [Guid]::NewGuid().ToString()
  dotnet dotnet-coverage instrument $VilensConsole --session-id $SessionId
  $dir = Split-Path $VilensConsole
  $VilensDll = Join-Path $dir "Vilens.dll"
  dotnet dotnet-coverage instrument $VilensDll --session-id $SessionId
  dotnet dotnet-coverage collect --session-id $SessionId --server-mode --background --output coverage\IntegrationTests.xml --output-format cobertura
}

$Iteration = 25
if ($Coverage) {
  $Iteration = 1
}

try {
  $Times = @()
  for ($i = 0; $i -lt $Iteration; $i++) {
    $TargetFile = Copy-Item $SourceFile -Destination $TempFolder -PassThru -Force
    if (Test-Path $SourcePdb) {
      Copy-Item $SourcePdb -Destination $TempFolder -Force | Out-Null
    }
    $StartTime = Get-Date
    if ($Aot) {
      & $VilensConsole $TargetFile --scope $Scope --features $Features | Out-Null
    }
    else {
      & dotnet $VilensConsole $TargetFile --scope $Scope --features $Features | Out-Null
    }
    # skip the first iteration
    if ($i -gt 0) {
      $Times += ((Get-Date) - $StartTime)
    }
    Write-Progress -Activity "Running performance tests" -Status ("Iterations remaining: {0}" -f ($Iteration - $i)) -PercentComplete (($i / $Iteration) * 100)
  }
  Write-Progress -Activity "Running performance tests" -Completed
  $Times | Measure-Object -AllStats -Property TotalMilliseconds # ~790ms (~270 AOT)
}
finally {
  if ($Coverage) {
    # stop the background process
    dotnet dotnet-coverage shutdown $SessionId
    Start-Sleep -Seconds 2

    # create the report
    dotnet ReportGenerator '-reports:coverage\*.xml;coverage\*\*' -TargetDir:$ReportDir '-ReportTypes:HtmlInline;MarkdownSummaryGithub'
    Copy-Item (Join-Path $ReportDir 'SummaryGithub.md') -Destination (Join-Path $PSScriptRoot 'Coverage.md') -Force
  }
}
