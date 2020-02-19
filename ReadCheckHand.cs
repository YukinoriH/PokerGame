using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using AddOn;

namespace SDK
{
    class ReadCheckHand
    {
        //Creates a list with all players using the player object.
        public static List<Player> createPlayerList(string[] inputArray)
        {
            string commCards = inputArray[0];
            int inputSize = inputArray.Length;
            List<Player> playerList = new List<Player>();
            Player tempPlayer;
            for (int x = 1; x < inputSize; x++)
            {
                tempPlayer = new Player();
                tempPlayer.setInfo(inputArray[x], commCards);
                playerList.Add(tempPlayer);
            }

            return playerList;
        }
        //Reads all the cards(player hand and community cards) and marks which cards are found and how many.
        //The numbers and suites are kept track in seperate tables
        //Takes in the player and the card this method will read
        public static bool readCard(Player currPlayer, string card)
        {
            char[] currHand = new char[2];
            bool invalidNum; bool invalidSuite;
            string number; string suite;
            if (card == null || card.Length != 2)
            {
                return true;
            }
            using (StringReader sr = new StringReader(card))
            {
                sr.Read(currHand, 0, 2);
                number = currHand[0].ToString();
                suite = currHand[1].ToString();
                invalidNum = true; invalidSuite = true;

                //To keep track of which card is the highest for suites(Used for tie-breakers in flushes)it will add 10*(the card number) if the previous card is lower. 
                //It will always add 1 to the value to indicate the number of suite cards found.
                //e.g. 2D will be 10*2 = 20 so the value is 21. If the next card is 4D, 10*4 is > 10*2, so the value now is 42.
                if (currPlayer.suiteTable.ContainsKey(suite))
                {
                    int numCard = checkFaceCards(number) * 10;
                    currPlayer.suiteTable[suite] = currPlayer.suiteTable[suite] + 1;
                    if (numCard > currPlayer.suiteTable[suite])
                    {
                        currPlayer.suiteTable[suite] = (currPlayer.suiteTable[suite] % 10) + numCard;
                    }
                    invalidSuite = false;
                }
                if (currPlayer.numberTable.ContainsKey(number))
                {
                    currPlayer.numberTable[number] = currPlayer.numberTable[number] + 1;
                    invalidNum = false;
                    if (number == "A")
                    {
                        currPlayer.numberTable["1"] = currPlayer.numberTable["1"] + 1;
                    }
                }
                //Error check: if the char values for the cards are not valid, returns true;
                if(invalidSuite || invalidNum)
                {
                    return true;
                }

            }
            return false;
        }

        //Goes through the number and suite tables and finds what poker hands each player has.
        public static void checkHand(Player currPlayer)
        {
            //Checks the suite table
            foreach (KeyValuePair<string, int> card in currPlayer.suiteTable)
            {
                if (card.Value%10 > 4)
                {
                    //This opertaion is used so the handTables's value is equal to the highest card number value
                    currPlayer.foundFlush((card.Value - (card.Value % 10)) / 10);
                }
            }
            //Checks the number table
            int count = 0;
            foreach (KeyValuePair<string, int> card in currPlayer.numberTable)
            {
                if (card.Value > 0)
                {
                    currPlayer.foundHighCard(checkFaceCards(card.Key));
                    count++;
                    if (count == 5)
                    {
                        currPlayer.foundStraight(checkFaceCards(card.Key));
                    }
                }
                else
                {
                    count = 0;
                }

                if (card.Value == 2)
                {
                    currPlayer.foundPair(checkFaceCards(card.Key));
                }
                else if (card.Value == 3)
                {
                    currPlayer.foundThreeKind(checkFaceCards(card.Key));
                }
                else if (card.Value == 4)
                {
                    currPlayer.foundFourKind(checkFaceCards(card.Key));
                }

            }
            //If there's both a flush and straight, check for a straight flush/royal flush
            if (currPlayer.handTable["Fl"] > 0 && currPlayer.handTable["St"] > 0)
            {
                checkStraightFlush(currPlayer);
            }
        }

