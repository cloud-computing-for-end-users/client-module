namespace Core.ImageReceiver
{
    internal class RenameFileWrapper : PrimaryKeyWrapper
    {
        public string OldFileName { get; set; }
        public string NewFileName { get; set; }
    }
}