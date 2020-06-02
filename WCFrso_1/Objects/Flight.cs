using System;

namespace Host.Objects
{
    public class Flight
    {    
        public string PortA { get; set; }
        
        public TimeSpan TimeA { get; set; }

        public string PortB { get; set; }

        public TimeSpan TimeB { get; set; }
    }
}
