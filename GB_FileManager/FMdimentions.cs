namespace GB_FileManager
{
    public struct FMdimentions
    {
        public int fileName;
        public int extension;
        public int creationInfo;
        public int length;
        public int tableHeight;
        public int tableWidth;
        public int infoPanelHeight;
        public int commandLineHeight;
        public int headerHeight;
        public int elementsPerPage;

        public FMdimentions(int windowWidth, int windowHeight)
        {
            extension = 10;
            creationInfo = 20;
            length = 15;
            infoPanelHeight = 7;
            commandLineHeight = 1;
            headerHeight = 4;
            elementsPerPage = 20;

            tableWidth = (windowWidth - 10) / 2;
            int tmp = tableWidth - extension - creationInfo - length;
            fileName = (tmp > 0) ? tmp : 20;
            tableHeight = elementsPerPage + headerHeight + commandLineHeight + infoPanelHeight;
        }
    }
}