        private static void checkStraightFlush(Player player)
        {
            //Finds which suite has the flush
            string checkSuite = "";
            foreach (KeyValuePair<string, int> x in player.suiteTable)
            {
                if (x.Value%10 > 4)
                {
                    checkSuite = x.Key;
                    break;
                }
            }
            //Creates an array of all cards
            string[] allCards = new string[7];
            player.pokerHand.CopyTo(allCards, 0);
            player.tableHand.CopyTo(allCards, player.pokerHand.Length);

            //Finds all cards with the specified suite from above
            int[] suiteCards = new int[8];
            int index = 0;
            foreach (string card in allCards)
            {
                char[] currHand = new char[2];
                string number; string suite;
                using (StringReader sr = new StringReader(card))
                {
                    sr.Read(currHand, 0, 2);
                    number = currHand[0].ToString();
                    suite = currHand[1].ToString();
                    if (suite.Equals(checkSuite))
                    {
                        suiteCards[index] = checkFaceCards(number);
                        index++;
                    }
                }
            }
            Array.Sort(suiteCards);
            bool royal = false;
            //Checks for Aces which are both the int value 1 and 14
            if (suiteCards[7] == 14)
            {
                suiteCards[0] = 1;
                Array.Sort(suiteCards);
                royal = checkRoyal(suiteCards);
            }
            //Now that the array is sorted, see if there are 5 consecutive numbers(i.e. find a Straight)
            int count = 1;
            for (int x = 0; x < suiteCards.Length - 1; x++)
            {
                if (suiteCards[x] + 1 == suiteCards[x + 1])
                {
                    count++;
                }
                else
                {
                    count = 1;
                }

                if (count > 4)
                {
                    player.foundStraightFlush(royal, suiteCards[x + 1]);
                    break;
                }
            }
        }
        //Returns the corresponding int value for every card
        private static int checkFaceCards(string number)
        {
            if (int.TryParse(number, out int x))
           {
                return x;
            }
            else if (number == "T")
            {
                return 10;
            }
            else if (number == "J")
            {
                return 11;
            }
            else if (number == "Q")
            {
                return 12;
            }
            else if (number == "K")
            {
                return 13;
            }
            else if (number == "A")
            {
                return 14;
            }
            return 0;
        }
        //Checks if the list contatins 10-Ace
        //Because the deck is ordered and can only contain 1 card of each number,
        //count will only ever reach 14 if 10, Jack, Queen, King, and Ace are in the deck
        private static bool checkRoyal(int[] deck)
        {
            int count = 0;
            foreach (int x in deck)
            {
                if (x == 10)
                {
                    count = 10;
                }
                else if (x > 10)
                {
                    count++;
                }
            }
            return (count >= 14);
        }
    }

    //All methods used to read and format the input from a poker engine
    //Outputs either the input as a string[] or an error message in x[0] if there's something wrong with the format of the input
    class InputOutput
    {
        public static string[] readInput(string input)
        {               
            string[] finalInput = formatInput(input);
            string errorCheck = checkError(finalInput);
            if (errorCheck == "")
            {
                //No error in format
                return finalInput;
            }
            else
            {
                string[] x = new string[1] { errorCheck };
                return x;
            }               

        }

        //Checks if the input is in the correct format by checking the number
        //of cards and the number of characters in each card. Returns an empty string if there are no problems.
        private static string checkError(string[] input)
        {
            Regex reg = new Regex(" ");
            int lineLength; string[] checkLine;

            foreach (string x in input){
                checkLine = reg.Split(x);
                lineLength = checkLine.Length;
                if (lineLength != 5 && lineLength != 3 )
                {
                    return ("Error: The number of cards is invalid");
                } 
                //Checks the community cards.
                if(lineLength == 5)
                {
                    foreach(string y in checkLine)
                    {
                        if(y.Length != 2)
                        {
                            return ("Error: Problem with the number of characters in cards");
                        }
                    }
                //Checks player names and cards.
                } else if (lineLength == 3)
                {
                    //Skips player name
                    for (int i = 1; i < 3; i++)
                    {
                        if (checkLine[i].Length != 2)
                        {
                            return ("Error: Problem with the number of characters in cards");
                        }
                    }
                }
            }
            return "";
        }

        //Seperates the input into string array, line by line
        private static string[] formatInput(string input)
        {
            Regex reg = new Regex(@"\r\n");
            string[] inputArray = reg.Split(input);
            return inputArray;
        }

        public static List<String> callAddOn(List<Player> playerList)
        {
            return DetermineWinner.findWinner(playerList);
        }

    }


    //Player object assigned to each player. Contains name, hand, community cards, strongest poker hand, and 3 tables.
    //The tables are used to keep track of which cards and poker hand each player has. 
    class Player
    {
        public string name;
        public string[] pokerHand = new string[2];
        public string[] tableHand = new string[5];
        public int strongestHandWeight;
        public Dictionary<string, int> suiteTable = new Dictionary<string, int>();
        public Dictionary<string, int> numberTable = new Dictionary<string, int>();
        public Dictionary<string, int> handTable = new Dictionary<string, int>();

