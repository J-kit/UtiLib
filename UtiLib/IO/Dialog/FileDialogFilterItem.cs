namespace UtiLib.IO.Dialog
{
    public struct FileDialogFilterItem
    {
        public string Description { get; set; }
        public string Extension { get; set; }

        public static FileDialogFilterItem Create(string extension, string description)
        {
            return new FileDialogFilterItem { Extension = extension, Description = description };
        }
    }
}