using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public enum ModeTypes : byte
    {
        RandomData = 1,
        HistoricalData = 2,
        LiveData = 3,
    }
}