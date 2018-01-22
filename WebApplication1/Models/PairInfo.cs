using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class PairInfo
    {
        public string TradingPair { get; set; }
        public int id { get; set; }
        public double last { get; set; }
        public double lowestAsk { get; set; }
        public double highestBid { get; set; }
        public double percentChange { get; set; }
        public double baseVolume { get; set; }
        public double quoteVolume { get; set; }
        public string isFrozen { get; set; }
        public double high24hr { get; set; }
        public double low24hr { get; set; }

        public DateTime PingDate { get; set; }
    }
}