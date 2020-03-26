using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace SeniorDesign
{
    class ScheduleCreator
    {
        public Dictionary<Track, Schedule> m_Schedule;
        #region Constructors
        public ScheduleCreator() {
            m_Schedule = new Dictionary<Track, Schedule>();
        }
        public ScheduleCreator(Dictionary<Track, Schedule> schedule)
        {
            m_Schedule = schedule;
        }
        #endregion Constructors
        #region Public Methods
        #region Get Schedule
        public Dictionary<Track, Schedule> GetSchedule()
        {
            return m_Schedule;
        }
        public Schedule GetSchedule(Track track)
        {
            return m_Schedule[track];
        }
        #endregion Get Schedule
        #region Save Schedule
        public void SaveSchedule()
        {
            var jsonString = JsonConvert.SerializeObject(m_Schedule, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText($"confernceSchedule.json", jsonString);
        }
        public void SaveSchedule(string fileName)
        {
            var jsonString = JsonConvert.SerializeObject(m_Schedule, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText($"{fileName}.json", jsonString);
        }
        public void SaveSchedule(Track track)
        {
            var jsonString = JsonConvert.SerializeObject(m_Schedule[track], Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText($"confernceSchedule_{track}.json", jsonString);
        }
        public void SaveSchedule(string fileName, Track track)
        {
            var jsonString = JsonConvert.SerializeObject(m_Schedule[track], Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText($"{fileName}.json", jsonString);
        }
        #endregion Save Schedule
        public void CreateSchedule(List<Track> tracks, List<Paper> papers, int numOfDays, int numOfSessionsPerDay, int numOfPapersPerSession)
        {
            var totalSessions = numOfDays * numOfSessionsPerDay;
            foreach (var track in tracks)
            {
                Schedule schedule = new Schedule();
                List<Paper> papersInTrack = new List<Paper>();
                foreach (var paper in papers)
                {
                    if (paper.track == track)
                        papersInTrack.Add(paper);
                }
                papersInTrack.Sort(new ComparePapers());

                Queue<Paper> sortedPapers = new Queue<Paper>();
                foreach (var paper in papersInTrack)
                    sortedPapers.Enqueue(paper);

                Queue<Paper> bestPapers = new Queue<Paper>();
                for (int i = 0; i < totalSessions; i++)
                    bestPapers.Enqueue(sortedPapers.Dequeue());

                Queue<Paper> betterPapers = new Queue<Paper>();
                for (int i = 0; i < totalSessions; i++)
                    betterPapers.Enqueue(sortedPapers.Dequeue());

                Queue<Paper> goodPapers = new Queue<Paper>();
                for (int i = 0; i < totalSessions * (numOfPapersPerSession - 2); i++)
                    goodPapers.Enqueue(sortedPapers.Dequeue());

                for (int i = 0; i < numOfDays; i++)
                {
                    for (int j = 0; j < numOfSessionsPerDay; j++)
                    {
                        List<Paper> sessionPapers = new List<Paper>();
                        sessionPapers.Add(betterPapers.Dequeue());
                        for (int k = 0; k < (numOfPapersPerSession - 2); k++)
                            sessionPapers.Add(goodPapers.Dequeue());
                        sessionPapers.Add(bestPapers.Dequeue());
                        schedule.SetSession(i, j, sessionPapers);
                    }
                }
                m_Schedule.Add(track, schedule);
            }
        }
        #endregion Public Methods
    }
}
