using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public enum NodeLocation
    {
        TopLeft = 2,
        TopCenter = 3,
        TopRight = 4,
        Left = 5,
        BottomRight = TopLeft + Constants.DirectionDifference,
        BottomCenter = TopCenter + Constants.DirectionDifference,
        BottomLeft = TopRight + Constants.DirectionDifference,
        Right = Left + Constants.DirectionDifference
    }
}
