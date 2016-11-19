using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class VisualNode
    {
        public int X;
        public int Y;
        public InternalNode Node;

        public List<string> GetEmptyNeighBours()
        {
            var result = new List<string>();
            foreach (var direction in Node.GetEmptyNodesAroundIt())
            {
                result.Add(Constants.MapDirectionToComputation[direction](X, Y));
            }
            return result;
        }

        public VisualNode(int x, int y, InternalNode node)
        {
            X = x;
            Y = y;
            Node = node;
        }

        public void Draw(int gridSize, Graphics g)
        {
            Draw(gridSize, g, false);
        }

        internal void Draw(int gridSize, Graphics g, bool last)
        {
            string nodeValue = (Node.Value == TicTacToeValue.o) ? "o" : "x";
            // draw the node's value
            // scale the coordinates.
            int TopX = X * gridSize;
            int TopY = Y * gridSize;
            var stringSize = g.MeasureString($"{nodeValue}", s_gameFont);

            var left = TopX + (gridSize - stringSize.Width) / 2;
            var top = TopY + (gridSize - stringSize.Height) / 2;

            g.DrawString($"{nodeValue}", s_gameFont, GetBrush(last), left, top);
#if DEBUG
            g.DrawString($"{X}_{Y}", new Font("Tahoma", 6), Brushes.Black, TopX, TopY);
#endif
        }

        private Brush GetBrush(bool last)
        {
            if (!last)
                return Node.Value == TicTacToeValue.x ? Brushes.DarkRed : Brushes.DarkGreen;
            else
                return Node.Value == TicTacToeValue.x ? Brushes.IndianRed : Brushes.LightGreen;
        }

        private readonly Font s_gameFont = new Font("Consolas", 30);
    }


}
