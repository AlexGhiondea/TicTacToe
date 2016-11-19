using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class TicTacToe : Form
    {
        private static Pen s_gridPen = new Pen(new SolidBrush(Color.LightGray), 1);
        private static Font s_font = new Font("Tahoma", 6, FontStyle.Regular);
        private int gridSize = 30;
        private TicTacToeValue s_currentPlayer;

        TicTacToeGame _game = new TicTacToeGame(NeededForWin);

        private static Stack<VisualNode> s_moves = new Stack<VisualNode>();
        public TicTacToeValue CurrentPlayer
        {
            get { return s_currentPlayer; }
            set { s_currentPlayer = value; lblNext.Text = value.ToString(); }
        }

        public TicTacToe()
        {
            InitializeComponent();

            // Enable double buffering for the drawing surface
            typeof(Panel).InvokeMember("DoubleBuffered",
               BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
               null, GameSurface, new object[] { true });
        }

        private void DrawGrid(Size rectangle, Graphics g)
        {
            int w = rectangle.Width;
            int h = rectangle.Height;

            for (int i = 0; i < h; i += gridSize)
            {
                g.DrawLine(s_gridPen, new Point(0, i), new Point(w, i));
                g.DrawString(((i / gridSize) + 1).ToString(), s_font, Brushes.Gray, new Point(0, i));
            }

            for (int i = 0; i < w; i += gridSize)
            {
                g.DrawLine(s_gridPen, new Point(i, 0), new Point(i, h));
                if (i > 0) // only draw the top corner once.
                    g.DrawString(((i / gridSize) + 1).ToString(), s_font, Brushes.Gray, new Point(i, 0));
            }
        }

        private int SnapToClosest(int actual)
        {
            int whole = actual / gridSize;
            int extra = (int)Math.Floor((double)(actual % gridSize) / gridSize);

            return (whole + extra);
        }

        private void GameSurface_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            DrawGrid(Size, e.Graphics);

            // draw all the visual nodes
            foreach (var node in _game.Nodes)
            {
                node.Draw(gridSize, e.Graphics);
            }

            // draw the last one different
            if (s_moves.Count > 0)
            {
                s_moves.Peek().Draw(gridSize, e.Graphics, last: true);
            }
        }

        private void GameSurface_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDownLocation = e.Location;
            _isDragOperation = false;
        }
        private Point _mouseDownLocation = default(Point);
        private const int NeededForWin = 5;

        private void RemovePreviousMove()
        {
            if (s_moves.Count > 0)
            {
                var node = s_moves.Pop();

                _game.RemoveNode(node);

                // reset the current move
                ChangePlayer();
                Refresh();
            }
        }

        private void ChangePlayer()
        {
            CurrentPlayer = CurrentPlayer == TicTacToeValue.o ? TicTacToeValue.x : TicTacToeValue.o;
        }

        private bool PlaceMove(VisualNode move)
        {
            if (move == null)
            {
                MessageBox.Show("Cannot place the move there!");
                return false;
            }

            s_moves.Push(move);

            Refresh();

            NodeLocation winDirection;
            if (_game.IsWinningMove(move.Node, out winDirection))
            {
                // Mark the wining nodes.
                _game.MarkWinningNodes(move, winDirection);
                Refresh();
                MessageBox.Show($"Winner is player {CurrentPlayer}!!!");
                return true;
            }
            else
            {
                ChangePlayer();
            }

            Application.DoEvents();
            return false;
        }

        private void PlaceAIMove()
        {
            var move = _game.GetAIMove(CurrentPlayer);
            PlaceMove(move);
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            RemovePreviousMove();
        }

        private void GameSurface_MouseUp(object sender, MouseEventArgs e)
        {
            int x = SnapToClosest(e.X);
            int y = SnapToClosest(e.Y);
            if (_isDragOperation)
            {
                // calculate the new offset based on the drag
                // the offset is calculated in gridSize increments.
                int beforeX = SnapToClosest(_mouseDownLocation.X);
                int beforeY = SnapToClosest(_mouseDownLocation.Y);

                _game.TranslateAllMoves(x - beforeX, y - beforeY);

                Refresh();
            }
            else
            {
                // put a visual node

                Debug.WriteLine($"Trying to add new node at offsetX:{x}, offsetY:{y}");

                var move = _game.AddMove(x, y, CurrentPlayer);

                if (!PlaceMove(move))
                {
                    // if we are playing against AI... suggest a move.
                    if (rbAI.Checked)
                    {
                        PlaceAIMove();
                    }
                }
            }
            _isDragOperation = false;
        }

        private static void GetCoordinates(string key, out int newX, out int newY)
        {
            var splitKey = key.Split('_');
            newX = int.Parse(splitKey[0]) - 1;
            newY = int.Parse(splitKey[1]) - 1;
        }

        private bool _isDragOperation = false;
        private void GameSurface_MouseMove(object sender, MouseEventArgs e)
        {
            // check if we moved more than 1 grid size
            if (_mouseDownLocation != default(Point))
            {
                if (Math.Abs(e.Location.X - _mouseDownLocation.X) > (gridSize / 2) ||
                    Math.Abs(e.Location.Y - _mouseDownLocation.Y) > (gridSize / 2))
                {
                    _isDragOperation = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PlaceAIMove();
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _game.ResetGame();
            s_moves.Clear();
            CurrentPlayer = TicTacToeValue.x;
            Refresh();
        }
    }
}
