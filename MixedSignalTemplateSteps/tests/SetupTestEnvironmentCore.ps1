# Prepare Environment
$MSStepsDotNETTargetDirectory = "$PSScriptRoot\Tests.TSMMixedSignalStepTemplates\Integration\Legacy"
$MSTestLibraryName = "NationalInstruments.TSMMixedSignalTestLibrary.dll"

$TestCategories = ("Function", "Performance", "Stress")
foreach ($TestCategory in $TestCategories)
{
    # Prepare mixed-signal .net step templates
	$MSStepsDotNETAssemblyTargetDirectory = "$MSStepsDotNETTargetDirectory\$TestCategory\Code Modules\Mixed Signal Tests\DotNET"
    $MSStepsDotNETAssemblyTargetPath = "$MSStepsDotNETAssemblyTargetDirectory\TSMMixedSignalStepTemplates.dll"
	New-Item -ItemType File -Path $MSStepsDotNETAssemblyTargetPath -Force 
	Copy-Item -Force -Path "$MSStepsDotNETSourceDirectory\net48\$MSStepsAssemblyName" -Destination $MSStepsDotNETAssemblyTargetPath
	Copy-Item -Force -Path "$MSStepsDotNETSourceDirectory\net48\$MSTestLibraryName" -Destination $MSStepsDotNETAssemblyTargetDirectory\$MSTestLibraryName
	New-Item -ItemType SymbolicLink -Force -Path "$MSStepsDotNETTargetDirectory\$TestCategory\Supporting Materials" -Value "$PSScriptRoot\Supporting Materials"
}

& $PSScriptRoot\CopyPinmaps.ps1 -TargetPath $PSScriptRoot