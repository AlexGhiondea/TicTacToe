using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public static class Constants
    {
        public static string GetKey(int x, int y)
        {
            return $"{x + 1}_{y + 1}"; //adding 1 to offset the dictionary key by 1 to match the visual cues.
        }

        public const int DirectionDifference = 100;

        public static readonly Dictionary<NodeLocation, Func<int, int, string>> MapDirectionToComputation = new Dictionary<NodeLocation, Func<int, int, string>>()
        {
            { NodeLocation.TopLeft,      (x,y)=>$"{GetKey(x - 1, y - 1)}" },
            { NodeLocation.BottomLeft,   (x,y)=>$"{GetKey(x - 1, y + 1)}" },
            { NodeLocation.Left,         (x,y)=>$"{GetKey(x - 1, y    )}" },
            { NodeLocation.TopRight,     (x,y)=>$"{GetKey(x + 1, y - 1)}" },
            { NodeLocation.BottomRight,  (x,y)=>$"{GetKey(x + 1, y + 1)}" },
            { NodeLocation.Right,        (x,y)=>$"{GetKey(x + 1, y    )}" },
            { NodeLocation.BottomCenter, (x,y)=>$"{GetKey(x    , y + 1)}" },
            { NodeLocation.TopCenter,    (x,y)=>$"{GetKey(x    , y - 1)}" }
        };

        public static NodeLocation GetReverseDirection(this NodeLocation nodeLocation)
        {
            return (nodeLocation + ((int)nodeLocation > DirectionDifference ? -1 : 1) * DirectionDifference);
        }
    }
}
