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

        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            ShowWindow(ThisConsole, MAXIMIZE);

            string path = Directory.GetCurrentDirectory();
            var dirInfo = new DirectoryInfo(path);

            while (true)
            {
                Console.Clear();

                PrintDirectory(dirInfo);


                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.F10:
                        return;

                    case ConsoleKey.F5:
                        dirInfo = dirInfo.Parent ?? dirInfo;
                        break;

                }
            }

        }
        /// <summary>
        /// Выводит на экран папки и файлы каталога
        /// </summary>
        /// <param name="di"></param>
        public static void PrintDirectory(DirectoryInfo di)
        {
            // Размеры столбцов таблицы
            int fileNameColumn = 40;
            int extentionColumn = 10;
            int fileLengthColumn = 10;

            // Разделитель
            string div = "| ";

            var fileList = di.GetFiles();
            var dirList = di.GetDirectories();

            List<string> table = new List<string>();

            for (int i = 0; i < dirList.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(div)
                  .Append(CutString(dirList[i].Name, fileNameColumn))
                  .Append(div)
                  .Append("Папка".PadRight(extentionColumn))
                  .Append(div)
                  .Append(" ".ToString().PadRight(fileLengthColumn))
                  .Append(div);

                table.Add(sb.ToString());
            }

            for (int i = 0; i < fileList.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(div)
                    .Append(CutString(fileList[i].Name, fileNameColumn))
                    .Append(div)
                    .Append(CutString(fileList[i].Extension, extentionColumn))
                    .Append(div)
                    .Append(fileList[i].Length.ToString().PadRight(fileLengthColumn))
                    .Append(div);

                table.Add(sb.ToString());
            }

            Console.WriteLine(di.FullName);
            foreach (string s in table)
            {
                Console.WriteLine(s);
            }
        }

        static string CutString(string str, int maxLength)
        {
            if (str.Length <= maxLength)
            {
                return str.PadRight(maxLength);
            }

            if (maxLength < 6)
            {
                return str.Substring(0, maxLength);
            }

            return str.Substring(0, maxLength / 2 - 1) + ".." + str.Substring(str.Length - (maxLength / 2 + 1));
        }
    }
}
