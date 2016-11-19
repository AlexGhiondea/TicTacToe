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

            //if (nodes.Count > 0 && !HasNeighbours(x, y))
            //{
            //    return null;
            //}

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

        private int Max(params int[] values)
        {
            int absoluteMax = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (Math.Abs(values[i]) > Math.Abs(absoluteMax))
                {
                    absoluteMax = values[i];
                }
            }
            return absoluteMax;
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

                positionValues = Max(CountOnDirection(NodeLocation.TopCenter, currentX, currentY, currentPlayer),
                                 CountOnDirection(NodeLocation.Left, currentX, currentY, currentPlayer),
                                 CountOnDirection(NodeLocation.TopLeft, currentX, currentY, currentPlayer),
                                 CountOnDirection(NodeLocation.TopRight, currentX, currentY, currentPlayer));

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

        private int CountOnDirection(NodeLocation direction, int currentX, int currentY, TicTacToeValue currentPlayer)
        {
            int countX = CountOnSingleDirection(direction, currentX, currentY, TicTacToeValue.x) +
                CountOnSingleDirection(Constants.GetReverseDirection(direction), currentX, currentY, TicTacToeValue.x);

            if (countX >= 4)
                countX *= countX;
            else if (countX >= 3)
                countX *= 2;

            int countO = CountOnSingleDirection(direction, currentX, currentY, TicTacToeValue.o) +
                CountOnSingleDirection(Constants.GetReverseDirection(direction), currentX, currentY, TicTacToeValue.o);

            if (countO >= 4)
                countO *= countO;
            else if (countO >= 3)
                countO *= 2;

            return currentPlayer == TicTacToeValue.x ? countX - countO : countO - countX;
        }

        private int CountOnSingleDirection(NodeLocation direction, int currentX, int currentY, TicTacToeValue value)
        {
            string nodeKey = Constants.MapDirectionToComputation[direction](currentX, currentY);
            if (nodes.ContainsKey(nodeKey))
            {
                int count = 0;
                nodes[nodeKey].Node.NavigateADirection(direction, value, ref count);
                return count;
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
