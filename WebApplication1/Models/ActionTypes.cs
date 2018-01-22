using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public enum ActionTypes : byte
    {
        Buy = 1,
        Sell = 2,
        DoNothing = 3
    }
}