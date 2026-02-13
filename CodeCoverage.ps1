#Requires -Version 7.4

[CmdletBinding()]
param (
  # Opens the html report (in your default browser).
  [Parameter()]
  [switch]$Open
)

Set-StrictMode -Version 3.0
$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

$ReportDir = 'coverage-report'
$CoverageDir = 'coverage'
$DocsDir = '.\docs\coverage'

# remove old coverage data and reports
Remove-Item $CoverageDir -Recurse -ErrorAction Ignore
Remove-Item $ReportDir -Recurse -ErrorAction Ignore

New-Item -Type Directory $CoverageDir -ErrorAction Ignore

dotnet build
dotnet build .\Tests\Dummies\Dummies.slnx

dotnet test --no-build --coverage --coverage-output-format 'xml' --results-directory $CoverageDir --coverage-settings 'CodeCoverage.xml' -p:PublishAot=false

# create the report
dotnet tool restore
dotnet ReportGenerator -Reports:"$CoverageDir/*" -TargetDir:"$ReportDir" -ReportTypes:'HtmlInline;MarkdownSummaryGithub;Badges'
Get-Content (Join-Path $ReportDir 'SummaryGithub.md') | Select-String -NotMatch 'Feature is only available for sponsors|Generated on' | Set-Content (Join-Path $DocsDir 'SummaryGithub.md')
# cspell:ignore linecoverage
Copy-Item (Join-Path $ReportDir 'badge_linecoverage.svg') -Destination $DocsDir -Force

if ($Open) {
  Invoke-Item (Join-Path $ReportDir 'index.html')
}
