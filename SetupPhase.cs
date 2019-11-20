using System;
using System.Collections.Generic;
using System.Linq;
using Main;
using ComputerPlayer;
using System.Text;
using System.Threading.Tasks;

namespace PokerEngine
{
    class SetupPhase
    {
        public static void startPhase()
        {
            Cards.setCards(); 
            int numCPU = setUpTable();
            beginPlay(numCPU);
            Console.ReadLine();
        }

        private static void beginPlay(int numCPU)
        {          
            bool playCont = true; bool roundCont = true; int amount = 100; int count;
            List<CPU> listOfCPUS = new List<CPU>();
            for (int x = 0; x < numCPU; x++)
            {
                CPU temp = new CPU();
                temp.setUpCPU("CPU" + x);
                listOfCPUS.Add(temp);
            }
            while (playCont)
            {
                List<string[]> cardsInPlay = dealCards(numCPU);
                count = 1;
                foreach (CPU comp in listOfCPUS)
                {
                    comp.setUpCPUHand(cardsInPlay[count][0], cardsInPlay[count][1]);
                    count++;
                }
                string[] winners = MainProgram.findPokerWinner(formatList(cardsInPlay));
                Console.WriteLine("Your cards: " + cardsInPlay[1][0] + " " + cardsInPlay[1][1]);
                int revealCards = 3; string commCards = ""; int pot = 0; 
                while (roundCont)
                {
                    bool playerChoice = true;
                    while (playerChoice)
                    {
                        playerChoice = false;
                        Console.WriteLine("Current Pot: " + pot);
                        Console.WriteLine("What would you like to do? (Fold, Bet, Check, Call) Remaining amount: " + amount);
                        String playerInput = Console.ReadLine();
                        if (playerInput.Equals("Bet"))
                        {
                            Console.WriteLine("How much will you bet? Remaining amount: " + amount);
                            int betAmount = Int32.Parse(Console.ReadLine());
                            pot += setBets(amount, betAmount);
                            amount -= betAmount;
                        }
                        else if (playerInput.Equals("Check"))
                        {

                        }
                        else if (playerInput.Equals("Fold"))
                        {
                            for (int x = 0; x < 5; x++)
                            {
                                commCards = commCards + cardsInPlay[0][x] + " ";
                            }
                            Console.WriteLine("Community Cards: " + commCards);
                            revealCards = 6;
                        }
                        else if (playerInput.Equals("Call"))
                        {

                        }
                        else
                        {
                            Console.WriteLine("Unkown Command. Please write a valid command");
                            playerChoice = true;
                        }
                    }
                    foreach(CPU comp in listOfCPUS)
                    {
                        comp.checkState(null);
                    }

                    if (revealCards > 5)
                    {
                        roundCont = false;
                    }
                    else
                    {
                        for (int x = 0; x < revealCards; x++)
                        {
                            commCards = commCards + cardsInPlay[0][x] + " ";
                        }
                        Console.WriteLine("Community Cards: " + commCards);
                    }
                    commCards = ""; revealCards++;
                }
                Console.WriteLine("Would you like to continue? y/n");
                string exit = Console.ReadLine();
                if (exit.Equals("n"))
                {
                    Console.WriteLine("Your winnings: " + amount);
                    playCont = false;
                }

            }

        }

        private static int setBets(int amount, int betAmount)
        {
            return betAmount;
        }

        private static string formatList(List<string[]> cardsInPlay)
        {
            string returnString = ""; string temp = "";int count = 0;
            for(int x = 0; x < cardsInPlay.Count; x++)
            {
                if (x == 0)
                {
                    for (int i = 0; i < cardsInPlay[x].Length; i++)
                    {
                        temp = temp + cardsInPlay[x][i];
                        if (i != cardsInPlay[x].Length - 1)
                        {
                            temp += " ";
                        }
                    }
                    returnString = returnString + temp + "\r\n";
                }
                else
                {
                    temp = "PlayerX ";
                    for (int j = 0; j < cardsInPlay[x].Length; j++)
                    {
                        temp = temp + cardsInPlay[x][j];
                        if (j != cardsInPlay[x].Length - 1)
                        {
                            temp += " ";
                        }
                    }
                    returnString += temp;
                    returnString += "\r\n";

                    
                }

            }

            return returnString;
        }

        private static List<string[]> dealCards(int numCPU)
        {
            HashSet<int> randomCards = new HashSet<int>();
            List<String[]> cardsInPlay = new List<String[]>();
            int totalCards = 7 + (2 * numCPU);
            string [] commCards = new string[5]; string[] playerHand = new string[2];
            Random random = new Random();
            while(randomCards.Count < totalCards)
            {
                randomCards.Add(random.Next(0, 51));
            }
            for(int i = 0; i < 5; i++)
            {
                commCards[i] = Cards.allCards[randomCards.ElementAt<int>(i)];        
            }
            cardsInPlay.Add(commCards);
            for (int j = 5; j < totalCards; j+=2)
            {
                playerHand[0] = Cards.allCards[randomCards.ElementAt<int>(j)];
                playerHand[1] = Cards.allCards[randomCards.ElementAt<int>(j + 1)];
                cardsInPlay.Add(playerHand);
            }
            return cardsInPlay;
        }
        private static int setUpTable()
        {
            bool typeNumPlayers = true; int CPUS = 0;
            Console.WriteLine("Please write the number of players you wish to play against. (Max 10)");
            while (typeNumPlayers)
            {               
                string numPlayers = Console.ReadLine();
                try
                {
                    CPUS = Int32.Parse(numPlayers);
                    if (CPUS > 10 || CPUS < 1)
                    {
                        Console.WriteLine("Invalid number of players. Please input an integer value between 1 - 10");
                    }
                    else
                    {
                        typeNumPlayers = false;
                    }                   

                }
                catch (FormatException)
                {
                    Console.WriteLine("Please input a valid integer value");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Invalid number of players. Please input an integer value between 1 - 10");
                }
            }
            return CPUS;

        }

    }

    class Cards
    {
        public static string[] allCards = new string[52];

        public static void setCards()
        {
            int cardNum = 2; string numString; string[] suites = new string[] { "D", "C", "H", "S" };
            int suiteOrder = 0;
            for(int i = 0; i < allCards.Length; i++)
            {
                if (cardNum == 10)
                {
                    numString = "T";
                    numString = numString + suites[suiteOrder];
                }
                else if(cardNum == 11)
                {
                    numString = "J";
                    numString = numString + suites[suiteOrder];

                }
                else if (cardNum == 12)
                {
                    numString = "Q";
                    numString = numString + suites[suiteOrder];

                }
                else if (cardNum == 13)
                {
                    numString = "K";
                    numString = numString + suites[suiteOrder];

                }
                else if (cardNum == 14)
                {
                    numString = "A";
                    numString = numString + suites[suiteOrder++];
                    cardNum = 1;

                }
                else
                {
                    numString = cardNum.ToString() + suites[suiteOrder];
                }                               
                allCards[i] = numString;
                cardNum++;
            }
        }
    }

}
