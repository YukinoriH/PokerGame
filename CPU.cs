using System;
using SDK;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerPlayer
{
    class CPU
    {
        private int amount;
        //The following behavior can be set:
        //0 - Plays very safe, bets in small increments unless it's a really good odds 
        //1 - Plays in moderation, takes small chances with bets 
        //2 - Sometimes takes some risk, large bets when good hand comes
        //3 - Plays risky, likes to bet a lot on decent hands 
        private int behavior;
        private Player comp = new Player();
        private string name;

        public void setUpCPU(string cpuname)
        {
            amount = 100;
            Random random = new Random();
            behavior = random.Next(0, 4);
            name = cpuname;
        }
        public void setUpCPUHand(string card1, string card2)
        {
            comp.setCPUInfo(name, card1, card2);
        }
        public int checkState(string commCards)
        {
            switch (behavior)
            {
                case 0:
                    break;

                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    break;
            }
            return 0;
        }

        private static int betAmount(string[] cards, string[] commCards)
        {
            //checkState();
            return 0;
        }
        private static int checkOdds()
        {
            return 0;
        }
    }
}
