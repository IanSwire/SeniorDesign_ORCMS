using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
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
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                p.event_id = -1;
                Console.WriteLine("Hello and welcome to Open Review Confernce Management Software.");
                Console.WriteLine("If you need help at anytime, just enter help to view a list of all the commands.");
                p.SetPath();
                p.SelectEvent();
                p.LoadDatabase();
                p.EventManagement();
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine();
            }
        }

        public void SetPath()
        {
            Console.WriteLine("Please enter the path to the SQLITE database that you wish to work with or enter new to start with a new database excluding .sqlite.");
            var userInput = Console.ReadLine().Trim();
            if(userInput.ToLower() == "new")
            {
                Console.WriteLine("Please enter the name you would like to assign to this SQLITE version 3 database or the complete file path excluding .sqlite.");
                userInput = Console.ReadLine().Trim();
                SQLiteConnection.CreateFile($"{userInput}.sqlite");
                databasePath = $"Data Source = {userInput}.sqlite; Version = 3;";
                using (SQLiteConnection myConnection = new SQLiteConnection(databasePath))
                {
                    myConnection.Open();
                    SQLiteCommand cmd = new SQLiteCommand(myConnection);
                    cmd.CommandText = "CREATE TABLE author (author_ID INTEGER PRIMARY KEY,human_ID INTEGER,submission_ID INTEGER NOT NULL,contact_author INTEGER DEFAULT 1,FOREIGN KEY (submission_ID) REFERENCES submission,FOREIGN KEY (human_ID) REFERENCES human,UNIQUE (human_ID, submission_ID));";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE event (event_ID INTEGER PRIMARY KEY,name TEXT,year INTEGER,number INTEGER,venue_ID INTEGER,deadline_abstract TEXT,deadline_submissions TEXT,deadline_reviews TEXT,deadline_answers TEXT,deadline_withdrawal TEXT,deadline_deletion_reviews TEXT,deadline_deletion_answer TEXT,FOREIGN KEY (venue_ID) REFERENCES venue);";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE human (human_ID INTEGER PRIMARY KEY,login TEXT,given_name TEXT NOT NULL,family_name TEXT,email TEXT NOT NULL);";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE reviewers (reviewer_id INTEGER NOT NULL,human_id INTEGER NOT NULL,track_ID INTEGER NOT NULL,CONSTRAINT reviewers_PK PRIMARY KEY (reviewer_id),CONSTRAINT reviewers_FK FOREIGN KEY (human_id) REFERENCES human(human_ID) ON DELETE CASCADE ON UPDATE CASCADE,CONSTRAINT reviewers_FK_1 FOREIGN KEY (track_ID) REFERENCES track(track_ID) ON DELETE CASCADE ON UPDATE CASCADE);";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE submission (submission_ID INTEGER PRIMARY KEY,event_ID INTEGER NOT NULL,submitting_human_ID INTEGER NOT NULL,submission_file_name TEXT,title TEXT NOT NULL,abstract TEXT NOT NULL,track_ID INTEGER DEFAULT(0),date_creation TEXT NOT NULL, \"rank\" INTEGER DEFAULT -1,UNIQUE (event_ID, title, submitting_human_ID),FOREIGN KEY (submitting_human_ID) REFERENCES human,FOREIGN KEY (track_ID) REFERENCES track);";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE track (track_ID INTEGER PRIMARY KEY,event_ID INTEGER NOT NULL,track_name TEXT NOT NULL,FOREIGN KEY (event_ID) REFERENCES event,UNIQUE (event_ID, track_name));";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE venue (venue_ID INTEGER PRIMARY KEY,name TEXT NOT NULL);";
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Your data base has been created.");
            }
            databasePath = $"Data Source = {userInput}.sqlite; Version = 3;";
        }
        public void SelectEvent()
        {
            GetEvents();
            Console.WriteLine("Please select an event from the list of events below, enter new to start creating a new event, or type back to go to the main menu.");
            for (int i = 1; i <= events.Count; i++)
                Console.WriteLine($" {i}) {events[i - 1].eventName}");

            var userInput = Console.ReadLine();
            if (userInput.ToLower() == "new")
                CreateNewEvent();
            else if (userInput == "back")
                return;
            else if (Convert.ToInt32(userInput) < events.Count + 1 && Convert.ToInt32(userInput) > 0)
                 event_id = events[Convert.ToInt32(userInput) - 1].eventID;
        }
        public void CreateNewEvent() {
            event_id = events.Count + 1;
            Console.WriteLine("What would you like to call your event?");
            var name = Console.ReadLine();
            Console.WriteLine("What year will you hold your event?");
            var year = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("What number in the event series is this? Use 1 if its the first time you are holding the event.");
            var number = Convert.ToInt32(Console.ReadLine());
            var venue_id = GetVenueID();
            using (SQLiteConnection myConnection = new SQLiteConnection(databasePath))
            {
                myConnection.Open();
                SQLiteCommand cmd = new SQLiteCommand(myConnection);
                cmd.CommandText = $"INSERT INTO event (event_ID, name, \"year\", \"number\", venue_ID, deadline_abstract, deadline_submissions, deadline_reviews, deadline_answers, deadline_withdrawal, deadline_deletion_reviews, deadline_deletion_answer) VALUES({event_id}, '{name}', {year}, {number}, {venue_id}, NULL, NULL, NULL, NULL, NULL, NULL, NULL);";
                cmd.ExecuteNonQuery();
            }
          /*Console.WriteLine("Would you like to assign deadlines now? (y/n)");
            var makeDeadlines = Console.ReadLine().Trim().ToLower();
            if (makeDeadlines == "y")
                SetDeadlines();
            else
            {
                
            }*/
        }
        public void SetDeadlines()
        {

        }
        public int GetVenueID()
        {
            int counter = 0;
            Console.WriteLine("Where would you like to hold this event? Below is a list past locations, enter the number of that venue or enter new to enter a new venue");
            using (SQLiteConnection myConnection = new SQLiteConnection(databasePath))
            {
                string getT = $"SELECT venue_ID,name FROM venue";
                SQLiteCommand oCmd = new SQLiteCommand(getT, myConnection);
                myConnection.Open();
                using (SQLiteDataReader oreader = oCmd.ExecuteReader())
                {
                    while (oreader.Read())
                    {
                        Console.WriteLine($"{oreader["venue_ID"]} | {oreader["name"]}");
                        counter++;
                    }
                }
            }

            do
            {
                var userInput = Console.ReadLine();
                if (userInput.ToLower() == "new")
                {
                    counter++;
                    CreateNewVenue(counter);
                    return counter;
                }
                else if (Convert.ToInt32(userInput) <= counter && Convert.ToInt32(userInput) > 0)
                {
                    return Convert.ToInt32(userInput);
                }
                Console.WriteLine("Whatever you have entered does not work with the system, please try again.");
            } while (true);
        }
        public void CreateNewVenue(int venueID)
        {
            Console.WriteLine("What would you like to call this venue?");
            var name = Console.ReadLine();
            using (SQLiteConnection myConnection = new SQLiteConnection(databasePath))
            {
                myConnection.Open();
                SQLiteCommand cmd = new SQLiteCommand(myConnection);
                cmd.CommandText = $"INSERT INTO venue (venue_ID, name) VALUES({venueID}, '{name}');";
                cmd.ExecuteNonQuery();
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
            if (event_id == -1)
                return;
            var coninuteRunning = true;
            while (coninuteRunning)
            {
                Console.WriteLine("Please select one of the following actions below or type back to the main menu:");
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
                        Console.WriteLine("How many days is your event?");
                        var numOfDays = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("How many sessions will happen on each day?");
                        var numOfSessionsPerDay = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("How many speakers are going to present during each session?");
                        var numOfPapersPerSession = Convert.ToInt32(Console.ReadLine());
                        scheduleCreator.CreateSchedule(tracks, listOfPapers, numOfDays, numOfSessionsPerDay, numOfPapersPerSession);
                        scheduleCreator.SaveSchedule();
                        break;
                    case "back":
                        coninuteRunning = false;
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
            if (event_id == -1)
                return;
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
