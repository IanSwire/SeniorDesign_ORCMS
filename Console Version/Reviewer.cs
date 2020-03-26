using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign
{
    class Reviewer
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public Track track { get; set; }
        public int numOfPapers { get; set; }
        public List<Paper> reviewPapers { get; set; }
        public Reviewer(string name, int id, Track t, int numofpapers, List<Paper> rP)
        {
            Name = name;
            ID = id;
            track = t;
            numOfPapers = numofpapers;
            reviewPapers = rP;
        }
    }
}
