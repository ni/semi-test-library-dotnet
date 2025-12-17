#include "extcode.h"
#ifdef __cplusplus
extern "C" {
#endif

/*!
 * Closes FPGA reference
 */
int32_t __stdcall CloseFPGA(uint64_t ReferenceID);
/*!
 * Opens FPGA reference using bitfile and RIO device. 
 * Note: Bitfile should be built for the exact type of  RIO device.
 */
int32_t __stdcall OpenFPGA(char ResourceName[], char FPGABitFilePath[], 
	uint64_t *ReferenceID);
/*!
 * Enables loop back by turning on Loop back boolean
 */
int32_t __stdcall EnableLoopBack(uint64_t ReferenceID, 
	uint64_t EnableLoopback);
/*!
 * Write channel data to the specified channel
 */
int32_t __stdcall WriteData(uint64_t ReferenceID, char ChannelName[], 
	uint8_t ChannelData);
/*!
 * Reads data from a specific channel mentioned.
 */
int32_t __stdcall ReadData(uint64_t ReferenceID, char ChannelName[], 
	uint8_t *ChannelData);

MgErr __cdecl LVDLLStatus(char *errStr, int errStrLen, void *module);

void __cdecl SetExecuteVIsInPrivateExecutionSystem(Bool32 value);

#ifdef __cplusplus
} // extern "C"
#endif

