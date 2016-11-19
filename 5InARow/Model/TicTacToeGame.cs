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

        public void ResetGame()
        {
            nodes.Clear();
        }

        public VisualNode AddMove(int x, int y, TicTacToeValue currentPlayer)
        {
            string nodeKey = Constants.GetKey(x, y);
            if (nodes.ContainsKey(nodeKey))
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

        public bool IsWinningMove(InternalNode currentMove, out NodeLocation winDirection)
        {
            // check the 4 directions.
            int colCount = 1 + currentMove.CountOnDirection(NodeLocation.TopCenter, currentMove.Value) + currentMove.CountOnDirection(NodeLocation.BottomCenter, currentMove.Value);
            winDirection = NodeLocation.TopCenter;
            if (colCount >= NeededForWin)
                return true;

            int rowCount = 1 + currentMove.CountOnDirection(NodeLocation.Left, currentMove.Value) + currentMove.CountOnDirection(NodeLocation.Right, currentMove.Value);
            winDirection = NodeLocation.Left;
            if (rowCount >= NeededForWin)
                return true;

            int bigDiagCount = 1 + currentMove.CountOnDirection(NodeLocation.TopLeft, currentMove.Value) + currentMove.CountOnDirection(NodeLocation.BottomRight, currentMove.Value);
            winDirection = NodeLocation.TopLeft;
            if (bigDiagCount >= NeededForWin)
                return true;

            int smallDiagCount = 1 + currentMove.CountOnDirection(NodeLocation.TopRight, currentMove.Value) + currentMove.CountOnDirection(NodeLocation.BottomLeft, currentMove.Value);
            winDirection = NodeLocation.TopRight;
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

        private void TraverseBoard(VisualNode node, NodeLocation direction, Action<VisualNode> runAction)
        {
            // flag all the nodes on the winDirection
            var currentNode = nodes[Constants.GetKey(node.X, node.Y)];
            TicTacToeValue nodeValue = node.Node.Value;

            // traverse all nodes of the same value
            while (currentNode.Node.Value == nodeValue)
            {
                runAction(currentNode);
                string nodeKey = Constants.MapDirectionToComputation[direction](currentNode.X, currentNode.Y);

                // continue on the direction, if there is anything there.
                if (nodes.ContainsKey(nodeKey))
                {
                    currentNode = nodes[nodeKey];
                }
                else
                {
                    break;
                }
            }
        }

        internal void MarkWinningNodes(VisualNode node, NodeLocation winDirection)
        {
            // flag all the nodes on that direction so that when they are drawn, you can tell who won.
            TraverseBoard(node, winDirection, (n) => n.PartOfWinningMove = true);
            TraverseBoard(node, winDirection.GetReverseDirection(), (n) => n.PartOfWinningMove = true);
        }

        private int CountOnDirectionCore(NodeLocation direction, int currentX, int currentY, TicTacToeValue currentPlayer)
        {
            int count = CountOnSingleDirection(direction, currentX, currentY, currentPlayer) +
                CountOnSingleDirection(Constants.GetReverseDirection(direction), currentX, currentY, currentPlayer);

            int openEndsX = 0;
            while (count > 100)
            {
                count = count - 100;
                openEndsX++;
            }

            if (count >= 4)
                count *= count;
            else if (count >= 3)
                count *= 2;

            //add back the ends.
            count += openEndsX;

            return count;
        }

        private int CountOnSingleDirectionUsingBoard(NodeLocation direction, int currentX, int currentY, TicTacToeValue currentPlayer)
        {
            int count = 0;
            string nodeKey = Constants.MapDirectionToComputation[direction](currentX, currentY);
            if (!nodes.ContainsKey(nodeKey))
            {
                return count;
            }

            Debug.WriteLine($"Checking node {nodeKey}");
            TraverseBoard(nodes[nodeKey], direction, (node) =>
            {
                if (node.Node.Value == TicTacToeValue.x)
                    count++;
                // check to see if there is a next neighbor
                string nk = Constants.MapDirectionToComputation[direction](node.X, node.Y);
                if (!nodes.ContainsKey(nk))
                {
                    if (count >= 2)
                        // we don't have a neighbour blocking us
                        count += 100;
                }
            });
            return count;
        }

        private int CountOnDirection(NodeLocation direction, int currentX, int currentY, TicTacToeValue currentPlayer)
        {
            int countX2 = 0;
            string nodeKey = Constants.MapDirectionToComputation[direction](currentX, currentY);
            if (nodes.ContainsKey(nodeKey))
            {
                Debug.WriteLine($"Checking node {nodeKey}");
                TraverseBoard(nodes[nodeKey], direction, (node) =>
                {
                    if (node.Node.Value == TicTacToeValue.x)
                        countX2++;
                    // check to see if there is a next neighbor
                    string nk = Constants.MapDirectionToComputation[direction](node.X, node.Y);
                    if (!nodes.ContainsKey(nk))
                    {
                        if (countX2 >= 2)
                            // we don't have a neighbour blocking us
                            countX2 += 100;
                    }
                });
            }

            nodeKey = Constants.MapDirectionToComputation[direction.GetReverseDirection()](currentX, currentY);
            int countx3 = 0;
            if (nodes.ContainsKey(nodeKey))
            {
                Debug.WriteLine($"Checking node {nodeKey}");
                TraverseBoard(nodes[nodeKey], direction.GetReverseDirection(), (node) =>
                {
                    if (node.Node.Value == TicTacToeValue.x)
                        countx3++;

                    // check to see if there is a next neighbor
                    string nk = Constants.MapDirectionToComputation[direction.GetReverseDirection()](node.X, node.Y);
                    if (!nodes.ContainsKey(nk))
                    {
                        if (countx3 >= 2)
                            // we don't have a neighbour blocking us
                            countx3 += 100;
                    }
                });
            }

            countX2 += countx3;

            int openEndsX = 0;
            while (countX2 > 100)
            {
                countX2 = countX2 - 100;
                openEndsX++;
            }

            if (countX2 >= 4)
                countX2 *= countX2;
            else if (countX2 >= 3)
                countX2 *= 2;

            //add back the ends.
            countX2 += openEndsX;

            int countX = CountOnDirectionCore(direction, currentX, currentY, TicTacToeValue.x);
            Debug.Assert(countX2 == countX);

            int countO = CountOnDirectionCore(direction, currentX, currentY, TicTacToeValue.o);

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
