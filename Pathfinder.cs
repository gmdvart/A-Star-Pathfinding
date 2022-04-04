using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace extermin4tus.Pathfinding
{
    public class Pathfinder
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGONAL_COST = 14;

        private GridMap m_GridMap;
        private List<ANode> openList;
        private List<ANode> closeList;
        private ANode startNode;
        private ANode goalNode;

        public Pathfinder(int columns, int rows, Vector3 gridPivot)
        {
            m_GridMap = new GridMap(columns, rows, gridPivot);
        }

        public Pathfinder(GridMap gridMap)
        {
            m_GridMap = gridMap;
        }

        public List<ANode> CalculatePath(Vector3 start, Vector3 goal)
        {
            openList = new List<ANode>();
            closeList = new List<ANode>();

            foreach (var node in m_GridMap.allNodes.Values)
            {
                node.gCost = int.MaxValue;
                node.previous = null;

                bool isCollidable = Physics2D.OverlapCircle(node.worldPosition, 0.05f);

                if (isCollidable) {
                    node.walkable = false;
                } else {
                    node.walkable = true;
                }
            }

            startNode = m_GridMap.GetNode(start);
            goalNode = m_GridMap.GetNode(goal);

            startNode.gCost = 0;
            startNode.hCost = CalculateDistannce(startNode, goalNode);

            openList.Add(startNode);
            
            while (openList.Count > 0)
            {
                var currentNode = LowestF();

                if (currentNode == goalNode) {
                    return GetPath(currentNode);
                }

                openList.Remove(currentNode);
                closeList.Add(currentNode);

                foreach (var neighbor in currentNode.neighbors)
                {
                    if (neighbor.walkable && !closeList.Contains(neighbor))
                    {
                        int tent_gCost = currentNode.gCost + CalculateDistannce(currentNode, goalNode);

                        if (tent_gCost < neighbor.gCost)
                        {
                            neighbor.previous = currentNode;
                            neighbor.gCost = tent_gCost;
                            neighbor.hCost = CalculateDistannce(neighbor, goalNode);

                            if (!openList.Contains(neighbor)) {
                                openList.Add(neighbor);
                            }
                        }
                    }
                }
            }
            
            return null;

            // TO DO: Walkable || Unwalkable tile determination
        }

        private List<ANode> GetPath(ANode from)
        {
            List<ANode> path = new List<ANode>();

            path.Add(from);

            while(from.previous != null)
            {
                path.Add(from.previous);
                from = from.previous;
            }

            path.Reverse();
            return path;
        }

        private int CalculateDistannce(ANode from, ANode to)
        {
            int xDist = Mathf.Abs(from.position.x - to.position.x);
            int yDist = Mathf.Abs(from.position.y - to.position.y);
            int delta = Mathf.Abs(xDist - yDist);

            return MOVE_DIAGONAL_COST * Mathf.Min(xDist, yDist) + MOVE_STRAIGHT_COST * delta;
        }

        private ANode LowestF()
        {
            int optimalIndex = 0;

            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].fCost < openList[optimalIndex].fCost) {
                    optimalIndex = i;
                }
            }

            return openList[optimalIndex];
        }
    }
}
