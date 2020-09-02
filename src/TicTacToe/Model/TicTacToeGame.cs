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
        public bool GameEnded = false;
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
            GameEnded = false;
        }

        public VisualNode AddMove(int x, int y, TicTacToeValue currentPlayer)
        {
            string nodeKey = Constants.GetKey(x, y);
            if (nodes.ContainsKey(nodeKey))
            {
                return null;
            }

            VisualNode newNode = new VisualNode(x, y, currentPlayer);
            nodes.Add(nodeKey, newNode);

            return newNode;
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

        public HashSet<string> GetEmptyNeighBours()
        {
            var result = new HashSet<string>();

            foreach (var node in nodes.Values)
            {
                // look on every direction
                foreach (NodeLocation direction in Enum.GetValues(typeof(NodeLocation)))
                {
                    string nodeKey = Constants.MapDirectionToComputation[direction](node.X, node.Y);

                    if (!nodes.ContainsKey(nodeKey))
                    {
                        result.Add(nodeKey);
                    }
                }
            }

            return result;
        }

        public VisualNode GetAIMove(TicTacToeValue currentPlayer)
        {
            if (nodes.Count == 0)
            {
                return AddMove(15, 15, currentPlayer);
            }

            string myMove = string.Empty;
            // figure out all the empty nodes outthere.
            HashSet<string> positions = GetEmptyNeighBours();

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
                else
                {
                    bestMoveCount = Math.Abs(bestX) > Math.Abs(bestO) ? bestX : bestO;
                }
            }

            Random r = new Random((int)DateTime.Now.Ticks);
            var possibleMoves = mapPositionContributions.Where(pair => pair.Value == bestMoveCount);
            myMove = possibleMoves.Skip(r.Next(possibleMoves.Count() - 1)).First().Key;
            Debug.WriteLine($"The best move for {currentPlayer} is {myMove}");

            int x = 0, y = 0;
            Constants.GetCoordinates(myMove, out x, out y);
            return AddMove(x, y, currentPlayer);
        }

        internal bool IsWinningMove(VisualNode move, out NodeLocation winDirection)
        {
            int count = 1;
            Action<VisualNode> Count = (n) =>
            {
                // we should not count the original move as we count that ouside of this loop.
                if (n.X == move.X && n.Y == move.Y)
                {
                    return;
                }

                if (n.Value == move.Value)
                    count++;
            };

            count = 1;
            winDirection = NodeLocation.TopLeft;
            TraverseBoard(move, winDirection, Count);
            TraverseBoard(move, winDirection.GetReverseDirection(), Count);
            if (count >= NeededForWin)
                return true;

            count = 1;
            winDirection = NodeLocation.TopCenter;
            TraverseBoard(move, winDirection, Count);
            TraverseBoard(move, winDirection.GetReverseDirection(), Count);
            if (count >= NeededForWin)
                return true;

            count = 1;
            winDirection = NodeLocation.TopRight;
            TraverseBoard(move, winDirection, Count);
            TraverseBoard(move, winDirection.GetReverseDirection(), Count);
            if (count >= NeededForWin)
                return true;

            count = 1;
            winDirection = NodeLocation.Left;
            TraverseBoard(move, winDirection, Count);
            TraverseBoard(move, winDirection.GetReverseDirection(), Count);
            if (count >= NeededForWin)
                return true;


            return false;
        }

        private void TraverseBoard(VisualNode node, NodeLocation direction, Action<VisualNode> runAction)
        {
            // flag all the nodes on the winDirection
            var currentNode = nodes[Constants.GetKey(node.X, node.Y)];
            TicTacToeValue nodeValue = node.Value;

            // traverse all nodes of the same value
            while (currentNode.Value == nodeValue)
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

        private AIMove CountOnSingleDirectionUsingBoard(NodeLocation direction, int currentX, int currentY, TicTacToeValue currentPlayer)
        {
            AIMove move = new AIMove();
            //int count = 0;
            string nodeKey = Constants.MapDirectionToComputation[direction](currentX, currentY);
            if (!nodes.ContainsKey(nodeKey))
            {
                return move;
            }

            TraverseBoard(nodes[nodeKey], direction, (node) =>
            {
                if (node.Value == currentPlayer)
                {
                    move.countPerDirection++;
                }
                else
                {
                    move.endsWithOpponentMove = true;
                }

                // check to see if there is a next neighbor
                string nk = Constants.MapDirectionToComputation[direction](node.X, node.Y);
                if (!nodes.ContainsKey(nk))
                {
                    move.endsWithOpponentMove = false;
                }
                else
                {
                    if (nodes[nk].Value != currentPlayer)
                    {
                        move.endsWithOpponentMove = true;
                    }
                }
            });
            return move;
        }

        private int CountOnBothDirectionsUsingBoard(NodeLocation direction, int currentX, int currentY, TicTacToeValue currentPlayer)
        {
            var moveFirstDir = CountOnSingleDirectionUsingBoard(direction, currentX, currentY, currentPlayer);
            var moveSecondDir = CountOnSingleDirectionUsingBoard(direction.GetReverseDirection(), currentX, currentY, currentPlayer);

            int count = moveFirstDir.countPerDirection + moveSecondDir.countPerDirection;

            // this is the case where we have something like: o x _ x x o 
            // there is no point to put an o in there as that can never ever be a winning move from x
            if (count <= 3 && (moveFirstDir.endsWithOpponentMove && moveSecondDir.endsWithOpponentMove))
                count = 0;

            if (count >= 4)
                count *= count;
            else if (count >= 3)
                count *= 2;

            if (count >= 2 && !moveFirstDir.endsWithOpponentMove)
                count++;

            if (count >= 2 && !moveSecondDir.endsWithOpponentMove)
                count++;

            return count;
        }

        private int CountOnDirection(NodeLocation direction, int currentX, int currentY, TicTacToeValue currentPlayer)
        {
            int countForX = CountOnBothDirectionsUsingBoard(direction, currentX, currentY, TicTacToeValue.x);
            int countForO = CountOnBothDirectionsUsingBoard(direction, currentX, currentY, TicTacToeValue.o);

            return currentPlayer == TicTacToeValue.x ? countForX - countForO : countForO - countForX;
        }

        internal void RemoveNode(VisualNode node)
        {
            nodes.Remove($"{Constants.GetKey(node.X, node.Y)}");

            // reset this in case we are undo-ing after the game has ended.
            GameEnded = false;
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
