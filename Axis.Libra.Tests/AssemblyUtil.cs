using System.Reflection;

namespace Axis.Libra.Tests
{
    public static class AssemblyUtil
    {
        public static readonly string SampleAssemblyName = "Axis.Libra.Tests.SampleAssembly";

        public static Assembly GetTestCQRsAssembly()
        {
            return Assembly.Load(new AssemblyName($"Axis.Libra.Tests.TestCQRs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
        }
    }
}
