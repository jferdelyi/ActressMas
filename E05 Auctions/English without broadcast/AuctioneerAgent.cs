﻿/**************************************************************************
 *                                                                        *
 *  Website:     https://github.com/florinleon/ActressMas                 *
 *  Description: English auction without broadcast using the ActressMas   *
 *               framework                                                *
 *  Copyright:   (c) 2018, Florin Leon                                    *
 *                                                                        *
 *  This program is free software; you can redistribute it and/or modify  *
 *  it under the terms of the GNU General Public License as published by  *
 *  the Free Software Foundation. This program is distributed in the      *
 *  hope that it will be useful, but WITHOUT ANY WARRANTY; without even   *
 *  the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR   *
 *  PURPOSE. See the GNU General Public License for more details.         *
 *                                                                        *
 **************************************************************************/

using ActressMas;
using System;
using System.Collections.Generic;
using System.Timers;

namespace EnglishAuction
{
    public class AuctioneerAgent : Agent
    {
        private List<string> bidders;
        private string highestBidder;
        private int currentPrice;
        private Timer timer;

        public AuctioneerAgent()
        {
            bidders = new List<string>();

            timer = new Timer();
            timer.Elapsed += t_Elapsed;
            timer.Interval = Utils.Delay;
        }

        private void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Send(this.Name, "wake-up");
        }

        public override void Setup()
        {
            currentPrice = Utils.ReservePrice;
            Broadcast(Utils.Str("price", currentPrice));
            timer.Start();
        }

        private void Broadcast(string messageContent)
        {
            foreach (Agent a in this.Environment.Agents)
            {
                if (a.Name != this.Name)
                    Send(a.Name, messageContent);
            }
        }

        public override void Act(Message message)
        {
            try
            {
                Console.WriteLine("\t[{1} -> {0}]: {2}", this.Name, message.Sender, message.Content);

                string action; string parameters;
                Utils.ParseMessage(message.Content, out action, out parameters);

                switch (action)
                {
                    case "bid":
                        HandleBid(message.Sender);
                        break;

                    case "wake-up":
                        HandleWakeUp();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void HandleBid(string sender)
        {
            bidders.Add(sender);
        }

        private void HandleWakeUp()
        {
            if (bidders.Count == 0) // no more bids
            {
                currentPrice -= Utils.Increment;
                if (currentPrice < Utils.ReservePrice)
                {
                    Console.WriteLine("[auctioneer]: Auction finished. No winner.");
                    Broadcast(Utils.Str("winner", "none"));
                }
                else
                {
                    Console.WriteLine("[auctioneer]: Auction finished. Sold to {0} for price {1}.", highestBidder, currentPrice);
                    Broadcast(Utils.Str("winner", highestBidder));
                }
                timer.Stop();
                Stop();
            }
            else if (bidders.Count == 1)
            {
                highestBidder = bidders[0];
                Console.WriteLine("[auctioneer]: Auction finished. Sold to {0} for price {1}", highestBidder, currentPrice);
                Broadcast(Utils.Str("winner", highestBidder));
                timer.Stop();
                Stop();
            }
            else
            {
                highestBidder = bidders[0]; // first or random from the previous round, breaking ties
                currentPrice += Utils.Increment;

                foreach (string a in bidders)
                    Send(a, Utils.Str("price", currentPrice));

                bidders.Clear();
            }
        }
    }
}