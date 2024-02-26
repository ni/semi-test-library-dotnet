using System.IO;
using Xunit;

namespace NationalInstruments.Tests.TSMMixedSignalStepTemplates.Integration.Legacy
{
    [Collection("NonParallelizable")]
    public class DotNETSequenceTests
    {
        [Theory]
        [InlineData(@"Mixed Signal Steps Tests.seq")]
        [InlineData(@"Mixed Signal Steps with RF Steps Tests.seq")]
        public void RunLegacyDotNETFunctionTest_ReturnsNoError(string sequenceFileName)
        {
            var sequenceFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\Integration\Legacy\Function", sequenceFileName);
            SequenceRunner.SequenceRunner.RunSequenceFileWithSemiOI(sequenceFilePath);
        }
    }
}
