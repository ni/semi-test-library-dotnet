using System.Reflection;
using NationalInstruments.SemiconductorTestLibrary.TestStandSteps;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.SignatureCheck
{
    public partial class SetupAndCleanupSignatureTests
    {
        [Fact]
        public void CleanupInstrumentation_HasCorrectSignature()
        {
            // Retrieve method
            var method = typeof(SetupAndCleanupSteps).GetMethod(
                "CleanupInstrumentation",
                BindingFlags.Public | BindingFlags.Static);

            Assert.NotNull(method);

            // Get parameters
            var p = method.GetParameters();

            // ---- Parameter Count ----------------------------------------------------
            Assert.Equal(3, p.Length);

            // -------------------------------------------------------------------------
            // Parameter 1: tsmContext
            // -------------------------------------------------------------------------
            Assert.Equal("tsmContext", p[0].Name);
            Assert.Equal(typeof(ISemiconductorModuleContext), p[0].ParameterType);
            Assert.False(p[0].IsOptional);

            // -------------------------------------------------------------------------
            // Parameter 2: resetDevice  (bool, default = false)
            // -------------------------------------------------------------------------
            Assert.Equal("resetDevice", p[1].Name);
            Assert.Equal(typeof(bool), p[1].ParameterType);
            Assert.True(p[1].IsOptional);
            Assert.Equal(false, p[1].DefaultValue);

            // -------------------------------------------------------------------------
            // Parameter 3: instrumentType  (NIInstrumentType, default = NIInstrumentType.All)
            // -------------------------------------------------------------------------
            Assert.Equal("instrumentType", p[2].Name);
            Assert.Equal(typeof(NIInstrumentType), p[2].ParameterType);
            Assert.True(p[2].IsOptional);
            Assert.Equal(NIInstrumentType.All, p[2].DefaultValue);

            // ---- Return Type --------------------------------------------------------
            Assert.Equal(typeof(void), method.ReturnType);
        }
    }
}