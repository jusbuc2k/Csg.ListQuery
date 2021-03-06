#!/usr/bin/env powershell
#requires -version 4
#
# CSG Build Script
# Copyright 2017 Cornerstone Solutions Group
Param(
	[alias("c")][string]
	$Configuration = "Release",
	[string]
	$BuildToolsVersion = "1.0-latest",
	[switch]
	$NoTest,
	[switch]
	$NoPackage,
	[string]
	$PullRequestNumber="",
	[string]
	$TestLogger = "trx;logfilename=TEST-$(get-date -format yyyyMMddHHmmss).trx"
)
. "$PSScriptRoot/bootstrap.ps1"	

$Solution =  "$(Get-Item -Path *.sln | Select-Object -First 1)"
$PackageProjects = @(
	".\src\Csg.ListQuery\Csg.ListQuery.csproj",
	".\src\Csg.ListQuery.Sql\Csg.ListQuery.Sql.csproj",
	".\src\Csg.ListQuery.Server.Abstractions\Csg.ListQuery.Server.Abstractions.csproj",
	".\src\Csg.ListQuery.Client.Abstractions\Csg.ListQuery.Client.Abstractions.csproj",
	".\src\Csg.ListQuery.AspNetCore\Csg.ListQuery.AspNetCore.csproj"
)
$PublishProjects = @(
	#".\src\Web\Web.csproj"
)
$TestProjects = Get-Item -Path tests\**\*Tests.csproj | %{ $_.FullName }
$SkipPackage = $NoPackage.IsPresent

if ($PullRequestNumber) {
    Write-Host "Building for a pull request (#$PullRequestNumber), skipping packaging." -ForegroundColor Yellow
    $SkipPackage = $true
}

Write-Host "==============================================================================" -ForegroundColor DarkYellow
Write-Host "The Build Script for Csg.ListQuery"
Write-Host "==============================================================================" -ForegroundColor DarkYellow
Write-Host "Build Tools:`t$BuildToolsVersion"
Write-Host "Solution:`t$Solution"
Write-Host "Skip Tests:`t$NoTest"
Write-Host "Pull Req:`t$PullRequestNumber"
Write-Host "==============================================================================" -ForegroundColor DarkYellow

try {
	Get-BuildTools -Version $BuildToolsVersion | Out-Null

	# Uncomment if you need to use msbuild commands in this file
	#$msbuild = Find-MSBuild
	
	Write-Host "Restoring Packages..." -ForegroundColor Magenta
	
	dotnet restore $SOLUTION -v m
	# Comment above and uncomment below if you need to use msbuild directly (if you have sdk and legacy projects)
	# & $msbuild /t:Restore $SOLUTION

	if ($LASTEXITCODE -ne 0) {
		throw "Package restore failed with exit code $LASTEXITCODE."
	}

	Write-Host "Performing build..." -ForegroundColor Magenta	
	
	dotnet build $SOLUTION --configuration $Configuration -v m

	if ($LASTEXITCODE -ne 0) {
		throw "Build failed with exit code $LASTEXITCODE."
	}

	if ( !($NoTest.IsPresent) -and $TestProjects.Length -gt 0 ) {
		Write-Host "Performing tests..." -ForegroundColor Magenta
		foreach ($test_proj in $TestProjects) {
			Write-Host "Testing $test_proj"			
			dotnet test $test_proj --no-build --configuration $Configuration --logger $TestLogger
			if ($LASTEXITCODE -ne 0) {
				throw "Test failed with code $LASTEXITCODE"
			}
		}
	}

	if ( !($SkipPackage) -and $PackageProjects.Length -gt 0 ) {
		Write-Host "Packaging..."  -ForegroundColor Magenta
		foreach ($pack_proj in $PackageProjects){
			Write-Host "Packing $pack_proj"
			
			dotnet pack $pack_proj --no-build --configuration $Configuration
			
			if ($LASTEXITCODE -ne 0) {
				throw "Pack failed with code $result"
			}
		}
	}

	if ( !($SkipPackage) -and $PublishProjects.Length -gt 0 ) {
		Write-Host "Publishing..." -ForegroundColor Magenta
		foreach ($pub_proj in $PublishProjects){
			Write-Host "Publishing $pack_proj"
			
			dotnet publish $pub_proj --no-build --no-restore --configuration $Configuration
			
			if ($LASTEXITCODE -ne 0) {
				throw "Publish failed with code $result"
			}
		}
	}

	Write-Host "All Done. This build is great! (as far as I can tell)" -ForegroundColor Green
	exit 0
} catch {
	Write-Host "ERROR: An error occurred and the build was aborted." -ForegroundColor White -BackgroundColor Red
	Write-Error $_	
	exit 3
} finally {
	Remove-Module 'BuildTools' -ErrorAction Ignore
}
