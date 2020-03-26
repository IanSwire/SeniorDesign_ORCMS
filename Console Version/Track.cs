using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign
{
    class Track
    {
        public string track_name { get; set; }
        public Track(string t)
        {
            track_name = t;
        }
        public override string ToString()
        {
            return track_name.ToString();
        }
    }
}
