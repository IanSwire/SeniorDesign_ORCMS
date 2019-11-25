using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace OpenReviewConferenceManagementSoftware.utils
{
    class Schedule
    {
        public int event_id { get; set; }  // Get from database field
        public DateTime lastUpdate { get; set; }  // Get from database field
        public Dictionary<string, ScheduleDay> _days { get; set; }  // Get from database JSON

        public Schedule(int id, string[] days, string[] jsons)
        {
            event_id = id;
            lastUpdate = DateTime.Now;

            int i = 0;
            foreach(var day in days)
            {
                ScheduleDay _schedule = JsonConvert.DeserializeObject<ScheduleDay>(jsons[i]);
                _days.Add(day, _schedule);
                i++;
            }
        }
    }

    class ScheduleDay
    {
        public Dictionary<string, ScheduleTrack[]> tracks { get; set; }
    }

    class ScheduleTrack
    {
        public IList<ScheduleItem> trackSchedule { get; set; }
    }

    class ScheduleItem
    {
        public string speakerName { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string description { get; set; }
    }
}
