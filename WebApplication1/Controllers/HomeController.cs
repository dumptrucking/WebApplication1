using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(RichesModel vm)
        {
            if (vm.ToolMode == ModeTypes.HistoricalData)
            {
                vm.LoadHistoricalData();
                for (int i = 0; i < vm.HistoryFromFile.Where(x => x.TradingPair == vm.TradingPair).Count(); i++)
                {
                    GetPing(vm);
                }
            }
            else if (vm.ToolMode == ModeTypes.RandomData)
            {
                // Run Simulation
                for (int i = 0; i < vm.NumberOfPings; i++)
                {
                    GetPing(vm);
                }
            }
            else
            {

            }

            // Convert back to base for clarity
            var CurrentPing = vm.History.Pings.Last();
            Sell(CurrentPing, 1);

            // Create File for Graph
            CreateGraphFile(vm.History.Pings);

            return View(vm);
        }

        public void CreateGraphFile(List<PingInfo> pingInfos)
        {               
            // File Creation
            var sw = new StreamWriter("C:\\Users\\marty.goon\\source\\repos\\WebApplication1\\WebApplication1\\temp.csv", false);
            sw.WriteLine("date,close");

            pingInfos.ForEach(x =>
            {
                sw.WriteLine(x.PingDate.ToString() + "," + x.LowestSell);
            });

            sw.Close();
        }



        public void GetPing(RichesModel vm)
        {
            // Get next ping
            var pingInfo = GetPingInfo(vm);
            var prevPing = vm.History.Pings.Any() ? vm.History.Pings.Last() : null;
            vm.History.Pings.Add(pingInfo);

            if (prevPing != null)
            {
                pingInfo.NumericChangeInLowestSell = pingInfo.LowestSell - prevPing.LowestSell;
                pingInfo.NumericChangeInHighestBuy = pingInfo.HighestBuy - prevPing.HighestBuy;
                pingInfo.PercentChangeInLowestSell = pingInfo.NumericChangeInLowestSell / prevPing.LowestSell;
                pingInfo.PercentChangeInHighestBuy = pingInfo.NumericChangeInHighestBuy / prevPing.HighestBuy;
                pingInfo.BaseHeld = prevPing.BaseHeld;
                pingInfo.AltHeld = prevPing.AltHeld;
                pingInfo.CurrentDepth = prevPing.CurrentDepth;
            }
            else
            {
                pingInfo.BaseHeld = vm.StartingBaseAmount;
            }

            // Make decisions
            var action = ActionTypes.DoNothing;
            if (vm.History.Pings.Count > 5)  // Let a couple of pings build up before making decisions since algos require previous pings
            {
                switch (vm.DecisionTypeId)
                {
                    case DecisionTypes.All:
                        var actions = new List<ActionTypes>
                        {
                            vm.History.ComparedToAvg(),
                            vm.History.ComparedToFour(),
                            vm.History.SumOfLastThree(),
                            vm.History.ThreeOfFive(),
                            vm.History.TrendOfLastThree()
                        };
                        if (actions.Where(x => x == ActionTypes.Buy).Count() > 2)
                            action = ActionTypes.Buy;
                        else if (actions.Where(x => x == ActionTypes.Sell).Count() > 2)
                            action = ActionTypes.Sell;
                        break;
                    case DecisionTypes.ComparedToAvg:
                        action = vm.History.ComparedToAvg();
                        break;
                    case DecisionTypes.ComparedToFour:
                        action = vm.History.ComparedToFour();
                        break;
                    case DecisionTypes.SumOfLastThree:
                        action = vm.History.SumOfLastThree();
                        break;
                    case DecisionTypes.ThreeOfFive:
                        action = vm.History.ThreeOfFive();
                        break;
                    case DecisionTypes.TrendOfLastThree:
                        action = vm.History.TrendOfLastThree();
                        break;
                    case DecisionTypes.PercentClimb:
                        action = vm.History.PercentClimb();
                        break;
                    case DecisionTypes.PercentClimb2:
                        action = vm.History.PercentClimb2();
                        break;
                    case DecisionTypes.PercentClimb3:
                        action = vm.History.PercentClimb3();
                        break;

                }


                switch (action)
                {
                    case ActionTypes.Buy:
                        switch (pingInfo.CurrentDepth)
                        {
                            case InvestmentDepthTypes.None:
                                vm.History.OriginalBuyAmount = pingInfo.LowestSell;
                                Buy(pingInfo, .25);
                                pingInfo.CurrentDepth = InvestmentDepthTypes.Shallow;
                                break;
                            case InvestmentDepthTypes.Shallow:
                                Buy(pingInfo, .33);
                                pingInfo.CurrentDepth = InvestmentDepthTypes.Medium;
                                break;
                            case InvestmentDepthTypes.Medium:
                                Buy(pingInfo, .50);
                                pingInfo.CurrentDepth = InvestmentDepthTypes.Deep;
                                break;
                            case InvestmentDepthTypes.Deep:
                                Buy(pingInfo, 1);
                                pingInfo.CurrentDepth = InvestmentDepthTypes.AllIn;
                                break;
                            case InvestmentDepthTypes.AllIn:
                                // Already all in, do nothing
                                break;
                            default:
                                break;
                        }
                        break;

                    case ActionTypes.Sell:
                        Sell(pingInfo, 1);
                        pingInfo.CurrentDepth = InvestmentDepthTypes.None;
                        vm.History.OriginalBuyAmount = 0;
                        break;
                }
            }
        }

        private PingInfo GetPingInfo(RichesModel vm)
        {
            if (vm.ToolMode == ModeTypes.RandomData)
            {
                if (!vm.History.Pings.Any())
                    return new PingInfo
                    {
                        LowestSell = 110,
                        HighestBuy = 90,
                        Volume = 100000,
                        PingDate = DateTime.Now,
                    };
                else
                {
                    var lastPing = vm.History.Pings.Last();
                    var ls = vm.History.Pings.Last().LowestSell;
                    var vol = vm.History.Pings.Last().Volume;
                    //var arrls = new List<double> { ls * .95m, ls * .96m, ls * .97m, ls * .98m, ls * .99m, ls, ls * 1.01m, ls * 1.02m, ls * 1.03m, ls * 1.04m, ls * 1.05m, };
                    var arrls = new List<double> { ls * .91, ls * .95, ls * .97, ls * .98, ls * .99, ls, ls * 1.01, ls * 1.02, ls * 1.03, ls * 1.05, ls * 1.09, }; // More Volitile
                    var arrhb = new List<double> { ls - 1, ls - 2, ls - 3, ls - 4, ls - 5 };
                    var arrvol = new List<double> { vol * .95, vol * .96, vol * .97, vol * .98, vol * .99, vol, vol * 1.01, vol * 1.02, vol * 1.03, vol * 1.04, vol * 1.05, };

                    return new PingInfo
                    {
                        LowestSell = arrls.OrderBy(x => Guid.NewGuid()).FirstOrDefault(),
                        HighestBuy = arrhb.OrderBy(x => Guid.NewGuid()).FirstOrDefault(),
                        Volume = arrvol.OrderBy(x => Guid.NewGuid()).FirstOrDefault(),
                        PingDate = lastPing.PingDate.AddMinutes(1),
                    };
                }
            }
            else if (vm.ToolMode == ModeTypes.HistoricalData)
            {
                if (vm.HistoryFromFile.Where(x => x.TradingPair == vm.TradingPair).Count() > vm.History.Pings.Count())
                {
                    var currentPing = vm.HistoryFromFile.Where(x => x.TradingPair == vm.TradingPair).OrderBy(x => x.PingDate).Skip(vm.History.Pings.Count()).FirstOrDefault();
                    var lastPing = vm.History.Pings.Any() ? vm.History.Pings.Last() : null;

                    return new PingInfo()
                    {
                        LowestSell = currentPing.lowestAsk,
                        HighestBuy = currentPing.highestBid,
                        Volume = currentPing.baseVolume,
                        PingDate = currentPing.PingDate,
                    };
                }
                else
                { return new PingInfo(); }
            }
            else
            {
                return new PingInfo();
            }
        }


        // SIMPLE PERCENT BUY/SELL
        private void Buy(PingInfo currentPing, double percent)
        {
            var investmentAmount = currentPing.BaseHeld * percent;
            currentPing.AltHeld += (investmentAmount / (currentPing.LowestSell)) * 0.9985;
            currentPing.BaseHeld = currentPing.BaseHeld * (1 - percent);
        }

        private void Sell(PingInfo currentPing, double percent)
        {
            var devestmentAmount = currentPing.AltHeld * percent;
            currentPing.BaseHeld += (devestmentAmount * currentPing.HighestBuy) * 0.9985;
            currentPing.AltHeld = currentPing.BaseHeld * (1 - percent);
        }

        public ActionResult Visualize()
        {
            return View();
        }








        //public void Buy(PingInfo currentPing)
        //{
        //    if (!currentPing.IsCurrentlyInvested)
        //    {
        //        currentPing.AltHeld = (currentPing.BaseHeld / (currentPing.LowestSell)) * 0.9985;
        //        currentPing.BaseHeld = 0;
        //        currentPing.IsCurrentlyInvested = true;
        //    }
        //}

        //public void Sell(PingInfo currentPing)
        //{
        //    if (currentPing.IsCurrentlyInvested)
        //    {
        //        currentPing.BaseHeld = (currentPing.AltHeld * currentPing.HighestBuy) * 0.9985;
        //        currentPing.AltHeld = 0;
        //        currentPing.IsCurrentlyInvested = false;
        //    }
        //}

        //public void BuyIncrement(PingHistory vm, PingInfo currentPing, double depth)
        //{
        //    // Record starting BaseAmt so increments will be easier to track
        //    if (vm.Depth == 0)
        //        vm.StartingBase = currentPing.BaseHeld;

        //    // If this is truly an new buy depth
        //    if (depth > vm.Depth)
        //    {
        //        var investmentPercent = depth - vm.Depth;
        //        var investmentAmount = vm.StartingBase * investmentPercent;
        //        currentPing.AltHeld = (investmentAmount / (currentPing.LowestSell)) * 0.9985;
        //        currentPing.BaseHeld = vm.StartingBase * (1 - depth);
        //        currentPing.IsCurrentlyInvested = true;

        //        // Record new depth
        //        vm.Depth = depth;
        //    }
        //}

        //public void SellIncrement(PingHistory vm, PingInfo currentPing, double depth)
        //{
        //    // If this is truly an new sell depth
        //    if (vm.Depth > depth)
        //    {
        //        var devestmentPercent = vm.Depth - depth;
        //        var devestmentAmount = vm.StartingBase * devestmentPercent;
        //        currentPing.BaseHeld = (devestmentAmount * currentPing.HighestBuy) * 0.9985;
        //        currentPing.AltHeld = vm.StartingBase * (1 - depth);
        //        currentPing.IsCurrentlyInvested = true;

        //        // Only Handling total sells until I fix the devestment percent problem

        //        // Record new depth
        //        vm.Depth = depth;
        //    }

        //    // Reset Starting base when cashing all the way out
        //    if (vm.Depth == 0)
        //        vm.StartingBase = 0;
        //}




        //public ActionTypes ComparedToAvg()
        //{
        //    if (One.ChangeFromAvg >= 0)
        //        return ActionTypes.Buy;
        //    else
        //        return ActionTypes.Sell;
        //}

        //public ActionTypes ComparedToAvg()
        //{
        //    if ((One.LowestSell - (Two.LowestSell + Three.LowestSell + Four.LowestSell) / 3) > 2
        //        && (Two.LowestSell - (Three.LowestSell + Four.LowestSell + Five.LowestSell) / 3) > 1)
        //        return ActionTypes.Buy;
        //    else if (One.LowestSell < Two.LowestSell && Two.LowestSell < Three.LowestSell)
        //        return ActionTypes.Sell;

        //    return ActionTypes.DoNothing;
        //}

        //////public ActionTypes ComparedToAvg()
        //////{
        //////    if (((One.LowestSell - Three.LowestSell) / Three.LowestSell) > .05m)
        //////        return ActionTypes.Buy;
        //////    else if (One.LowestSell < Two.LowestSell)
        //////        return ActionTypes.Sell;

        //////    return ActionTypes.DoNothing;
        //////}

        //public ActionTypes ComparedToAvg()  // Simple Idea
        //{
        //    if (One.LowestSell > Two.LowestSell)
        //        return ActionTypes.Buy;
        //    else if (One.LowestSell < Two.LowestSell)
        //        return ActionTypes.Sell;

        //    return ActionTypes.DoNothing;
        //}

        //public ActionTypes PercentClimb3()
        //{
        //    if ((One.LowestSell - Three.LowestSell) / Three.LowestSell > .05m)
        //        return ActionTypes.Buy;
        //    if ((One.LowestSell - Three.LowestSell) / Three.LowestSell < .015m)
        //        return ActionTypes.Sell;

        //    return ActionTypes.DoNothing;
        //}
        //public ActionTypes PercentClimb3()
        //{

        //     if ((One.Change < 0 && Two.Change < 0) || (One.Change + Two.Change + Three.Change) / 3 < .04m)
        //        return ActionTypes.Sell;
        //    else if ((Four.LowestSell + Three.LowestSell + Two.LowestSell) / 3 < One.LowestSell)
        //        return ActionTypes.Buy;

        //    return ActionTypes.DoNothing;
        //}

        //public ActionTypes PercentClimb3()
        //{

        //    if ((One.ChangeInLowestSell < 0 && Two.ChangeInLowestSell < 0) || (One.ChangeInLowestSell + Two.ChangeInLowestSell) / Four.LowestSell < .03m)
        //        return ActionTypes.Sell;
        //    else if (Three.ChangeInLowestSell > 0 && Two.ChangeInLowestSell > 0 && One.ChangeInLowestSell > 0)
        //        return ActionTypes.Buy;

        //    return ActionTypes.DoNothing;
        //}
    }
}