        //Sets up all the info needed for every player
        //Takes in the player information from the input from the poker engine 
        public void setInfo(string info,string commCards)
        {
            Regex reg = new Regex(" ");
            string[] spacedInfo = reg.Split(info);
            name = spacedInfo[0];
            pokerHand[0] = spacedInfo[1];
            pokerHand[1] = spacedInfo[2];
            string[] spacedCards = reg.Split(commCards);
            for(int x = 0; x < spacedCards.Length; x++)
            {
                tableHand[x] = spacedCards[x];
            }
            CreateHandTable();
        }
        public void setCPUInfo(string cpuName, string card1, string card2)
        {
            name = cpuName;
            pokerHand[0] = card1;
            pokerHand[1] = card2;
            CreateHandTable();
        }
        private void CreateHandTable()
        {
            suiteTable = new Dictionary<string, int>();
            numberTable = new Dictionary<string, int>();
            handTable = new Dictionary<string, int>();

            //Heart, Spade, Dimond, Clubs
            suiteTable.Add("D", 0);
            suiteTable.Add("C", 0);
            suiteTable.Add("H", 0);
            suiteTable.Add("S", 0);

            //2 - 9, Ten, Jack, Queen, King, Ace
            //An Ace will count towards both the value "1" and "A"
            string temp;
            for (int x = 1; x <= 9; x++)
            {
                temp = Convert.ToString(x);
                numberTable.Add(temp, 0);
            }
            numberTable.Add("T", 0);
            numberTable.Add("J", 0);
            numberTable.Add("Q", 0);
            numberTable.Add("K", 0);
            numberTable.Add("A", 0);

            //Poker hands are abbreviated to two chatacters
            handTable.Add("RF", 0);
            handTable.Add("SF", 0);
            handTable.Add("FK", 0);
            handTable.Add("FH", 0);
            handTable.Add("Fl", 0);
            handTable.Add("St", 0);
            handTable.Add("TK", 0);
            handTable.Add("TP", 0);
            handTable.Add("OP", 0);
            handTable.Add("HC", 0);
        }

        //The following methods are used when a poker hand is found. 
        //They will add the value of the strongest card in the poker hand to the handTable.
        //E.g. If there's a pair of Aces, the value for handTable["OP"] will be 14.
        public void foundHighCard(int value)
        {
            if(handTable["HC"] < value)
            {
                handTable["HC"] = value;
            }
        }
        public void foundPair(int value)
        {
            if (handTable["OP"] > 0)
            {
                if (handTable["TP"] < value)
                {
                    handTable["TP"] = value;
                    //If there are 2 pairs, the handTable will keep the pair with the higher card
                    if (handTable["TP"] < handTable["OP"])
                    {
                        handTable["TP"] = handTable["OP"];
                    } 
                    
                }
                //If there's a three of a kind, then another pair would become a full hosue
                if (handTable["TK"] > 0)
                {
                    handTable["FH"] = handTable["TK"];
                }
            }
            else
            {
                if (handTable["OP"] < value)
                {
                    handTable["OP"] = value;
                }               
            }         

        }
        public void foundThreeKind(int value)
        {
            if (handTable["TK"] < value)
            {
                handTable["TK"] = value;
            }
            //If there's a pair already, then a three of a kind would become a full hosue
            if (handTable["OP"] > 0)
            {
                if (handTable["FH"] < value)
                {
                    handTable["FH"] = value;
                }
            }
            //A pair will always be in a Three of a kind
            foundPair(value);
        }
        public void foundFourKind(int value)
        {
            if (handTable["FK"] < value)
            {
                handTable["FK"] = value;
            }
        }
        public void foundStraight(int value)
        {
            if(handTable["St"] < value)
            {
                handTable["St"] = value;
            }

        }
        public void foundFlush(int value)
        {
            if(handTable["Fl"] < value)
            {
                handTable["Fl"] = value;
            }

        }
        public void foundStraightFlush(bool royal, int value)
        {
            if (handTable["SF"] < value)
            {
                handTable["SF"] = value;
            }
            if (royal)
            {
                //No tie-breakers for a Royal flush 
                handTable["RF"] = 1;
            }
        }

    }
    //The following methods are used for testing purposes
    class TestMethods
    {

        //Prints out all tables for a player
        //Used for manual testing/de-buging
        public static void printPlayerTables(Player test)
        {
            Console.WriteLine("{0}'s Tables:", test.name);
            Console.WriteLine("Suite Table");
            foreach (KeyValuePair<string, int> kvp in test.suiteTable)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
            Console.WriteLine("");
            Console.WriteLine("Number Table");
            foreach (KeyValuePair<string, int> kvp in test.numberTable)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
            Console.WriteLine("");
            Console.WriteLine("Hand Table");
            foreach (KeyValuePair<string, int> kvp in test.handTable)
            {
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
            Console.WriteLine("");
        }
    }
}
