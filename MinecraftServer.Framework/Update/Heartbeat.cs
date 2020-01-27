namespace MinecraftServer.Framework.Update
{
    public class Heartbeat
    {
        public bool Flatline => timeToFlatline <= 0d;

        private double timeToFlatline;

        public Heartbeat() => OnHeartBeat();

        public void OnHeartBeat()
        {
            timeToFlatline = 300d;
        }

        public void Update(double lastTick)
        {
            timeToFlatline -= lastTick;
        }
    }
}