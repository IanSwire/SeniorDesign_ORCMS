using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorDesign
{
    class Session
    {
        public List<Paper> speakers;
        public Session()
        {
            speakers = new List<Paper>();
        }
        public void AddPaper(Paper paper)
        {
            speakers.Add(paper);
        }
        public void SetPapers(List<Paper> papers)
        {
            speakers.Clear();
            foreach (var paper in papers)
                speakers.Add(paper);
        }
    }
}
