using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SeniorDesign
{
    public class ComparePapers : IComparer<Paper>
    {
        int IComparer<Paper>.Compare(Paper x, Paper y)
        {
            return x.Rank.CompareTo(y.Rank);
        }
    }
}
