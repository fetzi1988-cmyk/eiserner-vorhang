using EisernerVorhang.Core.Simulation;
using NUnit.Framework;

namespace EisernerVorhang.Tests
{
    public class SimulationClockTests
    {
        [Test]
        public void OneSecond_ProducesFifteenTicks()
        {
            var clock = new SimulationClock(maxTicksPerFrame: 100);
            int total = 0;
            for (int frame = 0; frame < 60; frame++)
                total += clock.Advance(1.0 / 60.0);

            Assert.That(total, Is.InRange(14, 15));
        }

        [Test]
        public void SmallFrames_AccumulateUntilATickIsDue()
        {
            var clock = new SimulationClock();
            Assert.AreEqual(0, clock.Advance(SimulationClock.SecondsPerTick * 0.6));
            Assert.AreEqual(1, clock.Advance(SimulationClock.SecondsPerTick * 0.6));
        }

        [Test]
        public void LongFrame_IsCappedAtMaxTicks()
        {
            var clock = new SimulationClock(maxTicksPerFrame: 5);
            Assert.AreEqual(5, clock.Advance(10.0));
            Assert.AreEqual(0, clock.Advance(0.0));
        }
    }
}
