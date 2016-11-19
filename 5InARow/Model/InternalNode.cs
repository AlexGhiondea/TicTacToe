using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class InternalNode
    {
        Dictionary<NodeLocation, InternalNode> _neighbours = new Dictionary<NodeLocation, InternalNode>();

        public int CountOnDirection(NodeLocation direction, TicTacToeValue value)
        {
            // starting from this direction count how many there are
            InternalNode nextNode;

            if (_neighbours.TryGetValue(direction, out nextNode) && nextNode.Value == value)
            {
                // we have a neighbor there of the same value
                return 1 + nextNode.CountOnDirection(direction, value);
            }

            return 0;
        }

        public void NavigateADirection(NodeLocation direction, TicTacToeValue previousValue, ref int currentCount)
        {
            // starting from this direction count how many there are
            InternalNode nextNode;

            if (Value == previousValue)
            {
                currentCount++;
            }

            if (_neighbours.TryGetValue(direction, out nextNode))
            {
                // we have a neighbor there of the same value
                if (Value == previousValue)
                {
                    //currentCount++;
                    nextNode.NavigateADirection(direction, previousValue, ref currentCount);
                    return;
                }
                // we still have a neighbour, blocking us
            }
            else
            {
                if (currentCount >= 3)
                    // we don't have a neighbour blocking us
                    currentCount += 2;
            }
        }

        public int AddAllOnDirection(NodeLocation direction, TicTacToeValue? previousValue)
        {
            // starting from this direction count how many there are
            InternalNode nextNode;

            if (_neighbours.TryGetValue(direction, out nextNode))
            {
                // we have a neighbor there of the same value
                if (previousValue != null && Value == previousValue)
                {
                    return (Value == TicTacToeValue.x ? 1 : -1) + nextNode.AddAllOnDirection(direction, previousValue);
                }
                else if (previousValue == null)
                {
                    return (Value == TicTacToeValue.x ? 1 : -1) + nextNode.AddAllOnDirection(direction, Value);
                }
                else
                {
                    // the next value is different than the current one.

                    //we should just stop, as only next node will only have impact (not everything on the row).
                    return Value == TicTacToeValue.x ? 1 : -1;
                }
            }

            // no more neighbours.
            return (Value == TicTacToeValue.x ? 1 : -1);
        }

        public TicTacToeValue Value;
        public InternalNode(TicTacToeValue value)
        {
            Value = value;
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

        public IEnumerable<NodeLocation> GetEmptyNodesAroundIt()
        {
            // see which of the directions does not have anything in the dictionary
            foreach (NodeLocation direction in Enum.GetValues(typeof(NodeLocation)))
            {
                if (!_neighbours.ContainsKey(direction))
                    yield return direction;
            }
        }
    }
}
