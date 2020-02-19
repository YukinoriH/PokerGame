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

        private static int amount = 100;
        private static int pot = 0;
        private static void beginPlay(int numCPU)
        {          
            bool playCont = true; int count;
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
                count = 2;
                foreach (CPU comp in listOfCPUS)
                {
                    comp.setUpCPUHand(cardsInPlay[count][0], cardsInPlay[count][1]);
                    count++;
                }
                
                int revealCards = 3; string commCards = ""; bool roundCont = true; bool playerFold = false;
                while (roundCont)
                {
                    bool playerChoice1 = true;
                    while (playerChoice1)
                    {
                        int contPlay = playerTurn(commCards, cardsInPlay);
                        if (contPlay == 0)
                        {
                            revealCards = 6;
                            playerFold = true;
                            playerChoice1 = false;
                        } else if (contPlay == 1)
                        {
                            playerChoice1 = false;
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

                int cpuNum = 1;
                foreach (string [] x in cardsInPlay)
                {
                    if(x.Length < 3)
                    {
                        if (playerFold)
                        {
                            Console.WriteLine("CPU" + cpuNum++ + "'s Hand: " + x[0] + " " + x[1]);
                        } else
                        {
                            Console.WriteLine("Player's Hand: " + x[0] + " " + x[1]);
                            playerFold = true;
                        }
                        
                    }                  
                }
                string[] winners = MainProgram.findPokerWinner(formatList(cardsInPlay,playerFold));

                Console.WriteLine(winners[0] + " wins!");
                Console.WriteLine("Would you like to continue? y/n");
                bool playerChoice2 = true;
                while (playerChoice2)
                {
                    string exit = Console.ReadLine();
                    if (exit.Equals("n"))
                    {
                        Console.WriteLine("Your winnings: " + amount);
                        playCont = false;
                        playerChoice2 = false;
                    }
                    else if (exit.Equals("y"))
                    {
                        Console.WriteLine("Your winnings: " + amount);
                        playerChoice2 = false;
                    }
                    else
                    {
                        Console.WriteLine("Unkown Command. Please write a valid command");
                    }
                }
            }

        }
        private static int playerTurn(string commCards,List<string[]> cardsInPlay)
        {
            Console.WriteLine("Current Pot: " + pot);
            Console.WriteLine("Your cards: " + cardsInPlay[1][0] + " " + cardsInPlay[1][1]);
            Console.WriteLine("What would you like to do? (Fold, Bet, Check) Remaining amount: " + amount);
            String playerInput = Console.ReadLine().ToLower();
            if (playerInput.Equals("bet"))
            {
                Console.WriteLine("How much will you bet? Remaining amount: " + amount);
                while (true)
                {
                    try
                    {
                        int betAmount = Int32.Parse(Console.ReadLine());
                        pot += setBets(amount, betAmount);
                        amount -= betAmount;
                        break;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Please input a valid integer value");
                    }
                }
                return 1;
            }
            else if (playerInput.Equals("check"))
            {
                return 1;
            }
            else if (playerInput.Equals("fold"))
            {
                for (int x = 0; x < 5; x++)
                {
                    commCards = commCards + cardsInPlay[0][x] + " ";
                }
                cardsInPlay.RemoveAt(1);
                Console.WriteLine("Community Cards: " + commCards);              
                return 0;
            }
            else
            {
                Console.WriteLine("Unkown Command. Please write a valid command");
                return 2;
            }
        }

        private static int setBets(int amount, int betAmount)
        {
            return betAmount;
        }

        private static string formatList(List<string[]> cardsInPlay, bool playerFold)
        {
            string returnString = ""; string temp = "";int count = 1;
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
                    if (!playerFold && x == 1)
                    {
                        temp = "Player" + " ";
                    }
                    else
                    {
                        temp = "CPU" + count++ + " ";
                    }

                    for (int j = 0; j < 2; j++)
                    {
                        temp = temp + cardsInPlay[x][j];
                        if (j != cardsInPlay[x].Length - 1)
                        {
                            temp += " ";
                        }
                    }
                    returnString += temp;
                    if(x != cardsInPlay.Count - 1)
                    {
                        returnString += "\r\n";
                    }                    
                    
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
                playerHand = new string[2];
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
