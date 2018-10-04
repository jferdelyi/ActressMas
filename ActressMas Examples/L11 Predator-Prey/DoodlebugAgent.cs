﻿using ActressMas;
using System;
using System.Collections.Generic;

namespace PredatorPrey
{
    public class DoodlebugAgent : InsectAgent
    {
        private int _lastEaten;

        public override void Setup()
        {
            _turnsSurvived = 0;
            _lastEaten = 0;

            _worldEnv = (World)this.Environment;

            if (Utils.Verbose)
                Console.WriteLine("DoodlebugAgent {0} started in ({1},{2})", this.Name, Line, Column);
        }

        public override void Act(Message message)
        {
            try
            {
                if (Utils.Verbose)
                    Console.WriteLine("\t[{1} -> {0}]: {2}", this.Name, message.Sender, message.Content);

                DoodlebugAction();
                Send("scheduler", "done");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void DoodlebugAction()
        {
            /*
             • Move. For every time step, the doodlebug will move to an adjacent cell containing an ant and eat the
                ant. If there are no ants in adjoining cells, the doodlebug moves according to the same rules as the
                ant. Note that a doodlebug cannot eat other doodlebugs.
                • Breed. If a doodlebug survives for eight time steps, at the end of the time step it will spawn off a new
                doodlebug in the same manner as the ant.
                • Starve. If a doodlebug has not eaten an ant within three time steps, at the end of the third time step it
                will starve and die. The doodlebug should then be removed from the grid of cells.
             */

            _turnsSurvived++;
            _lastEaten++;

            // eat
            bool success = TryToEat();
            if (success)
                _lastEaten = 0;

            // move
            if (!success)
                TryToMove(); // implemented in base class InsectAgent

            // breed
            if (_turnsSurvived >= 8)
            {
                if (TryToBreed()) // implemented in base class InsectAgent
                    _turnsSurvived = 0;
            }

            // starve
            if (_lastEaten >= 3)
                Die();
        }

        private bool TryToEat()
        {
            List<Direction> allowedDirections = new List<Direction>();
            int newLine, newColumn;

            for (int i = 0; i < 4; i++)
            {
                if (_worldEnv.ValidMovement(this, (Direction)i, CellState.Ant, out newLine, out newColumn))
                    allowedDirections.Add((Direction)i);
            }

            if (allowedDirections.Count == 0)
                return false;

            int r = Utils.RandNoGen.Next(allowedDirections.Count);
            _worldEnv.ValidMovement(this, allowedDirections[r], CellState.Ant, out newLine, out newColumn);

            _worldEnv.Eat(this, newLine, newColumn);

            return true;
        }

        private void Die()
        {
            // removing the doodlebug

            if (Utils.Verbose)
                Console.WriteLine("Removing " + this.Name);

            this.Stop();
            _worldEnv.Die(this);
        }
    }
}