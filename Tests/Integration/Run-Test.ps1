#Requires -Version 7.4

[CmdletBinding()]
param (
  # The name of the project to test
  [Parameter(Mandatory)]
  [string]$TargetName,

  [Parameter(Mandatory)]
  [string]$Version,

  [Parameter()]
  [ValidateSet('Debug', 'Release')]
  [string]$Configuration = 'Debug',

  [Parameter(Mandatory)]
  [string]$Features,

  [Parameter()]
  [switch]$NoCleanup
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
Set-StrictMode -Version 3.0

function Test-Obfuscation {
  param (
    # Command to build the project
    [Parameter()]
    [scriptblock]$BuildCommand = { dotnet build -c $Configuration -p:TreatWarningsAsErrors=false },

    # Command to run the unit tests
    [Parameter()]
    [scriptblock]$TestCommand = { dotnet test -c $Configuration },

    # Command that returns the .csproj files of the project that should be obfuscated
    [Parameter()]
    [scriptblock]$FileListCommand = { Get-ChildItem src -Recurse -Filter '*.csproj' -Exclude '*Test*', '*Benchmark*' },

    # Names of the files that should be obfuscated
    [Parameter(Mandatory)]
    [string[]]$FileNames
  )

  $InjectScript = Join-Path $PSScriptRoot Inject-Vilens.ps1

  $PatchScript = Join-Path $PSScriptRoot Apply-Patch.ps1

  $TestFolder = Join-Path $PSScriptRoot $TargetName

  Push-Location $TestFolder
  try {

    # Restore the repository
    git clean -xdf
    git restore .

    # Apply the patch
    & $PatchScript -TargetName $TargetName -IgnoreMissingPatch

    $NuGetSource = Join-Path $PSScriptRoot ..\..\Publish
    $NuGetSource = Get-Item $NuGetSource

    dotnet nuget add source $NuGetSource -n LocalVilens

    # Inject obfuscation
    $Files = & $FileListCommand
    if (-not $Files) {
      throw "Cannot find project file to inject"
    }
    foreach ($File in $Files) {
      dotnet add $File package Vilens.MSBuild --version $Version
      & $InjectScript -TargetFile $File -Features $Features
      Write-Output "Injected '$($File)'"
    }

    & $BuildCommand

    # Make sure that the obfuscation tag files are created to verify that the obfuscation was successful
    $FoundFiles = Get-ChildItem -Recurse -Filter '*.vilens.done' | Select-Object -ExpandProperty Name -Unique | ForEach-Object { $_.Substring(0, $_.Length - 12) }
    if (-not $FoundFiles) {
      throw "No obfuscated files found"
    }
    $Comp = Compare-Object -ReferenceObject $FileNames -DifferenceObject $FoundFiles -PassThru
    if ($Comp) {
      throw "Expected files: $($FileNames -join ', ')`nFound files: $($FoundFiles -join ', ')"
    }

    & $TestCommand
  }
  finally {
    # Reset the repository again
    if (-not $NoCleanup) {
      git restore .
      git clean -xdf
    }
    Pop-Location
    $NuGetConfig = Join-Path $PSScriptRoot NuGet.config
    git restore $NuGetConfig
    dotnet build-server shutdown
  }
}

Push-Location $PSScriptRoot
try {
  switch ($TargetName) {
    'Moq' {
      Test-Obfuscation -FileNames 'Moq.dll'
    }
    'Autofac' {
      Test-Obfuscation -FileNames 'Autofac.dll' -BuildCommand { dotnet build Autofac.sln -c $Configuration } -TestCommand { dotnet test Autofac.sln -c $Configuration --filter 'FullyQualifiedName !~ Benchmark' } # Benchmarks are too slow
    }
    'NSubstitute' {
      Test-Obfuscation -FileNames 'NSubstitute.dll'
    }
    'Serilog' {
      Test-Obfuscation -FileNames 'Serilog.dll' -TestCommand { dotnet test -c $Configuration --filter 'FullyQualifiedName !~ Performance' } # Benchmarks are too slow
    }
    'NUnit' {
      Test-Obfuscation -FileNames 'nunit.framework.dll', 'mock-assembly.dll', 'nunit.framework.legacy.dll', 'nunitlite.dll', 'nunitlite-runner.exe', 'nunitlite-runner.dll'
    }
    default {
      throw 'Unknown target'
    }
  }
}
finally {
  Pop-Location
}
