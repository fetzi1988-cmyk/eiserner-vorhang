using EisernerVorhang.Core.Commands;
using EisernerVorhang.Core.Geometry;
using EisernerVorhang.Core.Simulation;
using EisernerVorhang.Core.Units;
using NUnit.Framework;

namespace EisernerVorhang.Tests
{
    public class UnitMovementTests
    {
        [Test]
        public void IntSqrt_IsExactForSquaresAndRoundsDown()
        {
            Assert.AreEqual(0, IntMath.Sqrt(0));
            Assert.AreEqual(1, IntMath.Sqrt(3));
            Assert.AreEqual(12, IntMath.Sqrt(144));
            Assert.AreEqual(12, IntMath.Sqrt(168));
            Assert.AreEqual(46340, IntMath.Sqrt(2147395600L));
        }

        [Test]
        public void Unit_ReachesTargetAndStops()
        {
            var world = new UnitWorld();
            var unit = world.Spawn(1, IntVector2.Zero, speedPerTick: 100);
            world.OrderMove(unit, new IntVector2(1000, 0));

            for (int i = 0; i < 9; i++)
                world.Tick();
            Assert.AreEqual(new IntVector2(900, 0), unit.Position);
            Assert.IsTrue(unit.IsMoving);

            world.Tick();
            Assert.AreEqual(new IntVector2(1000, 0), unit.Position);
            Assert.IsFalse(unit.IsMoving);
        }

        [Test]
        public void Formation_GivesEveryUnitItsOwnSlot()
        {
            var slots = Formation.Grid(IntVector2.Zero, 7, 500);

            Assert.AreEqual(7, slots.Count);
            CollectionAssert.AllItemsAreUnique(slots);
        }

        [Test]
        public void MoveCommand_OnlyMovesOwnUnits()
        {
            var sim = new GameSimulation();
            var own = sim.World.Spawn(1, IntVector2.Zero, 100);
            var enemy = sim.World.Spawn(2, IntVector2.Zero, 100);

            sim.Submit(new GameCommand(1, sim.NextTick, CommandType.Move,
                new[] { own.Id, enemy.Id }, 5000, 0));
            sim.Step();

            Assert.IsTrue(own.IsMoving);
            Assert.IsFalse(enemy.IsMoving);
        }

        [Test]
        public void SameCommands_GiveSameResult()
        {
            IntVector2 Run()
            {
                var sim = new GameSimulation();
                var unit = sim.World.Spawn(1, IntVector2.Zero, 67);
                sim.Submit(new GameCommand(1, 1, CommandType.Move, new[] { unit.Id }, 3333, -7777));
                for (int i = 0; i < 50; i++)
                    sim.Step();
                return unit.Position;
            }

            Assert.AreEqual(Run(), Run());
        }
    }
}
