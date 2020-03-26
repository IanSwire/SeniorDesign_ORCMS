using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign
{
    class Paper
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public Track track { get; set; }
        public int numReviewers { get; set; }
        public int Rank { get; set; }
        public Paper(string title, string author, Track t, int numreviewers, int rank)
        {
            Title = title;
            Author = author;
            track = t;
            numReviewers = numreviewers;
            Rank = rank;
        }
    }
}
