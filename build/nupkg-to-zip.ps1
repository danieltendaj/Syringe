# ===============================================================================
# 1. Creates .zip files from the nupkg files
# 2. Removes all un-needed folders from the zip files.
#
# Note: This script should be run from the root directory, not from the build folder.
# ===============================================================================
$root              = $PSScriptRoot;
$octopackTargetDir = "$root\_deploymentOutput\";

# Copy all Octopack nupkg files to zips
$nupkgFiles = dir "$octopackTargetDir/*.nupkg";
foreach ($file in $nupkgFiles)
{
    $newFilename = $file.FullName.Replace(".nupkg", ".zip");
    cp $file $newFilename -force;

    Write-Host "Copied $file to $newFilename"
}

# Clean up the zip files to remove un-needed nuget metadata folders and guff
[Reflection.Assembly]::LoadWithPartialName('System.IO.Compression') | out-null

$zipFiles = dir "$octopackTargetDir/*.zip";
$filesToRemove = "_rels/", "package/", "[Content_Types].xml", "Web.Debug.config", "Web.Release.config", 
                 "Syringe.Service.nuspec", "Syringe.Web.nuspec";

foreach ($zipFile in $zipFiles)
{
    Write-Host "Cleaning $zipfile" -ForegroundColor Green;

    $stream = New-Object IO.FileStream($zipfile.FullName, [IO.FileMode]::Open, [IO.FileAccess]::ReadWrite, [IO.FileShare]::ReadWrite)
    $mode   = [IO.Compression.ZipArchiveMode]::Update
    $zip    = New-Object IO.Compression.ZipArchive($stream, $mode)

    $toDelete = @();
    foreach($item in $zip.Entries)
    {
        $fullName = $item.FullName;

        foreach ($nameToRemove in $filesToRemove)
        {
            if ($fullName.ToLower().Contains($nameToRemove.ToLower()))
            {
                $toDelete += $item;
            }    
        }
    }

    foreach($item in $toDelete)
    {
        $item.Delete();
        Write-Host "  removed $($item.FullName)";
    }

    $zip.Dispose()
    $stream.Close()
    $stream.Dispose()
}