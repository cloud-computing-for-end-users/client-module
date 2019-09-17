namespace Core
{
    class CommsWrapper
    {
        public ExternalComms ExternalComms { get; set; }
        public InternalComms InternalComms { get; set; }

        public CommsWrapper()
        {
            ExternalComms = new ExternalComms();
            InternalComms = new InternalComms(ExternalComms.comm);
        }

        public void Start() { }
    }
}
