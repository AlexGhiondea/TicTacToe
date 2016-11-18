using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class TicTacToeGame
    {
        public TicTacToeGame(int neededForWin)
        {
            NeededForWin = neededForWin;
        }

        public IEnumerable<VisualNode> Nodes { get { return nodes.Values; } }

        private readonly int NeededForWin;
        Dictionary<string, VisualNode> nodes = new Dictionary<string, VisualNode>();

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

        public VisualNode AddMove(int x, int y, TicTacToeValue currentPlayer)
        {
            string nodeKey = Constants.GetKey(x, y);
            if (nodes.ContainsKey(nodeKey))
            {
                return null;
            }

            if (nodes.Count > 0 && !HasNeighbours(x, y))
            {
                return null;
            }

            var newInternalNode = new InternalNode(currentPlayer);

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
            nodes.Add(nodeKey, newNode);

            return newNode;
        }

        public bool IsWinningMove(InternalNode currentMove)
        {
            // check the 4 directions.
            int colCount = 1 + currentMove.CountOnDirection(NodeLocation.TopCenter, currentMove.Value) + currentMove.CountOnDirection(NodeLocation.BottomCenter, currentMove.Value);

            if (colCount >= NeededForWin)
                return true;

            int rowCount = 1 + currentMove.CountOnDirection(NodeLocation.Left, currentMove.Value) + currentMove.CountOnDirection(NodeLocation.Right, currentMove.Value);

            if (rowCount >= NeededForWin)
                return true;

            int bigDiagCount = 1 + currentMove.CountOnDirection(NodeLocation.TopLeft, currentMove.Value) + currentMove.CountOnDirection(NodeLocation.BottomRight, currentMove.Value);
            if (bigDiagCount >= NeededForWin)
                return true;

            int smallDiagCount = 1 + currentMove.CountOnDirection(NodeLocation.TopRight, currentMove.Value) + currentMove.CountOnDirection(NodeLocation.BottomLeft, currentMove.Value);
            if (smallDiagCount >= NeededForWin)
                return true;

            return false;
        }

        public VisualNode GetAIMove(TicTacToeValue currentPlayer)
        {
            string myMove = string.Empty;
            // figure out all the empty nodes outthere.
            HashSet<string> positions = new HashSet<string>();
            foreach (var node in nodes.Values)
            {
                foreach (var neighbour in node.GetEmptyNeighBours())
                {
                    positions.Add(neighbour);
                }
            }

            // we are going to count (-1, 1) based on the counts for each position.
            // we don't need to actually place the move, just compute the neighbours we need to check for.
            Dictionary<string, int> mapPositionContributions = new Dictionary<string, int>();
            int bestX = 0, bestO = 0;
            foreach (var position in positions)
            {
                int currentX, currentY;
                Constants.GetCoordinates(position, out currentX, out currentY);

                // find all the nei

                int positionValues = 0; // consider it neutral for now.

                positionValues = CountOnDirection(NodeLocation.TopCenter, currentX, currentY) +
                                 CountOnDirection(NodeLocation.Left, currentX, currentY) +
                                 CountOnDirection(NodeLocation.TopLeft, currentX, currentY) +
                                 CountOnDirection(NodeLocation.TopRight, currentX, currentY);

                // keep a running total of best X and best O positions
                if (positionValues >= 0 && positionValues > bestX)
                    bestX = positionValues;

                if (positionValues <= 0 && positionValues < bestO)
                    bestO = positionValues;

                Debug.WriteLine($"Computed value {position}={positionValues}");
                mapPositionContributions[position] = positionValues;
            }

            Debug.WriteLine($"Best X postion has value {bestX}, Best O position has value {bestO}");

            // we now need to figure out the win move.
            int bestMoveCount;
            if (currentPlayer == TicTacToeValue.o)
            {
                //if we have a tie, prefer to win the game!
                if (Math.Abs(bestO) == Math.Abs(bestX))
                {
                    bestMoveCount = bestO;
                }
                else
                {
                    bestMoveCount = Math.Abs(bestO) > Math.Abs(bestX) ? bestO : bestX;
                }
            }
            else
            {
                //if we have a tie, prefer to win the game!
                if (Math.Abs(bestO) == Math.Abs(bestX))
                {
                    bestMoveCount = bestX;
                }
                bestMoveCount = Math.Abs(bestX) > Math.Abs(bestO) ? bestX : bestO;
            }

            Random r = new Random((int)DateTime.Now.Ticks);
            var possibleMoves = mapPositionContributions.Where(pair => pair.Value == bestMoveCount);
            myMove = possibleMoves.Skip(r.Next(possibleMoves.Count() - 1)).First().Key;
            Debug.WriteLine($"The best move for {currentPlayer} is {myMove}");

            int x = 0, y = 0;
            Constants.GetCoordinates(myMove, out x, out y);
            return AddMove(x, y, currentPlayer);
        }

        private int CountOnDirection(NodeLocation direction, int currentX, int currentY)
        {
            //TODO: need to update this to not use recursion.
            // Instead, it needs to generate the node positions by hand and count that way.
            // otherwise, gaps in the graph are missed.
            // we might not even need a graph anymore!!!

            // from the current direction, make sure that you keep getting the next node on that direction

            //do
            //{
            //    var node = nodes[Constants.MapDirectionToComputation[direction](currentX, currentY)];
            //    currentX = node.X;
            //    currentY = node.Y;
            //} while (true);


            int count = CountOnSingleDirection(direction, currentX, currentY) +
                CountOnSingleDirection(Constants.GetReverseDirection(direction), currentX, currentY);

            // block once the opponent has 3
            if (Math.Abs(count) >= (NeededForWin / 2) + 1)
                count *= 20 * count; // large number to  sure it get picked

            return count;
        }

        private int CountOnSingleDirection(NodeLocation direction, int currentX, int currentY)
        {
            //try the first direciton
            string nodeKey = Constants.MapDirectionToComputation[direction](currentX, currentY);
            if (nodes.ContainsKey(nodeKey))
            {
                return nodes[nodeKey].Node.AddAllOnDirection(direction, null);
            }
            return 0;
        }

        internal void RemoveNode(VisualNode node)
        {
            // remove itself from the neighbours
            node.Node.RemoveFromNeighbours();

            nodes.Remove($"{Constants.GetKey(node.X, node.Y)}");
        }

        public void TranslateAllMoves(int offsetX, int offsetY)
        {
            Debug.WriteLine($"Translating by offsetX:{offsetX}, offsetY:{offsetY}");

            // need to re-map the locations based on the new offset.
            Dictionary<string, VisualNode> newNodeDict = new Dictionary<string, VisualNode>();
            foreach (var key in nodes.Keys)
            {
                int newX, newY;
                Constants.GetCoordinates(key, out newX, out newY);
                newX += offsetX + 1;
                newY += offsetY + 1;

                var newKey = $"{newX}_{newY}";
                Debug.WriteLine($"Remapping {key} to {newKey}");
                newNodeDict.Add(newKey, nodes[key]);
                nodes[key].X += offsetX;
                nodes[key].Y += offsetY;
            }
            nodes = newNodeDict;
        }



    }
}
