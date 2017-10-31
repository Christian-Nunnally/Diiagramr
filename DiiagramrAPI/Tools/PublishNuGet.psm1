
function Publish-NuGet($packageName) {
	Write-Host "Preparing to publish your NuGet $($packageName)"
	$nuspec = $packageName + ".nuspec"
	$csproj = $packageName + ".csproj"
	Write-Host "`n====== Creating File: $($nuspec) ======`n"
	Invoke-Expression "nuget spec $($csproj)"
	
	# replace content in the nuspec file before packing
	(Get-Content $nuspec).Replace(
		"<authors>`$author$</authors>", "<authors>$($env:USERNAME)</authors>"
		).Replace(
			"<description>`$description$</description>", "<description>No description</description>"
		).Replace(
			"    <licenseUrl>http://LICENSE_URL_HERE_OR_DELETE_THIS_LINE</licenseUrl>", ""
		).Replace(
			"    <projectUrl>http://PROJECT_URL_HERE_OR_DELETE_THIS_LINE</projectUrl>", ""
		).Replace(
			"    <iconUrl>http://ICON_URL_HERE_OR_DELETE_THIS_LINE</iconUrl>", ""
		).Replace(
			"<releaseNotes>Summary of changes made in this release of the package.</releaseNotes>",
			"<releaseNotes>No release notes.</releaseNotes>"
		).Replace(
			"<tags>Tag1 Tag2</tags>", "<tags>Diiagramr</tags>"
		) | Set-Content $nuspec
	
	# pack the project into a .nupkg
	Write-Host "`n====== Packing project into NuGet Package ======`n"
	Invoke-Expression "nuget pack $($csproj)"

	# get the full file name of the .nupkg
	$nupkg = (Get-ChildItem -Filter "*.nupkg").Fullname

	# publish the NuGet package to repo
	Write-Host "`n====== Publishing $($nupkg) to Diiagramr Repo ======`n"
	Invoke-Expression "nuget push $($nupkg) -Source http://diiagramrlibraries.azurewebsites.net/nuget/"
}

Export-ModuleMember Publish-NuGet