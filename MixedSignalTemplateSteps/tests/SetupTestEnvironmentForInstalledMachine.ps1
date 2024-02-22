# Setup Testing Environment for Dev Machines -- Templates from Installer
#
# Usage: Powershell -File SetupTestEnvironmentForInstalledMachine.ps1

$MSStepsDotNETSourceDirectory = "$env:TestStand64\Components\Modules\NI_SemiconductorModule\StepTypeTemplates\DotNET\Mixed Signal Tests"
$MSStepsAssemblyName = "Template - TSMMixedSignalStepTemplates.dll"

& "$PSScriptRoot\SetupTestEnvironmentCore.ps1"