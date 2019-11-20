using System;
using System.Collections.Generic;
using SDK;
using PokerEngine;

namespace Main
{
    //The poker engine can simply call "findPokerWinner" with the apporpirate input. With it returning the names of winners as a string[] or an error in array postition [0].
    //The "Main" method can be accesed via the console application.
    //The output will simply be a string with the name of the winner.
    class MainProgram
    {
        private static bool printResults = false;
        //Mainly used for testing purposes
        //Can change input.txt once the function promts user input for a command
        static void Main(string[] args)
        {
            String input; String userInput; String[] name;
            SetupPhase.startPhase();
            while (false)
            {
                Console.WriteLine("Please enter the specified test commad.\n0 - Only print names of winners from input.txt\n" +
                "1 - Print all info from players\n2 - Check if the winners match the specified name(Only input one name)\nend - closes program");
                userInput = Console.ReadLine();
                input = System.IO.File.ReadAllText(@".\input.txt");
                if (userInput == "0")
                {
                    name = findPokerWinner(input);
                    foreach (string player in name)
                    {
                        Console.WriteLine(player);
                    }
                }
                else if (userInput == "1")
                {
                    printResults = true;
                    name = findPokerWinner(input);
                    printResults = false;
                    foreach (string player in name)
                    {
                        Console.WriteLine(player);
                    }
                }
                else if (userInput == "2")
                {
                    Console.WriteLine("Please write the name of the winner.");
                    userInput = Console.ReadLine();
                    name = findPokerWinner(input);
                    if (name[0] == userInput)
                    {
                        Console.WriteLine(userInput + " is the correct winner\n");
                    }
                    else
                    {
                        Console.WriteLine("{0} is not the correct winner, {1} is the winner\n", userInput, name[0]);
                    }
                } else if (userInput == "end")
                {
                    break;
                }
                else {
                    Console.WriteLine("Unkown command. Please use a valid command\n");
                }                  
            }
        }
        //Call this method with the parameter being the input with all cards and players as a string in the proper format.
        //Will return a list of winners as a string[]. If there's an error, it will return the error message as a string[] at position [0].
        public static string[] findPokerWinner(string input)
        {
            string[] read = InputOutput.readInput(input);
            List<String> winnersList = new List<String>();
            if (read.Length != 1)
            {
                List<Player> players = ReadCheckHand.createPlayerList(read);
                winnersList = InputOutput.callAddOn(players);
                int index = 0; read = new string[winnersList.Count];
                foreach (string player in winnersList)
                {
                    read[index++] = player;
                }
                if (printResults)
                {
                    foreach(Player person in players)
                    {
                        TestMethods.printPlayerTables(person);
                    }
                }
                
            } 
            return read;
        }
    }

}
