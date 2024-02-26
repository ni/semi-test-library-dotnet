# Setup Testing Environment for Dev Machines -- Templates from Source Code
#
# Usage: Powershell -File SetupTestEnvironmentForDevMachine.ps1

$MSStepsDotNETSourceDirectory = "$PSScriptRoot\..\source\DotNET\TSMMixedSignalStepTemplates"
$MSStepsAssemblyName = "TSMMixedSignalStepTemplates.dll"

& "$PSScriptRoot\SetupTestEnvironmentCore.ps1"