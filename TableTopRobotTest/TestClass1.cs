using System;
using TableTopRobot.Models;
using Xunit;

namespace TableTopRobotTest
{
    public class TestClass1
    {
        [Fact]
        public void SetPreviousValuesTest()
        {
            string expected = "Meow!";
            PreviousMovements _previousMovements = new PreviousMovements();
            var x = 0;
            var y = 0;
            var f = FacingDirection.North;
            _previousMovements.SetPreviousValues(x, y, f);
            var actual = string.Format("{0},{1},{2}", _previousMovements.X, _previousMovements.Y, _previousMovements.F.ToString());
            Assert.NotEqual(expected, actual);

        }
}
}
