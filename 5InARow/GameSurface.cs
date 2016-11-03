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

namespace _5InARow
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
            DrawGrid(Size, e.Graphics);

            // draw all the visual nodes
            foreach (var node in nodes.Values)
            {
                node.Draw(gridSize, e.Graphics);
            }
        }

        private void GameSurface_MouseDown(object sender, MouseEventArgs e)
        {
            // put a visual node

            int x = SnapToClosest(e.X);
            int y = SnapToClosest(e.Y);

            AddMove(x, y);
        }

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

        private void TicTacToe_Load(object sender, EventArgs e)
        {
            //lblNext.DataBindings.Add("Text", this, nameof(CurrentPlayer), false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            RemovePreviousMove();
        }
    }

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
            // scale the coordinates.
            int TopX = X * gridSize;
            int TopY = Y * gridSize;

            // draw the node's value
            var stringSize = g.MeasureString($"{Node.Value}", s_gameFont);

            var left = TopX + (gridSize - stringSize.Width) / 2;
            var top = TopY + (gridSize - stringSize.Height) / 2;

            g.DrawString($"{Node.Value}", s_gameFont, Brushes.Black, left, top);
        }

        private readonly Font s_gameFont = new Font("Consolas", 30);
    }

    public class InternalNode
    {
        public int CountOnDirection(NodeLocation direction, TicTacToeValue value)
        {
            // starting from this direction count how many there are
            var nextNode = Neighbours.FirstOrDefault(x => x.Location == direction && x.Node.Value == value);

            if (nextNode == null)
            {
                return 0;
            }
            else
            {
                return 1 + nextNode.Node.CountOnDirection(direction, value);
            }
        }

        public TicTacToeValue Value;
        private List<NeighbourLocation> Neighbours;
        public InternalNode(TicTacToeValue value)
        {
            Value = value;
            Neighbours = new List<NeighbourLocation>();
        }

        public void AddNeighbour(InternalNode node, NodeLocation location)
        {
            Neighbours.Add(new NeighbourLocation() { Location = location, Node = node });
        }

        internal void RemoveFromNeighbours()
        {
            // go through each of it's neighbours and remove this node from their list.
            foreach (var node in Neighbours)
            {
                node.Node.RemoveNode(this);
            }
        }

        internal void RemoveNode(InternalNode node)
        {
            var NodeAtLocation = Neighbours.FirstOrDefault(x => x.Node == node);
            if (NodeAtLocation != null)
            {
                Neighbours.Remove(NodeAtLocation);
            }
        }

        private class NeighbourLocation
        {
            public NodeLocation Location;
            public InternalNode Node;
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
