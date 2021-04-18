using System;
using System.Collections.Generic;
using System.IO;

namespace GB_FileManager
{
    class FileManager
    {
        public FMdimentions Dimentions { get; set; }

        public string TopCornerAndPath => UIelement.ADR + "Path 1: "
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

        public string Divider2
        {
            get => UIelement.VRs.PadRight(Dimentions.fileName, UIelement.H)
                 + UIelement.VHs.PadRight(Dimentions.extension, UIelement.H)
                 + UIelement.VHs.PadRight(Dimentions.length, UIelement.H)
                 + UIelement.VHs.PadRight(Dimentions.creationInfo, UIelement.H)
                 + UIelement.VL;
        }

        public string BottomCorner
        {
            get => UIelement.AURs.PadRight(Dimentions.fileName, UIelement.H)
                 + UIelement.HUs.PadRight(Dimentions.extension, UIelement.H)
                 + UIelement.HUs.PadRight(Dimentions.length, UIelement.H)
                 + UIelement.HUs.PadRight(Dimentions.creationInfo, UIelement.H)
                 + UIelement.AUL;
        }

        public DirectoryInfo LeftPath { get; set; }
        public DirectoryInfo RightPath { get; set; }
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
        private bool rightPathActive;

        // Следующие переменые указывают, что необходимо очистить рабочую область
        private bool cleanLeft;
        private bool cleanRight;

        private List<FileSystemInfo[]> leftList;
        private List<FileSystemInfo[]> rightList;

        public FileManager()
        {
            Dimentions = new FMdimentions(Console.WindowWidth, Console.WindowHeight);

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
                    if (selectedRow == 0 && ((leftPageNum == 0 && !rightPathActive)||(rightPageNum == 0 && rightPathActive)))
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
            if (!RightPathActive && selectedRow == -1)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            Console.WriteLine("...");
            Console.ResetColor();

            // Строка для перехода к родительскому каталогу правой части
            Console.SetCursorPosition(Dimentions.tableWidth + 2, Dimentions.headerHeight);
            if (RightPathActive && selectedRow == -1)
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
                    if (!RightPathActive && selectedRow == i)
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
                    if (RightPathActive && selectedRow == i)
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

            Console.SetCursorPosition(0, Console.WindowHeight - 2);
        }

        public void Delete(string[] parameters)
        {
            if (parameters.Length == 0) // Нет параметров - Удалить выденный файл/папку
            {
                FileSystemInfo deletingElement = rightPathActive ? rightList[rightPageNum][selectedRow] : leftList[leftPageNum][selectedRow];
                deletingElement.Delete();
                return;
            }

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
                }
            }
            else
            {
                throw new Exception("Неверное количество параметров");
            }

            throw new Exception("Неправильный параметр");
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
    }
}
