namespace Core.ImageReceiver
{
    internal class GetImagesFromSlaveWrapper : PrimaryKeyWrapper
    {
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public string RunningOnOperatingSystem { get; set; }
    }
}
