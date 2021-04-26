using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GB_FileManager
{
    class FileManager
    {
        public FMdimentions Dimentions { get; set; }

        private string TopCornerAndPath => UIelement.ADR + "Path 1: "
                                        + LeftPath.FullName
                                        + UIelement.ALDs.PadLeft(Dimentions.tableWidth - 8 - LeftPath.FullName.Length, UIelement.H)
                                        + UIelement.ADR + "Path 2: "
                                        + RightPath.FullName
                                        + UIelement.ALDs.PadLeft(Dimentions.tableWidth - 8 - RightPath.FullName.Length, UIelement.H);

        private string DownCornerAndPage
        {
            get
            {

                string pagesTextLeft = "Стр.: " + (leftPageNum + 1) + "/" + leftList.Count;
                string pagesTextRight = "Стр.: " + (rightPageNum + 1) + "/" + rightList.Count;

                return UIelement.AUR
                       + pagesTextLeft
                       + UIelement.AULs.PadLeft(Dimentions.tableWidth - pagesTextLeft.Length, UIelement.H)
                       + UIelement.AUR
                       + pagesTextRight
                       + UIelement.AULs.PadLeft(Dimentions.tableWidth - pagesTextRight.Length, UIelement.H);
            }
        }

        public string Divider
        {
            get => UIelement.VRs.PadRight(Dimentions.fileName, UIelement.H)
                 + UIelement.HDs.PadRight(Dimentions.extension, UIelement.H)
                 + UIelement.HDs.PadRight(Dimentions.length, UIelement.H)
                 + UIelement.HDs.PadRight(Dimentions.creationInfo, UIelement.H)
                 + UIelement.VL;
        }

        public bool ControlMode => controlByHotKey;

        private string Divider2
        {
            get => UIelement.VRs.PadRight(Dimentions.fileName, UIelement.H)
                 + UIelement.VHs.PadRight(Dimentions.extension, UIelement.H)
                 + UIelement.VHs.PadRight(Dimentions.length, UIelement.H)
                 + UIelement.VHs.PadRight(Dimentions.creationInfo, UIelement.H)
                 + UIelement.VL;
        }

        private string BottomCorner
        {
            get => UIelement.AURs.PadRight(Dimentions.fileName, UIelement.H)
                 + UIelement.HUs.PadRight(Dimentions.extension, UIelement.H)
                 + UIelement.HUs.PadRight(Dimentions.length, UIelement.H)
                 + UIelement.HUs.PadRight(Dimentions.creationInfo, UIelement.H)
                 + UIelement.AUL;
        }

        private string InfoText => controlByHotKey ? "<Up>,<Down> - управление курсором, <Left>, <Right> - переключение активного окна <F10> - выход"
                                                : "Введите команду";

        public DirectoryInfo LeftPath { get; set; }
        public DirectoryInfo RightPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool RightPathActive
        {
            get => rightPathActive;
            set
            {
                rightPathActive = value;

                if (!rightPathActive)
                {
                    selectedRow = selectedRow < leftList[leftPageNum].Length ? selectedRow : leftList[leftPageNum].Length;
                }
                else
                {
                    selectedRow = selectedRow < rightList[rightPageNum].Length ? selectedRow : rightList[rightPageNum].Length;
                }
            }

        } // true - активна правая колонка, false  - левая

        private int selectedRow;
        private int leftPageNum;
        private int rightPageNum;
        private bool rightPathActive; // true - активна правая область
        private bool controlByHotKey; //true - управление с помощью горячих клавирш
        private string message;

        // Следующие переменые указывают, что необходимо очистить рабочую область
        private bool cleanLeft;
        private bool cleanRight;

        private List<FileSystemInfo[]> leftList;
        private List<FileSystemInfo[]> rightList;

        public FileManager()
        {
            Dimentions = new FMdimentions(Console.WindowWidth, Console.WindowHeight);

            message = "";
            LeftPath = RightPath = new DirectoryInfo(Directory.GetCurrentDirectory());
            leftList = CreateListByPage(LeftPath);
            rightList = CreateListByPage(RightPath);

            // Отрисовка неизменяемой части окна
            Console.WriteLine(TopCornerAndPath);
            Console.WriteLine(Divider + Divider);
            Console.WriteLine(GetRow("Название", "Тип", "Размер", "Дата создания") + GetRow("Название", "Тип", "Размер", "Дата создания"));
            Console.WriteLine(Divider2 + Divider2);

            for (int i = 0; i < Dimentions.elementsPerPage + 1; i++)
            {
                Console.WriteLine(GetRow("", "", "", "") + GetRow("", "", "", ""));
            }

            Console.WriteLine(DownCornerAndPage);

        }

        private void PrintInfoPanel(string message)
        {
            string[] text = new string[Dimentions.infoPanelHeight - 2];
            message = InfoText.PadRight(Dimentions.tableWidth * 2) + message;

            for (int i = 0; i < text.Length; i++)
            {
                int charNumberPerRow = Dimentions.tableWidth * 2 - 2;
                if (message.Length > charNumberPerRow * (i + 1))
                {
                    text[i] = message.Substring(i * charNumberPerRow, charNumberPerRow -1);
                }
                else
                {
                    text[i] = message.Length > charNumberPerRow * i ? message.Substring(charNumberPerRow * i) : string.Empty;
                }
            }

            Console.SetCursorPosition(0, Dimentions.tableHeight - Dimentions.infoPanelHeight + 1);
            Console.WriteLine(UIelement.ADRs + UIelement.Hs.PadLeft(Dimentions.tableWidth * 2, UIelement.H) + UIelement.ALDs);
            foreach (var item in text)
            {
                Console.WriteLine(UIelement.V + item.PadRight(Dimentions.tableWidth * 2) + UIelement.V);
            }
            Console.WriteLine(UIelement.AUR + UIelement.Hs.PadLeft(Dimentions.tableWidth * 2, UIelement.H) + UIelement.AUL);

        }

        public void SwitchControlMode()
        {
            controlByHotKey |= true;
        }

        /// <summary>
        /// Выделить строку выше текущей
        /// </summary>
        public void SelectRowUp()
        {
            if (selectedRow > 0)
            {
                selectedRow--;
            }
            else
            {
                if (selectedRow == 0 && ((leftPageNum == 0 && !rightPathActive) || (rightPageNum == 0 && rightPathActive)))
                {
                    selectedRow--;
                }
                else
                {
                    PrevPage();
                }
            }
        }

        /// <summary>
        /// Выделить строку ниже текущей
        /// </summary>
        public void SelectRowDown()
        {
            if (!RightPathActive)
            {
                if (selectedRow < leftList[leftPageNum].Length - 1)
                {
                    selectedRow++;
                }
                else
                {
                    NextPage();
                }
            }
            else
            {
                if (selectedRow < rightList[rightPageNum].Length - 1)
                {
                    selectedRow++;
                }
                else
                {
                    NextPage();
                }
            }
        }
        /// <summary>
        /// Переход к предыдущей странице активного каталога
        /// </summary>
        public void PrevPage()
        {
            if (!RightPathActive)
            {
                if (leftPageNum > 0)
                {
                    leftPageNum--;
                    cleanLeft = true;
                    selectedRow = leftList[leftPageNum].Length - 1;
                }
            }
            else
            {
                if (rightPageNum > 0)
                {
                    rightPageNum++;
                    cleanRight = true;
                    selectedRow = rightList[rightPageNum].Length - 1;
                }
            }
        }
        /// <summary>
        /// Переход к следующей странице активного каталога
        /// </summary>
        public void NextPage()
        {
            if (!RightPathActive)
            {
                if (leftPageNum < leftList.Count - 1)
                {
                    leftPageNum++;
                    cleanLeft = true;
                    selectedRow = 0;
                }
            }
            else
            {
                if (rightPageNum < rightList.Count - 1)
                {
                    rightPageNum++;
                    cleanRight = true;
                    selectedRow = 0;
                }
            }
        }

        /// <summary>
        /// Переход к указанной странице
        /// </summary>
        /// <param name="parameters"></param>
        public void GoToPage(string[] parameters)
        {
            if (parameters.Length > 2 || parameters.Length == 0)
            {
                throw new Exception("Неверное количество параметров");
            }

            if (parameters.Length == 1)
            {
                if (!int.TryParse(parameters[0], out int pageNum) || pageNum < 0)
                {
                    throw new Exception("Неверный номер страницы");
                }

                if (!RightPathActive)
                {
                    leftPageNum = pageNum < leftList.Count ? pageNum : leftPageNum;
                }
                else
                {
                    rightPageNum = pageNum < rightList.Count ? pageNum : rightPageNum;
                }

            }
            else
            {
                string path = parameters[0].ToLower();
                int pageNum = 0;
                if ((path == "r" || path == "l") && int.TryParse(parameters[0], out pageNum) && pageNum > 0)
                {
                    if (path == "l")
                    {
                        leftPageNum = pageNum < leftList.Count ? pageNum : leftPageNum;
                    }
                    else
                    {
                        rightPageNum = pageNum < rightList.Count ? pageNum : rightPageNum;
                    }
                }
                else
                {
                    throw new Exception("Неверный параметр");
                }
            }
        }

        public void GotoParentDirectory()
        {
            if (!rightPathActive)
            {
                LeftPath = LeftPath.Parent is null ? LeftPath : LeftPath.Parent;
                cleanLeft = true;
            }
            else
            {
                RightPath = RightPath.Parent is null ? RightPath : RightPath.Parent;
                cleanRight = true;
            }

            Console.SetCursorPosition(0, 0);
            Console.WriteLine(TopCornerAndPath);
        }

        /// <summary>
        /// Изменяет отображаемый каталог
        /// </summary>
        /// <param name="parameters"></param>
        public void ChangeDirectory(string[] parameters)
        {
            if (parameters.Length > 2)
            {
                throw new Exception("Неверное количество параметров");
            }

            if (parameters.Length == 1)
            {
                if (!rightPathActive)
                {
                    try
                    {
                        LeftPath = new DirectoryInfo(parameters[0]);
                        cleanLeft = true;
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    try
                    {
                        RightPath = new DirectoryInfo(parameters[0]);
                        cleanRight = true;
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            else
            {
                string dir = parameters[0].ToLower();

                if (dir == "l" || dir == "r")
                {
                    if (dir == "l")
                    {
                        try
                        {
                            LeftPath = new DirectoryInfo(parameters[1]);
                            cleanLeft = true;
                        }
                        catch
                        {
                            throw;
                        }
                    }
                    else
                    {
                        try
                        {
                            RightPath = new DirectoryInfo(parameters[1]);
                            cleanRight = true;
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
            }
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(TopCornerAndPath);
        }

        /// <summary>
        /// Копирует каталог или папку согласно передаемым параметрам
        /// </summary>
        /// <param name="parameters"></param>
        public void Copy(string[] parameters)
        {
            if (parameters.Length == 0)
            {
                // Нет параметров  - копироваие выделенного файла или папки в другую область
                FileSystemInfo src = rightPathActive ? rightList[rightPageNum][selectedRow] : leftList[leftPageNum][selectedRow];
                string dst = rightPathActive ? LeftPath.FullName : RightPath.FullName;

                if (src.Attributes == FileAttributes.Directory)
                {
                    try
                    {
                        CopyDirectory(src.FullName, dst);
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(dst);
                        FileInfo fi = new FileInfo(src.FullName);
                        fi.CopyTo(dst);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            else
            {
                if (parameters.Length == 2)
                {
                    // 2 параметра - файл/каталог и путь для копирования
                    FileInfo src = new FileInfo(parameters[0]);
                    if (src.Exists)
                    {
                        Directory.CreateDirectory(parameters[1]);
                        src.CopyTo(Path.Combine(parameters[1], src.Name));
                    }
                    else
                    {
                        CopyDirectory(parameters[0], parameters[1]);
                    }
                }
                else
                {
                    throw new Exception("Неверное количество параметров");
                }
            }


        }

        /// <summary>
        /// Копирует папку, включая вложенные папки и файлы
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        private void CopyDirectory(string sourceDirName, string destDirName)
        {
            DirectoryInfo src = new DirectoryInfo(sourceDirName);

            if (!src.Exists)
            {
                throw new Exception("Указанной папки не существует");
            }

            DirectoryInfo[] srcDirs = src.GetDirectories();

            Directory.CreateDirectory(destDirName);

            FileInfo[] files = src.GetFiles();
            foreach (var file in files)
            {
                string path = Path.Combine(destDirName, file.Name);
                file.CopyTo(path, true);
            }

            foreach (var dir in srcDirs)
            {
                string path = Path.Combine(destDirName, dir.Name);
                CopyDirectory(dir.FullName, path);
            }
        }

        public void DeleteSelectedItem()
        {
            FileSystemInfo deletingElement = rightPathActive ? rightList[rightPageNum][selectedRow] : leftList[leftPageNum][selectedRow];
            deletingElement.Delete();
            return;
        }

        /// <summary>
        /// Удаление файла/папки
        /// </summary>
        /// <param name="parameters"></param>
        public void Delete(string[] parameters)
        {
            if (parameters.Length == 1)
            {
                if (Directory.Exists(parameters[0]))
                {
                    // Удаляем папку
                    Directory.Delete(parameters[0]);
                    return;
                }
                if (File.Exists(parameters[0]))
                {
                    // Удаляем файл
                    File.Delete(parameters[0]);
                    return;
                }
            }
            else
            {
                throw new Exception("Неверное количество параметров");
            }

            throw new Exception("Неправильный параметр");
        }

        /// <summary>
        ///  Выводит на экран рабочую область файлового менеджера
        /// </summary>
        public void Print()
        {
            leftList = CreateListByPage(LeftPath);
            rightList = CreateListByPage(RightPath);

            // Очистка экрана перед выводом
            if (cleanLeft)
            {
                for (int i = 0; i < Dimentions.elementsPerPage; i++)
                {
                    Console.SetCursorPosition(0, Dimentions.headerHeight + i);
                    Console.WriteLine(GetRow("", "", "", ""));
                }
                cleanLeft = false;
            }
            if (cleanRight)
            {
                for (int i = 0; i < Dimentions.elementsPerPage; i++)
                {
                    Console.SetCursorPosition(Dimentions.tableWidth + 1, Dimentions.headerHeight + i);
                    Console.WriteLine(GetRow("", "", "", ""));
                }
                cleanRight = false;
            }

            // Строка перехода к родительскому каталогу левой части
            Console.SetCursorPosition(1, Dimentions.headerHeight);
            if (!RightPathActive && selectedRow == -1 && controlByHotKey)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            Console.WriteLine("...");
            Console.ResetColor();

            // Строка для перехода к родительскому каталогу правой части
            Console.SetCursorPosition(Dimentions.tableWidth + 2, Dimentions.headerHeight);
            if (RightPathActive && selectedRow == -1 && controlByHotKey)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            Console.WriteLine("...");
            Console.ResetColor();

            // Вывод на экран файлов левого каталога
            for (int i = 0; i < Dimentions.elementsPerPage; i++)
            {
                try
                {
                    if (!RightPathActive && selectedRow == i && controlByHotKey)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    PrintRow(1, Dimentions.headerHeight + i + 1, leftList[leftPageNum][i]);

                    Console.ResetColor();
                }
                catch
                {
                    break;
                }
            }

            // Вывод на экран файлов правого каталога
            for (int i = 0; i < Dimentions.elementsPerPage; i++)
            {
                try
                {
                    if (RightPathActive && selectedRow == i && controlByHotKey)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    PrintRow(Dimentions.tableWidth + 2, Dimentions.headerHeight + i + 1, rightList[rightPageNum][i]);

                    Console.ResetColor();
                }
                catch
                {
                    break;
                }
            }
            Console.SetCursorPosition(0, Dimentions.headerHeight + Dimentions.elementsPerPage + 1);
            Console.WriteLine(DownCornerAndPage);
            PrintInfoPanel(message);
            message = "";

            //Console.SetCursorPosition(0, Console.WindowHeight - 2);
        }

        /// <summary>
        /// Заполнение строки значениями свойств экземпляра FileSystemInfo
        /// </summary>
        /// <param name="left">Начальная позиция курсора по горизонтали</param>
        /// <param name="top">Начальная позиция курсора по вертикали</param>
        /// <param name="element">Экземпляр FileSystemInfo</param>
        private void PrintRow(int left, int top, FileSystemInfo element)
        {
            // Заполнение колонки "Наименование"
            Console.SetCursorPosition(left, top);
            Console.WriteLine(GetCellString(element.Name, Dimentions.fileName));

            // Заполнение столбца "Тип"
            Console.SetCursorPosition(left + Dimentions.fileName, top);
            string ext = element.Attributes == FileAttributes.Directory ? "Папка" : GetCellString(element.Extension, Dimentions.extension);
            Console.WriteLine(ext);

            // Заполнение столбца "Размер"
            Console.SetCursorPosition(left + Dimentions.fileName + Dimentions.extension, top);
            try
            {
                FileInfo fi = new FileInfo(element.FullName);
                Console.WriteLine(FileLengthText(fi.Length));
            }
            catch { }

            // Заполнение столбца "Создан"
            Console.SetCursorPosition(left + Dimentions.fileName + Dimentions.extension + Dimentions.length, top);
            Console.WriteLine(element.CreationTime.ToString());
        }

        /// <summary>
        ///  Возвращает список массивов FileSystemInfo указанного каталога. Каждый массив содержит все папки и каталоги страницы
        /// </summary>
        /// <param name="di"></param>
        /// <returns></returns>
        public List<FileSystemInfo[]> CreateListByPage(DirectoryInfo di)
        {
            List<FileSystemInfo[]> result = new List<FileSystemInfo[]>();
            Queue<FileSystemInfo> elements = new Queue<FileSystemInfo>(di.GetFileSystemInfos());

            while (elements.Count > 0)
            {
                int arraySize = elements.Count > Dimentions.elementsPerPage ? Dimentions.elementsPerPage : elements.Count;
                FileSystemInfo[] currPage = new FileSystemInfo[arraySize];

                for (int i = 0; i < currPage.Length; i++)
                {
                    currPage[i] = elements.Dequeue();
                }

                result.Add(currPage);
            }

            return result;

        }

        /// <summary>
        /// Формирует список массивов строк с файлами и папками для файлового менеджера
        /// </summary>
        /// <param name="di"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<string[]> CreateDirectoryList(DirectoryInfo di)
        {
            var fileList = di.GetFiles();
            var dirList = di.GetDirectories();

            List<string[]> table = new List<string[]>();

            Queue<string> globalList = new Queue<string>();

            for (int i = 0; i < dirList.Length; i++)
            {
                string row = GetRow(dirList[i].Name, "Папка", " ", dirList[i].CreationTime.ToShortDateString());
                globalList.Enqueue(row);
            }

            for (int i = 0; i < fileList.Length; i++)
            {
                string row = GetRow(fileList[i].Name, fileList[i].Extension, fileList[i].Length.ToString(), fileList[i].CreationTime.ToShortDateString());
                globalList.Enqueue(row);
            }

            while (true)
            {
                string[] currentPage = new string[Dimentions.tableHeight];

                if (globalList.Count < Dimentions.tableHeight)
                {
                    int i = 0;

                    while (globalList.Count > 0)
                    {
                        currentPage[i++] = globalList.Dequeue();
                    }

                    for (int j = i; j < currentPage.Length; j++)
                    {
                        currentPage[j] = GetRow("", "", "", "");
                    }

                    table.Add(currentPage);
                    break;
                }
                else
                {
                    for (int i = 0; i < currentPage.Length; i++)
                    {
                        currentPage[i] = globalList.Dequeue();
                    }

                    table.Add(currentPage);
                }
            }


            return table;
        }

        /// <summary>
        /// Сокращает строку до выбранной длинны
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        private static string GetCellString(string str, int maxLength)
        {
            if (str.Length <= maxLength)
            {
                return str/*.PadRight(maxLength)*/;
            }

            if (maxLength < 6)
            {
                return str.Substring(0, maxLength);
            }

            return str.Substring(0, maxLength / 2 - 1) + ".." + str.Substring(str.Length - (maxLength / 2 + 1));
        }

        private string GetRow(string filename, string extension, string length, string creationDate)
        {
            return UIelement.V + GetCellString(filename, Dimentions.fileName - 1).PadRight(Dimentions.fileName - 1)
                 + UIelement.V + GetCellString(extension, Dimentions.extension - 1).PadRight(Dimentions.extension - 1)
                 + UIelement.V + GetCellString(length, Dimentions.length - 1).PadRight(Dimentions.length - 1)
                 + UIelement.V + GetCellString(creationDate, Dimentions.creationInfo - 1).PadRight(Dimentions.creationInfo - 1)
                 + UIelement.V;
        }

        private string FileLengthText(long lengthBytes)
        {
            double lengthKBytes = (double)lengthBytes / 1024;

            if (lengthKBytes < 1024)
            {
                return $"{lengthKBytes:F2} Кб";
            }
            else
            {
                double lengthMBytes = lengthKBytes / 1024;
                if (lengthMBytes < 1024)
                {
                    return $"{lengthMBytes:F2} Мб";
                }
                else
                {
                    double lengthGBytes = lengthMBytes / 1024;
                    if (lengthMBytes < 1024)
                    {
                        return $"{lengthGBytes:F2} Гб";
                    }
                    else
                    {
                        return $"{lengthGBytes / 1024:F2} Тб";
                    }
                }
            }

        }

        public void ExecuteCommand(string command)
        {
            string[] arg = command.Split("--");
            string methodName = arg[0].Trim();

            try
            {
                MethodInfo method = this.GetType().GetMethod(methodName);

                if (method is null)
                {
                    throw new Exception("Неизвестная комманда");
                }

                if (arg.Length > 1)
                {
                    arg = arg.Where(x => x.Trim() != methodName).ToArray();

                    method.Invoke(this, new[] { arg });
                }
                else
                {
                    method.Invoke(this, null);
                }
            }
            catch (Exception e)
            {
                message = "Ошибка! " + e.Message;
            }
        }

        public string UserInputHelp()
        {
            var methods = this.GetType().GetMethods();
            string[] methodNames = new string[methods.Length];
            string userInputPrev = "";

            for (int i = 0; i < methods.Length; i++)
            {
                methodNames[i] = methods[i].Name;
            }

            while (true)
            {
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }

                string userInput = userInputPrev;

                if (char.IsLetterOrDigit(key.KeyChar))
                {
                    userInput += key.KeyChar;
                }


                int cursorPositionLeft = Console.CursorLeft;
                int cursorPositionTop = Console.CursorTop;

                List<string> tmpList = new List<string>();
                string helpRow = "Возможно, вы имели ввиду: ";

                foreach (string methodName in methodNames)
                {
                    if (methodName.StartsWith(userInput))
                    {
                        helpRow += methodName + " ";
                    }
                }

                PrintInfoPanel(helpRow);
                Console.SetCursorPosition(cursorPositionLeft, cursorPositionTop);

                userInputPrev = userInput;

            }
            return userInputPrev;
        }

        private static string[][] AvaiableMethedsInfo = { new[] { "SwitchControlMode - переключение типа управления" },
                                                          new[] { "ChangeDirectory - смена каталога", "Параметры: r - правое окно, l - левое окно", "Путь нового каталога"},
                                                          new[] {};
    }
}
