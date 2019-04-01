using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TableTopRobot.Interfaces;
using TableTopRobot.Models;

namespace TableTopRobot
{
    class Program
    {
        protected static Boolean IsRobotPlaced { get; set; }

        protected static string Command { get; set; }
        protected static int X { get; set; }
        protected static int  Y { get; set; }
        protected static FacingDirection F { get; set; }

        protected static IPreviousMovements _previousMovements;

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                //An arugement is not supplied
                Console.WriteLine($"Please supply the text file path as the application arugement.");
                return;
            }

            #region Setting up DI 

                //setup our DI
                var serviceProvider = new ServiceCollection()
                    .AddSingleton<IPreviousMovements, PreviousMovements>()
                    .BuildServiceProvider();

                //inject the service class
                _previousMovements = serviceProvider.GetService<IPreviousMovements>();
            #endregion

            try
            {
                //use this command on Debug mode
                //var result = ReadCsvSimple(@"C:\iSelect\TableTopRobot\Publish\RobotCommands_3.txt", ';');

                var result = ReadCsvSimple(args[0], ';');

                foreach (var items in result)
                {
                    //read the first command
                    var itemArray = items[0].Split(',');
                    Command = itemArray[0];

                    if (Command.Contains("PLACE"))
                    {
                        //Robot started as first time or placed in new position
                        if (itemArray.Length < 3)
                        {
                            //invalid Argument
                            return;
                        }
                        else
                        {
                            var xs = itemArray[0].Split(' ')[1];
                            var x = int.Parse(xs);
                            var y = int.Parse(itemArray[1]);
                            var f = itemArray[2];

                            SetNewValues(x, y, f);

                            if (IsValidMovement())
                            {
                                _previousMovements.SetPreviousValues(X, Y, F);
                                IsRobotPlaced = true;
                            }
                        }

                    }
                    else
                    {
                        if (!IsRobotPlaced)
                        {
                            //robot has to be placed to run any other command than 'PLACE'
                            Console.WriteLine($"Please place the robot on the table top.");
                            return;
                        }

                        bool isToValidate = true;

                        switch (Command.Trim().ToUpper())
                        {
                            case "MOVE":
                                if (_previousMovements.F == FacingDirection.South)
                                    TurnToSouth();
                                else if (_previousMovements.F == FacingDirection.North)
                                    TurnToNorth();
                                else if (_previousMovements.F == FacingDirection.West)
                                    TurnToWest();
                                else if (_previousMovements.F == FacingDirection.East)
                                    TurnToEast();
                                break;
                            case "LEFT":
                                if (_previousMovements.F == FacingDirection.South)
                                    TurnToEast();
                                else if (_previousMovements.F == FacingDirection.North)
                                    TurnToWest();
                                else if (_previousMovements.F == FacingDirection.West)
                                    TurnToSouth();
                                else if (_previousMovements.F == FacingDirection.East)
                                    TurnToNorth();
                                break;
                            case "RIGHT":
                                if (_previousMovements.F == FacingDirection.South)
                                    TurnToWest();
                                else if (_previousMovements.F == FacingDirection.North)
                                    TurnToEast();
                                else if (_previousMovements.F == FacingDirection.West)
                                    TurnToNorth();
                                else if (_previousMovements.F == FacingDirection.East)
                                    TurnToSouth();
                                break;
                            case "REPORT":
                                isToValidate = false;
                                var lastMovement = string.Format("{0},{1},{2}", _previousMovements.X, _previousMovements.Y, _previousMovements.F.ToString());
                                Console.WriteLine($"Output : {lastMovement}");
                                break;
                            default:
                                isToValidate = false;
                                break;
                        }

                        if (isToValidate)
                        {
                            if (IsValidMovement())
                            {
                                _previousMovements.SetPreviousValues(X, Y, F);
                            }
                        }

                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Program is terminated due to an error occured. Error details : {ex.Message}!");
            }
                                        
        }


        private static IEnumerable<string[]> ReadCsvSimple(string file, char delimiter)
        {
            return File
              .ReadLines(file)
              .Where(line => !string.IsNullOrEmpty(line)) // skip empty lines if any
              .Select(line => line.Split(delimiter));
        }

        /// <summary>
        /// set X , Y, F with input values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="f">string value</param>
        private static void SetNewValues(int x, int y, string f)
        {
            X = x;
            Y = y;

            switch (f.Trim().ToUpper())
            {
                case "SOUTH":
                    F = FacingDirection.South;
                    break;
                case "NORTH":
                    F = FacingDirection.North;
                    break;
                case "EAST":
                    F = FacingDirection.East;
                    break;
                case "WEST":
                    F = FacingDirection.West;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// set X , Y, F with input values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="f">Enum value</param>
        private static void SetNewValues(int x, int y, FacingDirection f)
        {
            X = x;
            Y = y;
            F = f;
        }

        /// <summary>
        /// Check wether the movement is valid to make sure robot is not falling off from the table
        /// </summary>
        /// <returns></returns>
        private static bool IsValidMovement()
        {
            if ((X >= 0 && X <= 5) && (Y >= 0 && Y <= 5))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Turn robot to SOUTH direction with new X Y values
        /// </summary>
        private static void TurnToSouth()
        {
            SetNewValues(_previousMovements.X, _previousMovements.Y - 1, FacingDirection.South);
        }
        /// <summary>
        /// Turn robot to NORTH direction with new X Y values
        /// </summary>
        private static void TurnToNorth()
        {
            SetNewValues(_previousMovements.X, _previousMovements.Y + 1, FacingDirection.North);
        }
        /// <summary>
        /// Turn robot to WEST direction with new X Y values
        /// </summary>
        private static void TurnToWest()
        {
            SetNewValues(_previousMovements.X -1, _previousMovements.Y, FacingDirection.West);
        }
        /// <summary>
        /// Turn robot to EAST direction with new X Y values
        /// </summary>
        private static void TurnToEast()
        {
            SetNewValues(_previousMovements.X + 1, _previousMovements.Y, FacingDirection.East);
        }
    }
}
