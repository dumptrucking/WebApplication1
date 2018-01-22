using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace WebApplication1.Models
{
    public class RichesModel
    {
        public RichesModel()
        {
            History = new PingHistory();
            NumberOfPings = 250;
            DecisionTypeId = DecisionTypes.PercentClimb3;
            StartingBaseAmount = 10000;
            ToolMode = ModeTypes.HistoricalData;
            TradingPair = "BTC_ETH";
        }

        public PingHistory History { get; set; }
        [Display(Name = "Number of Pings")]
        public int NumberOfPings { get; set; }
        [Display(Name = "Decision Type")]
        public DecisionTypes DecisionTypeId { get; set; }
        public double StartingBaseAmount { get; set; }
        public ModeTypes ToolMode { get; set; }
        [Display(Name ="Trading Pair")]
        public string TradingPair { get; set; }

        public List<string> TradingPairs { get; set; }

        public List<PairInfo> HistoryFromFile { get; set; }

        public void LoadHistoricalData()
        {
            var sr = new StreamReader("C:\\Users\\marty.goon\\source\\repos\\WebApplication1\\WebApplication1\\App_Data\\pings.txt");
            var sb = new StringBuilder();
            sb.Append("{\"Pings\": [");

            while (!sr.EndOfStream)
            {
                var pingLine = sr.ReadLine();
                sb.Append(pingLine);
                sb.Append(",");
            }
            var content = sb.ToString().TrimEnd(',') + "]}";

            var historicalData = JsonConvert.DeserializeObject<HistoricalData>(content);
            HistoryFromFile = historicalData.Pings.SelectMany(x => x.Items).ToList();
            TradingPairs = HistoryFromFile.Select(x => x.TradingPair).Distinct().ToList();
        }
    }
}