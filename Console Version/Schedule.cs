using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign
{
    class Schedule
    {
        public List<List<Session>> schedule;
        public Schedule(int numOfDays = 2, int numOfSessionsPerDay = 4)
        {
            schedule = new List<List<Session>>();
            for (int i = 0; i < numOfDays; i++)
            {
                var day = new List<Session>();
                for (int j = 0; j < numOfSessionsPerDay; j++)
                    day.Add(new Session());
                schedule.Add(day);
            }
        }
        public void SetSession(int day, int sessionIndex, List<Paper> papers)
        {
            schedule[day][sessionIndex].speakers = papers;
        }
    }
}
