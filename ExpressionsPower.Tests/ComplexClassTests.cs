using System;
using Xunit;

namespace ExpressionsPower.Tests
{
    public class ComplexClassTests
    {
        private class SimpleClass
        {
            public decimal D3 { get; set; }

            public string Str2 { get; set; }
        }

        private class ComplexClass
        {
            public decimal D1 { get; set; }

            public decimal D2 { get; set; }

            public string Str1 { get; set; }

            public SimpleClass InnerClass { get; set; }
        }

        private static decimal Modify(decimal value)
        {
            return value % 1 == 0
                ? Math.Round(value)
                : value;
        }

        [Fact]
        public void ModifyProperties_ShouldRoundAllIntegerDecimals_InComplesClass()
        {
            var obj = new ComplexClass
            {
                D1 = 1.5M,
                D2 = 10.0M,
                Str1 = "mystr",
                InnerClass = new SimpleClass
                {
                    D3 = 2.0M,
                    Str2 = "innerstr"
                }
            };

            var results = PropertiesModifier.ModifyProperties<ComplexClass, decimal>(Modify);

            foreach (var result in results)
            {
                result.Invoke(obj);
            }

            Assert.Equal("1.5", obj.D1.ToString());
            Assert.Equal("10", obj.D2.ToString());
            Assert.Equal("2", obj.InnerClass.D3.ToString());
        }
    }
}
