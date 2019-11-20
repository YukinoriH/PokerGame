using System;
using System.Collections.Generic;
using SDK;

namespace AddOn
{
    class DetermineWinner
    {
        //Iterates through all players in player list and finds the strongest poker hand they have.
        //Gives a "weight" to that poker hand then compares it to all other players.
        //Returns a list of winners with the highest or equal "weight"
        //If there is an error, returns null and prints the error message. 
        public static List<String> findWinner(List<Player> playerList)
        {
            int strongest = 0; int temp = 0;
            List<Player> winners = new List<Player>();
            List<String> winnerList = new List<String>();
            foreach (Player p in playerList)
            {
                temp = strongestHand(p);
                if (temp == 0)
                {
                    winnerList.Add("Error: Invalid card in player's hand");
                    return winnerList;
                } else if(temp == -1)
                {
                    winnerList.Add("Error: Invalid card in community cards");
                    return winnerList;
                }
                else if (strongest <= temp)
                {
                    //Only keeps the players with the strongest poker hand
                    strongest = temp;
                    winners.Add(p);
                    winners.RemoveAll(item => item.strongestHandWeight < strongest);
                }          
            }
            //Returns a List<string> with the names of all players
            winners.ForEach(person => winnerList.Add(person.name));
            return (winnerList);
        }

        private static int strongestHand(Player currPlayer)
        {
            //Iterates through every card from the players hand and marks which cards the player has
            foreach (string card in currPlayer.pokerHand)
            {
                if(ReadCheckHand.readCard(currPlayer, card))
                {
                    return 0;
                }

            }
            //Iterates through every card from the community cards and marks which cards the player has
            foreach (string card in currPlayer.tableHand)
            {
                if(ReadCheckHand.readCard(currPlayer, card))
                {
                    return -1;
                }

            }
            //Finds all poker hands the player has 
            ReadCheckHand.checkHand(currPlayer);

            //Gives a weight to determine the strength of a poker hand. Based on the poker hand and the highest value for that hand.
            //E.g. A pair 8 will have a weight of 36. -> (140 - 14*8 + 8).
            //The weight decreases by 14 to prevent a weaker poker hand from having a higher value then a stronger poker hand
            //E.g. A pair of Aces has a weight of 42(140 - 14*8 + 14), while 2 pairs of 2's is 44(140 - 14*7 + 2).
            int weight = 140;
            foreach (KeyValuePair<string, int> hand in currPlayer.handTable)
            {
                if (hand.Value > 0)
                {
                    currPlayer.strongestHandWeight = hand.Value + weight;
                    return hand.Value + weight;
                }
                weight -= 14;
            }
            return weight;
        }

        
    }
}
