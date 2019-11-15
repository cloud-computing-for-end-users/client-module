using System;

namespace Core.ImageReceiver
{
    internal class MouseUpAndDownWrapper : MouseMoveWrapper
    {
        public string Button { get; set; }
        public bool Down { get; set; }
    }
}