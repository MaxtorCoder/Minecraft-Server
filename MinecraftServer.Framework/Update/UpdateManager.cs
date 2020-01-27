using System;
using System.Diagnostics;
using System.Threading;

namespace MinecraftServer.Framework.Update
{
    public class UpdateManager
    {
        private static volatile bool shutdownRequested;

        public static void Initialize(Action<double> updateAction)
        {
            var worldThread = new Thread(() =>
            {
                var stopwatch = new Stopwatch();
                double lastTick = 0d;

                while (!shutdownRequested)
                {
                    stopwatch.Restart();

                    updateAction(lastTick);

                    Thread.Sleep(1);
                    lastTick = (double)stopwatch.ElapsedTicks / Stopwatch.Frequency;
                }
            });

            worldThread.Start();
        }

        public static void Shutdown()
        {
            shutdownRequested = true;
        }
    }
}