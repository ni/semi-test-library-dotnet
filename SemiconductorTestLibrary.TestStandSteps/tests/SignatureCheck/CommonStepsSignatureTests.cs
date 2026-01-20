using System.Linq;
using System.Reflection;
using NationalInstruments.DAQmx;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.TestStandSteps;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.Utilities;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.SignatureCheck
{
    public class CommonStepsSignatureTests
    {
        [Fact]
        public void GetLeakageTestWithParameters_HasCorrectSignature()
        {
            var classType = typeof(CommonSteps);
            var parameterTypes = new[]
            {
                typeof(ISemiconductorModuleContext),
                typeof(string[]),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(double),
                typeof(bool)
            };
            var method = classType.GetMethod(
                "LeakageTest",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                parameterTypes,
                modifiers: null);

            Assert.NotNull(method);
            var parameters = method.GetParameters();
            AssertParameter(parameters[0], "tsmContext", false);
            AssertParameter(parameters[1], "pinsOrPinGroups", false);
            AssertParameter(parameters[2], "voltageLevel", false);
            AssertParameter(parameters[3], "currentLimit", false);
            AssertParameter(parameters[4], "apertureTime", false);
            AssertParameter(parameters[5], "settlingTime", false);
            AssertBoolParameter(parameters[6], "serialOperationEnabled", true, false);
            Assert.Equal(typeof(void), method.ReturnType);
        }

        [Fact]
        public void GetAllLeakageTestOverloads_HasSingleOverload()
        {
            var classType = typeof(CommonSteps);
            var overloads = classType.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(m => m.Name == "LeakageTest");

            Assert.Single(overloads);
        }
    }
}
