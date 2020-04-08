using System;
using Xunit;
using console_jogger;
using Xunit.Abstractions;
using System.Globalization;
using System.Threading;
using System.Linq;

namespace console_jogger.Tests
{
    public class PolarPositionTests
    {

        private readonly ITestOutputHelper output;

        public PolarPositionTests(ITestOutputHelper output)
    {
        this.output = output;
        const string culture = "en-US";
        CultureInfo ci = CultureInfo.GetCultureInfo(culture);
        Thread.CurrentThread.CurrentCulture = ci;
    }


        [Fact]
        public void Circle3D_data(){
            var circle = new Circle3D{x=200,y=0,z=200,radius=50};
            output.WriteLine(circle.GetPositionAtAngle(0));
            output.WriteLine(circle.GetPositionAtAngle(90));
            output.WriteLine(circle.GetPositionAtAngle(180));
            circle = new Circle3D{x=200,y=200,z=200,radius=50};
            
            foreach(int i in Enumerable.Range(1,360).ToArray()){
                output.WriteLine(circle.GetPositionAtAngle(i));
            }
        }
        

        [Fact]
        public void AngleProperty_xyzToAngle_Translate()
        {
            var polar = new PolarPosition{x=100,y=100,z=5};
            Assert.Equal(45,polar.Angle);
            polar = new PolarPosition{x=100,y=0,z=5};
            Assert.Equal(0,polar.Angle);
            polar = new PolarPosition{x=0,y=40,z=5};
            Assert.Equal(90,polar.Angle);
            polar = new PolarPosition{x=100,y=-100,z=5};
            Assert.Equal(-45,polar.Angle);
        }
    }
}
