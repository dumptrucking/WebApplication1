using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class PingHistory
    {
        public PingHistory()
        {
            Pings = new List<PingInfo>();
        }

        // The collection of Pings
        public List<PingInfo> Pings { get; set; }

        // Base held at the begining of the buying cycle
        public double StartingBase { get; set; }

        // How much of the total base is invested
        public double Depth { get; set; }

        public double OriginalBuyAmount { get; set; }


        // DECISION INFO
        // Get last five pings
        public PingInfo One { get { return Pings.Last(); } }
        public PingInfo Two
        {
            get
            { return Pings.Count > 1 ? Pings[Pings.IndexOf(One) - 1] : new PingInfo(); }
        }
        public PingInfo Three
        {
            get
            { return Pings.Count > 2 ? Pings[Pings.IndexOf(Two) - 1] : new PingInfo(); }
        }
        public PingInfo Four
        {
            get
            { return Pings.Count > 3 ? Pings[Pings.IndexOf(Three) - 1] : new PingInfo(); }
        }
        public PingInfo Five
        {
            get
            { return Pings.Count > 4 ? Pings[Pings.IndexOf(Four) - 1] : new PingInfo(); }
        }
        public string LastChanges(int repsBeforeChange)
        {
            var retval = "";
            for (int i = 0; i < repsBeforeChange; i++)
            {
                retval += Pings.Count > i && Pings[Pings.Count - (i + 1)].NumericChangeInLowestSell < 0 ? "N" : "P";
            }

            return retval;
        }

        // ALGOS
        public ActionTypes SumOfLastThree()
        {
            if (Three.NumericChangeInLowestSell + Two.NumericChangeInLowestSell + One.NumericChangeInLowestSell > 0)
                return ActionTypes.Buy;
            if (Three.NumericChangeInLowestSell + Two.NumericChangeInLowestSell + One.NumericChangeInLowestSell < 0)
                return ActionTypes.Sell;

            return ActionTypes.DoNothing;
        }

        public ActionTypes TrendOfLastThree()
        {
            if (One.NumericChangeInLowestSell >= 0 && Two.NumericChangeInLowestSell >= 0 && Three.NumericChangeInLowestSell >= 0)
                return ActionTypes.Buy;
            if (One.NumericChangeInLowestSell < 0 && Two.NumericChangeInLowestSell < 0 && Three.NumericChangeInLowestSell < 0)
                return ActionTypes.Sell;

            return ActionTypes.DoNothing;
        }

        public ActionTypes ComparedToFour()
        {
            if (One.LowestSell >= Four.LowestSell && Two.LowestSell >= Four.LowestSell && Three.LowestSell >= Four.LowestSell)
                return ActionTypes.Buy;
            if (One.LowestSell < Four.LowestSell && Two.LowestSell < Four.LowestSell && Three.LowestSell < Four.LowestSell)
                return ActionTypes.Sell;

            return ActionTypes.DoNothing;
        }

        public ActionTypes ThreeOfFive()
        {
            var positives = 0;
            if (One.NumericChangeInLowestSell >= 0)
                positives++;
            if (Two.NumericChangeInLowestSell >= 0)
                positives++;
            if (Three.NumericChangeInLowestSell >= 0)
                positives++;
            if (Four.NumericChangeInLowestSell >= 0)
                positives++;
            if (Five.NumericChangeInLowestSell >= 0)
                positives++;
            if (positives > 2)
                return ActionTypes.Buy;
            else
                return ActionTypes.Sell;
        }

        public ActionTypes ComparedToAvg()
        {
            if (One.LowestSell > Two.LowestSell && ((One.LowestSell - Two.LowestSell) / Two.LowestSell > .02))
                return ActionTypes.Buy;
            else if (One.LowestSell < Two.LowestSell)
                return ActionTypes.Sell;

            return ActionTypes.DoNothing;
        }

        public ActionTypes PercentClimb()
        {
            if ((One.LowestSell - Two.LowestSell) / Two.LowestSell >= .02
                && (Two.LowestSell - Three.LowestSell) / Three.LowestSell >= .02
                && (Three.LowestSell - Four.LowestSell) / Four.LowestSell >= .02)
                return ActionTypes.Buy;
            if ((One.LowestSell - Two.LowestSell) / Two.LowestSell < -.02
                && (Two.LowestSell - Three.LowestSell) / Three.LowestSell < -.02
                && (Three.LowestSell - Four.LowestSell) / Four.LowestSell < -.02)
                return ActionTypes.Sell;

            return ActionTypes.DoNothing;
        }

        public ActionTypes PercentClimb2()
        {
            if ((One.LowestSell - Two.LowestSell) / Two.LowestSell
                + (Two.LowestSell - Three.LowestSell) / Three.LowestSell
                + (Three.LowestSell - Four.LowestSell) / Four.LowestSell >= .04)
                return ActionTypes.Buy;
            if ((One.LowestSell - Two.LowestSell) / Two.LowestSell
                + (Two.LowestSell - Three.LowestSell) / Three.LowestSell
                + (Three.LowestSell - Four.LowestSell) / Four.LowestSell < -.04)
                return ActionTypes.Sell;

            return ActionTypes.DoNothing;
        }



        public ActionTypes PercentClimb3()
        {


            if ((Two.NumericChangeInLowestSell > 0 && One.NumericChangeInLowestSell > 0 && Three.NumericChangeInLowestSell > 0))
                //&& (Three.PercentChangeInLowestSell + Two.PercentChangeInLowestSell + One.PercentChangeInLowestSell > .02))
                return ActionTypes.Buy;
            else if ((One.NumericChangeInLowestSell < 0 && Two.NumericChangeInLowestSell < 0) || One.LowestSell < (OriginalBuyAmount))
                return ActionTypes.Sell;

            return ActionTypes.DoNothing;
        }

    }
}