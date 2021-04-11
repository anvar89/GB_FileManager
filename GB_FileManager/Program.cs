using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GB_FileManager
{
    class Program
    {
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int HIDE = 0;
        private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE = 9;


        static string div = "│";

        static void Main(string[] args)
        {

            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            ShowWindow(ThisConsole, MAXIMIZE);
            //Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            FileManager fm = new FileManager();



            while (true)
            {
                Console.Clear();

                fm.Print();


                Console.SetCursorPosition(0, 0);
                Console.SetCursorPosition(0, Console.WindowHeight);

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.F10:
                        return;

                    case ConsoleKey.F5:
                        //dirInfoL = dirInfoL.Parent ?? dirInfoL;
                        break;

                }
            }

        }





    }
}
