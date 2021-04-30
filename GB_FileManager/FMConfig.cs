using System;
using System.Text.Json.Serialization;

namespace GB_FileManager
{
    [Serializable]
    public class FMConfig
    {
        public string LeftPathString { get; set; }
        public string RightPathString { get; set; }

        public int ExtensionColumn { get; set; }
        public int CreationInfoColumn { get; set; }
        public int LengthColumn { get; set; }
        public int InfoPanelHeight { get; set; }
        public int ElementsPerPage { get; set; }

        [JsonIgnore]
        public int HeaderHeight { get => 4; }
        [JsonIgnore]
        public int CommandLineHeight { get => 1; }
        [JsonIgnore]
        public int TableHeight { get => ElementsPerPage + HeaderHeight + CommandLineHeight + InfoPanelHeight; }
        [JsonIgnore]
        public int TableWidth { get => (Console.WindowWidth - 10) / 2; }
        [JsonIgnore]
        public int FileNameColumn
        {
            get
            {
                int tmp = TableWidth - ExtensionColumn - CreationInfoColumn - LengthColumn;
                return (tmp > 0) ? tmp : 20;
            }
        }

        public FMConfig()
        {

        }
    }
}
