﻿/**************************************************************************
 *                                                                        *
 *  Description: Simple example of using the ActressMas framework         *
 *  Website:     https://github.com/florinleon/ActressMas                 *
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
using System.Threading;

namespace Agents2
{
    public class Program
    {
        private static void Main(string[] args)
        {
            // === Init ===

            var env = new ConcurrentEnvironment();
            var a1 = new MyAgent1(); env.Add(a1, "agent1");
            var a2 = new MyAgent2(); env.Add(a2, "agent2");

            // === Run ===

            a1.Start(); a2.Start();

            // === Wait ===

            env.WaitAll();
        }
    }

    public class MyAgent1 : ConcurrentAgent
    {
        public override void Setup()
        {
            Send("agent2", "start from 1");

            for (int i = 0; i < 10; i++)
            {
                Send("agent2", "in setup for: " + i.ToString());
                Thread.Sleep(100);
            }
        }

        public override void Act(Message message)
        {
            Console.WriteLine("[{0}]    {1} said: {2}", this.Name, message.Sender, message.Content);

            for (int i = 0; i < 10; i++)
            {
                Send("agent2", "in act for: " + i.ToString());
                Thread.Sleep(100);
            }
        }
    }

    public class MyAgent2 : ConcurrentAgent
    {
        public override void Setup()
        {
            Send("agent1", "start from 2");
        }

        public override void Act(Message message)
        {
            Console.WriteLine("[{0}]    {1} said: {2}", this.Name, message.Sender, message.Content);
        }
    }
}