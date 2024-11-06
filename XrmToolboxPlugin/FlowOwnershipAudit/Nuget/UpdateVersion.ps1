param (
    [string]$nuspecPath,
    [string]$assemblyInfoPath
)

# Load the .nuspec file
[xml]$nuspec = Get-Content $nuspecPath

# Get the current version
$currentVersion = $nuspec.package.metadata.version

# Split the version into major, minor, and build parts
$versionParts = $currentVersion -split '\.'
$major = [int]$versionParts[0]
$minor = [int]$versionParts[1]
$build = [int]$versionParts[2]

# Increment the build number
$build++

# Create the new version string
$newVersion = "$major.$minor.$build"

# Update the version in the .nuspec file
$nuspec.package.metadata.version = $newVersion

# Save the updated .nuspec file
$nuspec.Save($nuspecPath)

# Update the AssemblyInfo.cs file
(Get-Content $assemblyInfoPath) -replace '\[assembly: AssemblyVersion\(".*"\)\]', "[assembly: AssemblyVersion(`"$newVersion`")]" |
 Set-Content $assemblyInfoPath

(Get-Content $assemblyInfoPath) -replace '\[assembly: AssemblyFileVersion\(".*"\)\]', "[assembly: AssemblyFileVersion(`"$newVersion`")]" |
 Set-Content $assemblyInfoPath

Write-Output "Updated version to $newVersion"
