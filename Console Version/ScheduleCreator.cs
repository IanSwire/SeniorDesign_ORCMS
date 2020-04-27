using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("Day ID");
            dt.Columns.Add("Track");
            dt.Columns.Add("Session ID");
            dt.Columns.Add("Position ID");
            dt.Columns.Add("Speaker Name");
            dt.Columns.Add("Paper Name");
            foreach (var pairTrackSchedule in m_Schedule)
            {
                int dayCount = 0;
                foreach (var day in pairTrackSchedule.Value.schedule)
                {
                    int sessionID = 0;
                    foreach(var session in day)
                    {
                        int speakerPosition = 0;
                        foreach(var paper in session.speakers)
                        {
                            DataRow newRow = dt.NewRow();
                            newRow["Day ID"] = $"{dayCount}";
                            newRow["Track"] = $"{pairTrackSchedule.Key}";
                            newRow["Session ID"] = $"{sessionID}";
                            newRow["Position ID"] = $"{speakerPosition}";
                            newRow["Speaker Name"] = $"{paper.Author}";
                            newRow["Paper Name"] = $"{paper.Title}";
                            dt.Rows.Add(newRow);
                            speakerPosition++;
                        }
                        sessionID++;
                    }
                    dayCount++;
                }
            }
            
            StringBuilder sb = new StringBuilder();
            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));
            StringBuilder sb2 = new StringBuilder();
            sb2.Append("<html><style>table, th, td {padding: 10px;border: 1px solid black;border-collapse: collapse;}</style><h1>Schedule</h1><table><tr><th>Day</th><th>Track</th><th>Session ID</th><th>Speaking Order</th><th>Speaker Name</th><th>Speaker Paper</th></tr>");
            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));

                sb2.AppendLine("<tr><td>");
                sb2.AppendLine(string.Join("</td><td>", fields));
                sb2.AppendLine("</td></tr>");
            }
            File.WriteAllText("confernceSchedule.csv", sb.ToString());

            sb2.Append("</table></html>");
            File.WriteAllText("confernceSchedule.html", sb2.ToString());

            var jsonString = JsonConvert.SerializeObject(m_Schedule, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText($"confernceSchedule.json", jsonString);
        }
        /*public void SaveSchedule(string fileName)
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
        }*/
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
