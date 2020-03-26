using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;


namespace SeniorDesign
{
    class Program
    {
        private List<Track> tracks = new List<Track>();
        private List<Paper> listOfPapers = new List<Paper>();
        private List<Reviewer> listOfReviewers = new List<Reviewer>();
        private List<ConferenceEvent> events = new List<ConferenceEvent>();
        private ScheduleCreator scheduleCreator = new ScheduleCreator();
        private string databasePath = "";
        private int event_id = -1;
        static void Main(string[] args)
        {
            var p = new Program();
            while (true)
            {
                Console.WriteLine("Hello and welcome to Open Review Confernce Management Software.");
                Console.WriteLine("If you need help at anytime, just enter help to view a list of all the commands.");
                p.SetPath();
                p.SelectEvent();
                p.LoadDatabase();
                p.EventManagement();
            }
        }

        public void SetPath()
        {
            Console.WriteLine("Please enter the path to the SQLITE database that you wish to work with or enter new to start with a new database.");
            var userInput = Console.ReadLine().Trim();
            if(userInput.ToLower() == "new")
            {

            }
            else
                databasePath = $"Data Source = {userInput}; Version = 3;";
        }
        public void SelectEvent()
        {
            GetEvents();
            Console.WriteLine("Please select and event from the list of events below or enter new to start creating a new event.");
            for(int i = 1; i <= events.Count; i++)
            {
                Console.WriteLine($" {i}) {events[i - 1].eventName}");
            }
            var userInput = Console.ReadLine();
            if(userInput.ToLower() == "new")
            {

            }
            else if(Convert.ToInt32(userInput) < events.Count + 1 && Convert.ToInt32(userInput) > 0)
            {
                event_id = events[Convert.ToInt32(userInput) - 1].eventID;
            }
        }
        public void GetEvents()
        {
            events.Clear();
            using (SQLiteConnection myConnection = new SQLiteConnection(databasePath))
            {
                string getT = $"SELECT name,event_ID FROM event";
                SQLiteCommand oCmd = new SQLiteCommand(getT, myConnection);
                myConnection.Open();
                using (SQLiteDataReader oreader = oCmd.ExecuteReader())
                {
                    while (oreader.Read())
                    {
                        var name = oreader.GetString(0);
                        var id = oreader.GetInt32(1);
                        events.Add(new ConferenceEvent(name, id));
                    }
                }
            }
        }
        public void EventManagement()
        {
            while (true)
            {
                Console.WriteLine("Please select one of the following actions below:");
                Console.WriteLine(" 1) Distribute papers to reviews");
                Console.WriteLine(" 2) Create conference schedule");
                var userInput = Console.ReadLine().Trim();
                switch (userInput)
                {
                    case "1":
                        Console.WriteLine("Distribute papers to reviews has been selected.");
                        PaperDistributer.DistributePapers(tracks, listOfPapers, listOfReviewers);
                        break;
                    case "2":
                        Console.WriteLine("Create conference schedule has been selected.");
                        scheduleCreator.CreateSchedule(tracks, listOfPapers, 1, 2, 2);
                        scheduleCreator.SaveSchedule();
                        break;
                    default:
                        Console.WriteLine("The input you have entered does not match any of the choices.");
                        break;
                }
                Console.WriteLine();
            }
        }

        public void LoadDatabase()
        {
            LoadTracks();
            LoadSubmissions();
            LoadReviewers();
        }
        private void LoadTracks() {
            using (SQLiteConnection myConnection = new SQLiteConnection(databasePath))
            {
                string getT = $"SELECT track_name FROM track WHERE event_ID={event_id}";
                SQLiteCommand oCmd = new SQLiteCommand(getT, myConnection);
                myConnection.Open();
                using (SQLiteDataReader oreader = oCmd.ExecuteReader())
                {
                    while (oreader.Read())
                    {
                        var name = oreader.GetString(0);
                        tracks.Add(new Track(name));
                    }
                }
            }
        }
        private void LoadSubmissions() {
            using (SQLiteConnection myConnection = new SQLiteConnection(databasePath))
            {
                string getP = $"SELECT title,given_name,track_name,rank FROM submission,track,human WHERE submission.track_ID = track.track_ID AND track.event_ID={event_id} AND human.human_ID = submitting_human_ID";
                SQLiteCommand pCmd = new SQLiteCommand(getP, myConnection);
                myConnection.Open();
                using (SQLiteDataReader preader = pCmd.ExecuteReader())
                {
                    while (preader.Read())
                    {
                        var title = preader.GetString(0);
                        var author = preader.GetString(1);
                        var track = preader.GetString(2);
                        var rank = preader.GetInt32(3);

                        Track _track = null;
                        foreach (var __track in tracks)
                        {
                            if (__track.track_name == track)
                                _track = __track;
                        }

                        listOfPapers.Add(new Paper(title, author, _track, rank));
                    }
                }
            }
        }
        private void LoadReviewers()
        {
            using (SQLiteConnection myConnection = new SQLiteConnection(databasePath))
            {
                string getR = $"SELECT given_name,reviewer_ID,track_name FROM reviewers,track,human WHERE reviewers.track_ID = track.track_ID AND track.event_ID={event_id} AND human.human_ID = reviewers.human_id";
                SQLiteCommand rCmd = new SQLiteCommand(getR, myConnection);
                myConnection.Open();
                using (SQLiteDataReader rreader = rCmd.ExecuteReader())
                {
                    while (rreader.Read())
                    {
                        var name = rreader.GetString(0);
                        var id = rreader.GetInt32(1);
                        var track = rreader.GetString(2);

                        Track _track = null;
                        foreach (var __track in tracks)
                        {
                            if (__track.track_name == track)
                                _track = __track;
                        }

                        listOfReviewers.Add(new Reviewer(name, id, _track, 0, new List<Paper>()));
                    }
                }
            }
        }
    }
}
