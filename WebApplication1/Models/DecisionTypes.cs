using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public enum DecisionTypes : byte
    {
        All = 1,
        SumOfLastThree = 2,
        TrendOfLastThree = 3,
        ComparedToFour = 4,
        ThreeOfFive = 5,
        ComparedToAvg = 6,
        PercentClimb = 7,
        PercentClimb2 = 8,
        PercentClimb3 = 9,
    }
}