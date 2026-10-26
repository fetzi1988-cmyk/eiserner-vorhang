using EisernerVorhang.Core.Commands;
using EisernerVorhang.Core.Simulation;
using NUnit.Framework;

namespace EisernerVorhang.Tests
{
    public class CommandQueueTests
    {
        private static GameCommand Move(int player, long tick, int unit) =>
            new GameCommand(player, tick, CommandType.Move, new[] { unit });

        [Test]
        public void CommandsWaitForTheirTick()
        {
            var queue = new CommandQueue();
            queue.Enqueue(Move(1, 5, 10));

            Assert.IsEmpty(queue.DequeueUpTo(4));
            Assert.AreEqual(1, queue.DequeueUpTo(5).Count);
            Assert.AreEqual(0, queue.Count);
        }

        [Test]
        public void SameTick_IsOrderedByPlayerThenArrival()
        {
            var queue = new CommandQueue();
            queue.Enqueue(Move(2, 1, 100));
            queue.Enqueue(Move(1, 1, 200));
            queue.Enqueue(Move(1, 1, 300));

            var result = queue.DequeueUpTo(1);

            Assert.AreEqual(200, result[0].UnitIds[0]);
            Assert.AreEqual(300, result[1].UnitIds[0]);
            Assert.AreEqual(100, result[2].UnitIds[0]);
        }

        [Test]
        public void LateCommands_RunOnTheNextStep()
        {
            var sim = new GameSimulation();
            sim.Step();
            sim.Step();

            int executed = 0;
            sim.CommandExecuted += _ => executed++;
            sim.Submit(Move(1, 1, 1));
            sim.Step();

            Assert.AreEqual(1, executed);
        }
    }
}
