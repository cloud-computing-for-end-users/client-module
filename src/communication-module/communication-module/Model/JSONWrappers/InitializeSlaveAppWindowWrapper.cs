namespace Core.ImageReceiver
{
    internal class InitializeSlaveAppWindowWrapper : SlaveKeyWrapper
    {
        public string PathToImages { get; set; }
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
    }
}
