﻿
function Publish-NuGet {
	$csproj = (Get-ChildItem -Filter "*.csproj").Name
	Write-Host "Preparing to publish your NuGet $($csproj)"

	$nuspec = (Get-ChildItem -Filter "*.nuspec").Name
	if ($nuspec) {
		Write-Host "`nFile $($nuspec) already exists... delete before publishing." -ForegroundColor "red"
		return
	}

	Write-Host "`n====== Creating File: $($nuspec) ======`n" -ForegroundColor "yellow"
	Invoke-Expression "nuget spec $($csproj)"

	# Get generated .nuspec file.
	$nuspec = (Get-ChildItem -Filter "*.nuspec").Name
	if (!$nuspec) {
		Write-Host "`nError creating `.nuspec` file for project. View output above..." -ForegroundColor "red"
		return
	}
	
	# Get requested `<authors>` field.
	Write-Host "`nEnter Authors or keep default [" -NoNewline -ForegroundColor "cyan"
	Write-Host "$($env:username)" -NoNewline -ForegroundColor "green"
	Write-Host "]: " -NoNewline -ForegroundColor "cyan"
	$authors = Read-Host
	if (!$authors) { $authors = $env:username }

	# Get requested `<description>` field.
	Write-Host "`nEnter a description or keep default [" -NoNewline -ForegroundColor "cyan"
	Write-Host "No description." -NoNewline -ForegroundColor "green"
	Write-Host "]: " -NoNewline -ForegroundColor "cyan"
	$description = Read-Host
	if (!$description) { $description = "No description." }

	# Get requested `<licenseUrl>` field if any.
	Write-Host "`nEnter a License URL if any: " -NoNewline -ForegroundColor "cyan"
	$licenseUrl = Read-Host
	if ($licenseUrl) { $licenseUrl = "    <licenseUrl>" + $licenseUrl + "</licenseUrl>" }

	# Get requested `<projectUrl>` field if any.
	Write-Host "`nEnter a Project URL if any: " -NoNewline -ForegroundColor "cyan"
	$projectUrl = Read-Host
	if ($projectUrl) { $projectUrl = "    <projectUrl>" + $projectUrl + "</projectUrl>" }

	# Get requested `<iconUrl>` field if any.
	Write-Host "`nEnter an Icon URL if any: " -NoNewline -ForegroundColor "cyan"
	$iconUrl = Read-Host
	if ($iconUrl) { $iconUrl = "    <iconUrl>" + $iconUrl + "</iconUrl>" }

	# Get requested `<releaseNotes>` field.
	Write-Host "`nEnter Release Notes or keep default [" -NoNewline -ForegroundColor "cyan"
	Write-Host "No release notes." -NoNewline -ForegroundColor "green"
	Write-Host "]: " -NoNewline -ForegroundColor "cyan"
	$releaseNotes = Read-Host
	if (!$releaseNotes) { $releaseNotes = "No release notes." }

	# Get requested `<tags>` field.
	Write-Host "`nEnter space-separated tags or keep default [" -NoNewline -ForegroundColor "cyan"
	Write-Host "Diiagramr" -NoNewline -ForegroundColor "green"
	Write-Host "]: " -NoNewline -ForegroundColor "cyan"
	$tags = Read-Host
	if (!$tags) { $tags = "Diiagramr" }

	# replace content in the nuspec file before packing
	(Get-Content $nuspec).Replace(
		"<authors>`$author$</authors>", "<authors>$($authors)</authors>"
		).Replace(
			"<description>`$description$</description>", "<description>$($description)</description>"
		).Replace(
			"    <licenseUrl>http://LICENSE_URL_HERE_OR_DELETE_THIS_LINE</licenseUrl>", "$($licenseUrl)"
		).Replace(
			"    <projectUrl>http://PROJECT_URL_HERE_OR_DELETE_THIS_LINE</projectUrl>", "$($projectUrl)"
		).Replace(
			"    <iconUrl>http://ICON_URL_HERE_OR_DELETE_THIS_LINE</iconUrl>", "$($iconUrl)"
		).Replace(
			"<releaseNotes>Summary of changes made in this release of the package.</releaseNotes>",
			"<releaseNotes>$($releaseNotes)</releaseNotes>"
		).Replace(
			"<tags>Tag1 Tag2</tags>", "<tags>$($tags)</tags>"
		) | Set-Content $nuspec
	
	# pack the project into a .nupkg
	Write-Host "`n====== Packing project into NuGet Package ======`n" -ForegroundColor "yellow"
	Invoke-Expression "nuget pack $($csproj)"

	# get the full file name of the .nupkg
	$nupkg = (Get-ChildItem -Filter "*.nupkg").Name
	if (!$nupkg) {
		# NuGet Package wasn't created successfully
		Write-Host "`nError creating NuGet Package. View output above..." -ForegroundColor "red"
		return
	}
	
	# publish the NuGet package to repo
	Write-Host "`n====== Publishing $($nupkg) to Diiagramr Repo ======`n" -ForegroundColor "yellow"
	Invoke-Expression "nuget push $($nupkg) -Source http://diiagramrlibraries.azurewebsites.net/nuget/"
}

Export-ModuleMember Publish-NuGet