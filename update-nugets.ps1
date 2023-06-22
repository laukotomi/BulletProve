nuget locals all -list | 
foreach-object { $_ -split ": " | select-object -Skip 1 } |
Where-Object { Test-Path -Path $_ } |
Get-ChildItem -Directory |
Where-Object { $_.Name.StartsWith("bulletprove") } |
Remove-Item -Recurse -Force


$projectPath = Join-Path -Path $PSScriptRoot -ChildPath "example\Example.Api.IntegrationTests\Example.Api.IntegrationTests.csproj"
$source = Join-Path -Path $PSScriptRoot -ChildPath "Build"
nuget restore $projectPath -DirectDownload -Force -NoCache -Source $source