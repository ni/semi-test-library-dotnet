param(
    [System.String]$OfflineModeFilePath
)
# Set Offline Mode
if ($OfflineModeFilePath -eq "") 
{ 
    $OfflineModeFilePath = "MixedSignal.offlinecfg"
}
    
."C:\Program Files (x86)\National Instruments\Shared\OfflineMode\NationalInstruments.Semiconductor.OfflineModeAPITool.exe" /enter $OfflineModeFilePath
