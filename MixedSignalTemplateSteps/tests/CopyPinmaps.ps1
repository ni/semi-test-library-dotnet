# Copy the Corresponding Pinmaps and User-Defined Calibration Data
param (
    [string] $TargetPath,

    [switch] $Help
)


$Tester_Name =  $env:COMPUTERNAME

function copy_Pinmaps([string] $TargetPath)
{
    $Pinmaps_Path_Target = Join-Path $TargetPath "Supporting Materials\Pin Maps"
    $Pinmaps_Path = Join-Path $Pinmaps_Path_Target "$Tester_Name\*"
    
    if (Test-Path -Path $Pinmaps_Path)
    {
        Copy-Item $Pinmaps_Path $Pinmaps_Path_Target -Force
    }
    Else
    {
         Write-Host "`nWarning: $Pinmaps_Path not exists" -ForegroundColor Yellow
    }
}


try
{
    $helpText = "Syntax: $PSCommandPath [-TargetPath path]`n" +
            "- Specify the Tagert Testing Folder. (e.g. -TargetPath 'RootPath\MixedSignalSteps\MixedSignalTemplateSteps\tests')"
   
    if ($Help)
    {
        Write-Host $helpText

        return
    }

    copy_Pinmaps $TargetPath
}
catch
{   
    Write-Host "`nError Occurred:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red

    return "[copy_PinmapsandCalData]`n$($_.Exception.Message)`n"
}
