using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class PingInfo
    {
        public PingInfo()
        {
            CurrentDepth = InvestmentDepthTypes.None;
        }

        // INFO
        public double LowestSell { get; set; }
        public double HighestBuy { get; set; }
        public double Volume { get; set; }
        public DateTime PingDate { get; set; }


        // HOLDINGS
        // Current number of base coins held
        public double BaseHeld { get; set; }
        // Current number of Alt coins held
        public double AltHeld { get; set; }
        // How much of our Base is invested in Alt
        public InvestmentDepthTypes CurrentDepth { get; set; }


        // CHANGES
        // The LowestSell price difference from the previous ping
        public double NumericChangeInLowestSell { get; set; }
        // The HighestBuy price difference from the previous ping
        public double NumericChangeInHighestBuy { get; set; }
        // The LowestSell price difference from the previous ping
        public double PercentChangeInLowestSell { get; set; }
        // The HighestBuy price difference from the previous ping
        public double PercentChangeInHighestBuy { get; set; }
    }
}