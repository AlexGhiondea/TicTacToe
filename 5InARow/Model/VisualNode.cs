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
        public TicTacToeValue Value;
        public bool PartOfWinningMove;

        public VisualNode(int x, int y, TicTacToeValue value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public void Draw(int gridSize, Graphics g)
        {
            Draw(gridSize, g, false);
        }

        internal void Draw(int gridSize, Graphics g, bool last)
        {
            const int boxPad = 2;

            string nodeValue = (Value == TicTacToeValue.o) ? "o" : "x";
            // draw the node's value
            // scale the coordinates.
            int TopX = X * gridSize;
            int TopY = Y * gridSize;
            var stringSize = g.MeasureString($"{nodeValue}", s_gameFont);

            var left = TopX + (gridSize - stringSize.Width) / 2;
            var top = TopY + (gridSize - stringSize.Height) / 2;

            g.DrawString($"{nodeValue}", s_gameFont, GetBrush(), left, top);

            if (last)
            {
                g.DrawRectangle(s_lastMove, new Rectangle(TopX + boxPad, TopY + boxPad, gridSize - 2 * boxPad, gridSize - 2 * boxPad));
            }

#if DEBUG
            g.DrawString($"{X}_{Y}", s_debugFont, Brushes.Black, TopX, TopY);
#endif
        }

#if DEBUG
        private static readonly Font s_debugFont = new Font("Tahoma", 6);
#endif

        private Brush GetBrush()
        {
            if (PartOfWinningMove)
                return Brushes.Orange;
            else
                return Value == TicTacToeValue.x ? Brushes.IndianRed : Brushes.Teal;
        }
        private static readonly Pen s_lastMove = new Pen(Brushes.Black, 1.5f);
        private static readonly Font s_gameFont = new Font("Consolas", 30);
    }


}
