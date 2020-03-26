using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign
{
    class ConferenceEvent
    {
        public string eventName { get; set; }
        public int eventID { get; set; }
        public ConferenceEvent(string name, int id)
        {
            eventName = name;
            eventID = id;
        }
    }
}
