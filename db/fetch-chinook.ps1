$ErrorActionPreference = "Stop"
$url = "https://raw.githubusercontent.com/lerocha/chinook-database/master/ChinookDatabase/DataSources/Chinook_SqlServer.sql"
$output = Join-Path $PSScriptRoot "Chinook_SqlServer.sql"

Invoke-WebRequest -Uri $url -OutFile $output
Write-Host "Saved Chinook SQL Server script to $output"
