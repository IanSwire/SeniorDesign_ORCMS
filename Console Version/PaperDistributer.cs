using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SeniorDesign
{
    static class PaperDistributer {
        public static void DistributePapers(List<Track> tracks, List<Paper> listOfPapers, List<Reviewer> listOfReviewers)
        {
            Dictionary<Track, List<Paper>> m_papers = new Dictionary<Track, List<Paper>>();
            Dictionary<Track, List<Reviewer>> m_reviewers = new Dictionary<Track, List<Reviewer>>();

            Console.WriteLine("Enter the number of reviewers per paper: ");
            int n = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter the max papers per reviewer: ");
            int m = Convert.ToInt32(Console.ReadLine());

            // Sets Up local varibles
            foreach(var track in tracks)
            {
                var paperList = new List<Paper>();
                var reviewerList = new List<Reviewer>();
                foreach (var paper in listOfPapers)
                {
                    if (paper.track.track_name == track.track_name)
                        paperList.Add(paper);
                }
                foreach(var reviewer in listOfReviewers)
                {
                    if (reviewer.track.track_name == track.track_name)
                        reviewerList.Add(reviewer);
                }
                m_papers.Add(track, paperList);
                m_reviewers.Add(track, reviewerList);
            }

            var outReviewAssignments = new List<Reviewer>();
            // Distributes papers
            foreach (var track in tracks)
            {
                var papersInTrack = m_papers[track];
                List<Paper> papersToDistribute = new List<Paper>();
                for (int i = 0; i < n; i++) //add the papers to the stack that are in this track
                {
                    foreach (var paper in papersInTrack)
                        papersToDistribute.Add(paper);
                }

                var reviewers = m_reviewers[track];  //get the list of people to reviewers in this track
                foreach (var paper in papersToDistribute) //foreach in paper in papers to distribute 
                {
                    //Console.WriteLine($"Trying to add {paper.Title}");
                    bool _continue = false;
                    while (!_continue)
                    {
                        Random rnd = new Random();
                        var Ri = rnd.Next(0, reviewers.Count);
                        //Console.WriteLine($"Trying to add to Reviewer {reviewers[Ri].Name}");
                        if (reviewers[Ri].numOfPapers < m)
                        {
                            bool add = true;
                            foreach (var _paper in reviewers[Ri].reviewPapers)
                                if (_paper.Title == paper.Title)
                                    add = false;

                            if (add)
                            {
                                //Console.WriteLine($"Adding {paper.Title} to {reviewers[Ri].Name}");
                                reviewers[Ri].reviewPapers.Add(paper);
                                reviewers[Ri].numOfPapers += 1;
                                _continue = true;
                            }
                        }
                    }
                }

                // Displays the finished paper assignments
                Console.WriteLine($"Paper distribution for {track}");
                foreach (var review in reviewers)
                {
                    Console.WriteLine($"Reviewer: {review.Name} : {review.ID}");
                    foreach (var paper in review.reviewPapers)
                        Console.WriteLine($"    {paper.Title}");

                    outReviewAssignments.Add(review);
                }
            }
            var jsonString = JsonConvert.SerializeObject(outReviewAssignments, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText($"paper_distribution.json", jsonString);
        }
    }
}
