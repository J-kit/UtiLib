using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtiLib.Extensions;
using UtiLib.IO;

namespace UtiLib.ConsoleTests.Tests
{
    internal class InterfaceForceTest
    {
        public static void TestForceInterface()
        {
            TestR();
            var instance = new TestClass();

            var fake = instance.ForceCast<ITest>();

            Console.WriteLine(fake.IsEven(4));
            Console.WriteLine(fake.IsEven(5));
            Debugger.Break();
        }

        private static void TestR()
        {
            var resa = Test();

            var pres = new R<TestClass>();

            pres = new TestClass() { Bb = "Yoooo" };

            //TestClass res = pres;
            Debugger.Break();
        }

        public static R<TestClass> Test()
        {
            return new TestClass { Bb = "Yoooo" };
        }

        internal interface ITest
        {
            bool IsEven(int num);
        }

        internal class TestClass : ITest
        {
            public string Bb { get; set; }

            public bool IsEven(int num)
            {
                return num % 2 == 0;
            }
        }
    }
}