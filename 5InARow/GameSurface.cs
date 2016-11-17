﻿using System;
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
            foreach (var node in nodes.Values)
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

        Dictionary<string, VisualNode> nodes = new Dictionary<string, VisualNode>();

        private void AddMove(int x, int y)
        {
            string nodeKey = Constants.GetKey(x, y);
            if (nodes.ContainsKey(nodeKey))
            {
                MessageBox.Show("A move already exists there!!!");
                return;
            }

            // check to see if it is close to any other nodes, on all sides. If not, error.
            if (!HasNeighbours(x, y) && nodes.Count > 0)
            {
                MessageBox.Show("A move needs to be close to an existing node!");
                return;
            }

            var newInternalNode = new InternalNode(CurrentPlayer);

            foreach (NodeLocation value in Enum.GetValues(typeof(NodeLocation)))
            {
                string neighborLocation = Constants.MapDirectionToComputation[value](x, y);
                VisualNode neighbourNode;
                if (nodes.TryGetValue(neighborLocation, out neighbourNode))
                {
                    Debug.WriteLine($"Node {nodeKey} has node {neighborLocation} at {value}");

                    // add the existing node to the new node as neighbour
                    newInternalNode.AddNeighbour(neighbourNode.Node, value);

                    // get the reverse direction
                    NodeLocation reverseDirection = value.GetReverseDirection();
                    neighbourNode.Node.AddNeighbour(newInternalNode, reverseDirection);

                    Debug.WriteLine($"Node {Constants.GetKey(neighbourNode.X, neighbourNode.Y)} has node {nodeKey} at {reverseDirection}.");
                }
            }

            VisualNode newNode = new VisualNode(x, y, newInternalNode);
            s_moves.Push(newNode);
            nodes.Add(nodeKey, newNode);

            Refresh();

            if (HasWon(newInternalNode))
            {
                MessageBox.Show($"Winner is player {CurrentPlayer}!!!");

                // reset the board
                CurrentPlayer = TicTacToeValue.x;
                nodes.Clear();
                s_moves.Clear();
                Refresh();
            }
            else
            {
                ChangePlayer();
            }
        }

        private bool HasWon(InternalNode newInternalNode)
        {
            // check the 4 directions.
            int colCount = 1 + newInternalNode.CountOnDirection(NodeLocation.TopCenter, newInternalNode.Value) + newInternalNode.CountOnDirection(NodeLocation.BottomCenter, newInternalNode.Value);

            int rowCount = 1 + newInternalNode.CountOnDirection(NodeLocation.Left, newInternalNode.Value) + newInternalNode.CountOnDirection(NodeLocation.Right, newInternalNode.Value);

            int bigDiagCount = 1 + newInternalNode.CountOnDirection(NodeLocation.TopLeft, newInternalNode.Value) + newInternalNode.CountOnDirection(NodeLocation.BottomRight, newInternalNode.Value);

            int smallDiagCount = 1 + newInternalNode.CountOnDirection(NodeLocation.TopRight, newInternalNode.Value) + newInternalNode.CountOnDirection(NodeLocation.BottomLeft, newInternalNode.Value);

            return colCount >= NeededForWin || rowCount >= NeededForWin || bigDiagCount >= NeededForWin || smallDiagCount >= NeededForWin;
        }
        private const int NeededForWin = 5;

        private void RemovePreviousMove()
        {
            if (s_moves.Count > 0)
            {
                var node = s_moves.Pop();
                // remove itself from the neighbours
                node.Node.RemoveFromNeighbours();
                nodes.Remove($"{Constants.GetKey(node.X, node.Y)}");
                // reset the current move
                ChangePlayer();
                Refresh();
            }
        }

        private void ChangePlayer()
        {
            CurrentPlayer = (TicTacToeValue)((int)(CurrentPlayer + 1) % 2);
        }

        private bool HasNeighbours(int x, int y)
        {
            foreach (NodeLocation value in Enum.GetValues(typeof(NodeLocation)))
            {
                string neighborLocation = Constants.MapDirectionToComputation[value](x, y);
                if (nodes.ContainsKey(neighborLocation))
                {
                    Debug.WriteLine($"Found neighbour at: {neighborLocation}");
                    return true;
                }
            }
            return false;
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

                TranslateAllMoves(x - beforeX, y - beforeY);

                Refresh();
            }
            else
            {
                // put a visual node

                Debug.WriteLine($"Trying to add new node at offsetX:{x}, offsetY:{y}");

                AddMove(x, y);
            }
            _isDragOperation = false;
        }

        private void TranslateAllMoves(int offsetX, int offsetY)
        {
            Debug.WriteLine($"Translating by offsetX:{offsetX}, offsetY:{offsetY}");

            // need to re-map the locations based on the new offset.
            Dictionary<string, VisualNode> newNodeDict = new Dictionary<string, VisualNode>();
            foreach (var key in nodes.Keys)
            {
                var splitKey = key.Split('_');
                int newX = int.Parse(splitKey[0]) + offsetX;
                int newY = int.Parse(splitKey[1]) + offsetY;

                var newKey = $"{newX}_{newY}";
                Debug.WriteLine($"Remapping {key} to {newKey}");
                newNodeDict.Add(newKey, nodes[key]);
                nodes[key].X += offsetX;
                nodes[key].Y += offsetY;
            }
            nodes = newNodeDict;
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
    }

    public class VisualNode
    {
        public int X;
        public int Y;
        public InternalNode Node;

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
            // scale the coordinates.
            int TopX = X * gridSize;
            int TopY = Y * gridSize;

            // draw the node's value
            var stringSize = g.MeasureString($"{Node.Value}", s_gameFont);

            var left = TopX + (gridSize - stringSize.Width) / 2;
            var top = TopY + (gridSize - stringSize.Height) / 2;

            g.DrawString($"{Node.Value}", s_gameFont, last ? Brushes.Blue : Brushes.Black, left, top);
        }

        private readonly Font s_gameFont = new Font("Consolas", 30);
    }

    public class InternalNode
    {
        Dictionary<NodeLocation, InternalNode> _neighbours = new Dictionary<NodeLocation, InternalNode>();

        public int CountOnDirection(NodeLocation direction, TicTacToeValue value)
        {
            // starting from this direction count how many there are
            InternalNode nextNode;
            if (!_neighbours.TryGetValue(direction, out nextNode)) 
            {
                return 0;
            }

            return 1 + nextNode.CountOnDirection(direction, value);
        }

        public TicTacToeValue Value;
        public InternalNode(TicTacToeValue value)
        {
            Value = value;
            //Neighbours = new List<NeighbourLocation>();
        }

        public void AddNeighbour(InternalNode node, NodeLocation location)
        {
            _neighbours[location] = node;
        }

        internal void RemoveFromNeighbours()
        {
            // go through each of it's neighbours and remove this node from their list.
            foreach (var node in _neighbours.Values)
            {
                node.RemoveNode(this);
            }
        }

        internal void RemoveNode(InternalNode node)
        {
            var nodeAtLocation = _neighbours.FirstOrDefault(x => x.Value == node);
            if (nodeAtLocation.Value != null)
            {
                _neighbours.Remove(nodeAtLocation.Key);
            }
        }
    }

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

    public enum TicTacToeValue
    {
        x = 0,
        o = 1
    }
}
