using System;
using System.Collections.Generic;
using System.Text;
using TableTopRobot.Models;

namespace TableTopRobot.Interfaces
{
    public interface IPreviousMovements
    {
        int X { get; set; }

        int Y { get; set; }

        FacingDirection F { get; set; }

        void SetPreviousValues(int x, int y, FacingDirection f);

    }
}
