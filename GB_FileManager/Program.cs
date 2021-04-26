﻿using System;

namespace GB_FileManager
{
    class Program
    {


        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            FileManager fm = new FileManager();



            while (true)
            {
                //Console.Clear();

                fm.Print();


                if (fm.ControlMode)
                {
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.Delete:
                            fm.Delete(new string[0]);
                            break;

                        case ConsoleKey.Enter:
                            fm.GotoParentDirectory();
                            break;

                        case ConsoleKey.LeftArrow:
                            fm.RightPathActive = false;
                            break;

                        case ConsoleKey.RightArrow:
                            fm.RightPathActive = true;
                            break;

                        case ConsoleKey.UpArrow:
                            fm.SelectRowUp();
                            break;

                        case ConsoleKey.DownArrow:
                            fm.SelectRowDown();
                            break;

                        case ConsoleKey.F10:
                            return;

                        case ConsoleKey.F5:
                            //dirInfoL = dirInfoL.Parent ?? dirInfoL;
                            break;

                    }
                }
                else
                {
                    string s = fm.UserInputHelp();
                    fm.ExecuteCommand(s);
                }
            }

        }





    }
}
