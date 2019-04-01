using System;
using System.Collections.Generic;
using System.Text;
using TableTopRobot.Interfaces;

namespace TableTopRobot.Models
{
    public class PreviousMovements : IPreviousMovements
    {
        public int X { get; set; }
        public int Y { get; set; }
        public FacingDirection F { get; set; }

        public void SetPreviousValues(int x, int y, FacingDirection f)
        {
            X = x;
            Y = y;
            F = f;

        }
        
        
    }

    public enum FacingDirection
    {
        North,
        South,
        West,
        East
    }
}
