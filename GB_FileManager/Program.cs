using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace GB_FileManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string configFileName = "config.json";

            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            FileManager fm;
            FMConfig config;

            // Чтение конфигурационного файла
            using (FileStream fs = new FileStream(configFileName, FileMode.OpenOrCreate))
            {
                try
                {
                    config = await JsonSerializer.DeserializeAsync<FMConfig>(fs);
                }
                catch (Exception e)
                {
                    // При неудачном чтении загружаются значения по умолчанию
                    FileManager.Log(e);
                    config = new FMConfig()
                    {
                        LeftPathString = Directory.GetCurrentDirectory(),
                        RightPathString = Directory.GetCurrentDirectory(),
                        ExtensionColumn = 12,
                        CreationInfoColumn = 20,
                        LengthColumn = 13,
                        InfoPanelHeight = 7,
                        ElementsPerPage = 40,
                    };
                }
            }

            fm = new FileManager(config);

            while (true)
            {
                //Console.Clear();
                fm.Print();

                if (fm.ControlMode)
                {
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.F12:
                            fm.SwitchControlMode();
                            break;

                        case ConsoleKey.Delete:
                            fm.Delete(new string[0]);
                            break;

                        case ConsoleKey.Enter:
                            fm.KeyAction();
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
                            fm.Copy(new string[0]);
                            break;

                    }
                }
                else
                {
                    string s = fm.UserInputHelp();
                    fm.ExecuteCommand(s);
                }

                // Сохранение текущего состояния
                File.Delete(configFileName);
                using (FileStream fs = new FileStream(configFileName, FileMode.OpenOrCreate))
                {
                    await JsonSerializer.SerializeAsync<FMConfig>(fs, fm.Config);
                }
            }

        }





    }
}
