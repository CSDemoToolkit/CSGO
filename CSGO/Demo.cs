using DemoInfo;

namespace CSGO
{
    public class Demo
    {
        public void Analyze(string path)
        {
            DemoParser parser = new(File.OpenRead(path));
            parser.ParseHeader();

            //Console.WriteLine(parser.Map);
            //Console.WriteLine(parser.Header.PlaybackTicks);

            parser.PlayerKilled += Parser_PlayerKilled;

            //parser.TickDone += Parser_TickDone;
            //parser.RoundEnd += Parser_RoundEnd;
            //parser.LastRoundHalf += Parser_LastRoundHalf;

            Console.WriteLine("Starting");

            parser.ParseToEnd();
            //Console.WriteLine(parser.CurrentTick);

            Console.WriteLine("Done!");
            /*
            */
        }

        bool matchStart = false;
        int t1 = 0;
        int t2 = 0;

        private void Parser_LastRoundHalf(object? sender, LastRoundHalfEventArgs e)
        {
            Console.WriteLine("Half!");
            int ts = t1;
            t1 = t2;
            t2 = ts;
        }

        private void Parser_RoundEnd(object? sender, RoundEndedEventArgs e)
        {
            matchStart = true;
            if (e.Winner == Team.Terrorist)
            {
                t1++;
            }
            else if (e.Winner == Team.CounterTerrorist)
            {
                t2++;
            }

            Console.WriteLine($"New Round T-CT: {t1} - {t2}");
        }

        private void Parser_TickDone(object? sender, TickDoneEventArgs e)
        {
            
        }

        private void Parser_PlayerKilled(object? sender, PlayerKilledEventArgs e)
        {
            //Console.WriteLine($"    {e.Killer.Name}({e.Killer.EntityID}) killed {e.Victim.Name}({e.Victim.EntityID})");
        }
    }
}