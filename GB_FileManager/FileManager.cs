using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace GB_FileManager
{
    class FileManager
    {
        public FMdimentions Dimentions { get; set; }

        public string TopCornerAndPath 
        { 
            get => UIelement.ADRs.PadRight(Console.WindowWidth / 4 - LeftPath.FullName.Length / 2 + 1, UIelement.H)
                 + LeftPath.FullName
                 + UIelement.ALDs.PadLeft(Console.WindowWidth / 4 - LeftPath.FullName.Length / 2, UIelement.H)
                 + UIelement.ADRs.PadRight(Console.WindowWidth / 4 - LeftPath.FullName.Length / 2 + 1, UIelement.H)
                 + RightPath.FullName
                 + UIelement.ALDs.PadLeft(Console.WindowWidth / 4 - LeftPath.FullName.Length / 2, UIelement.H); }

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

        private int selectedRow;
        private int leftPageNum;
        private int rightPageNum;
        private bool rightPathActive; // true - активна правая колонка, false  - левая

        private List<FileSystemInfo[]> leftList;
        private List<FileSystemInfo[]> rightList;

        public FileManager()
        {
            Dimentions = new FMdimentions(Console.WindowWidth, Console.WindowHeight);

            LeftPath = RightPath = new DirectoryInfo(Directory.GetCurrentDirectory());

            leftList = CreateListByPage(LeftPath);
            rightList = CreateListByPage(RightPath);

        }

        public void PageUp()
        {
            if (!rightPathActive)
            {
                leftPageNum = leftPageNum > 0 ? leftPageNum - 1 : 0;
            }
            else
            {
                rightPageNum = rightPageNum > 0 ? rightPageNum - 1 : 0;
            }
        }

        public void PageDown()
        {
            if (!rightPathActive)
            {
                leftPageNum = leftPageNum < leftList.Count ? leftPageNum + 1 : leftList.Count + 1;
            }
            else
            {
                rightPageNum = rightPageNum < rightList.Count ? rightPageNum + 1 : rightPageNum + 1;
            }
        }

        /// <summary>
        ///  Выводит на экран рабочую область файлового менеджера
        /// </summary>
        public void Print()
        {


            Console.WriteLine(TopCornerAndPath);
            Console.WriteLine(Divider + Divider);
            Console.WriteLine(GetRow("Название", "Тип", "Размер", "Дата создания") + GetRow("Название", "Тип", "Размер", "Дата создания"));
            Console.WriteLine(Divider2 + Divider2);

            for (int i = 0; i < Dimentions.tableHeight; i++)
            {
                Console.WriteLine(GetRow("", "", "", "") + GetRow("", "", "", ""));
            }

            Console.WriteLine(BottomCorner + BottomCorner);

        }

        /// <summary>
        ///  Возвращает список массивов FileSystemInfo указанного каталога
        /// </summary>
        /// <param name="di"></param>
        /// <returns></returns>
        public List<FileSystemInfo[]> CreateListByPage(DirectoryInfo di)
        {
            List<FileSystemInfo[]> result = new List<FileSystemInfo[]>();
            Queue<FileSystemInfo> elements = new Queue<FileSystemInfo>(di.GetFileSystemInfos());

            while (elements.Count > 0)
            {
                int arraySize = elements.Count > Dimentions.tableHeight ? Dimentions.tableHeight : elements.Count;
                FileSystemInfo[] currPage = new FileSystemInfo[arraySize];

                for (int i = 0; i < currPage.Length; i++)
                {
                    currPage[i] = elements.Dequeue();
                }
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
                return str.PadRight(maxLength);
            }

            if (maxLength < 6)
            {
                return str.Substring(0, maxLength);
            }

            return str.Substring(0, maxLength / 2 - 1) + ".." + str.Substring(str.Length - (maxLength / 2 + 1));
        }

        private string GetRow(string filename, string extension, string length, string creationDate)
        {
            return UIelement.V + GetCellString(filename, Dimentions.fileName - 1)
                 + UIelement.V + GetCellString(extension, Dimentions.extension - 1)
                 + UIelement.V + GetCellString(length, Dimentions.length - 1)
                 + UIelement.V + GetCellString(creationDate, Dimentions.creationInfo - 1)
                 + UIelement.V;
        }
    }
}
