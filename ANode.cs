using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace extermin4tus.Pathfinding
{
    public class ANode
    {
        public Vector3Int position { get; private set; } // position in TIleMap coordinates
        public Vector3 worldPosition { get; private set; } // position in Global coordinates

        public int gCost { get; set; } = 0;
        public int hCost { get; set; } = 0;
        public int fCost
        {
            get { return gCost + hCost; }
        }
        public bool walkable = false;

        public ANode(Vector3Int position)
        {
            this.position = position;
        }

        public ANode(Vector3Int position, Vector3 worldPosition)
        {
            this.position = position;
            this.worldPosition = worldPosition;
        }

        public ANode previous = null;
        public List<ANode> neighbors { get; private set; } = new List<ANode>();

        public void SetNeighbors(GridMap gridMap)
        {
            if (position.x > gridMap.bounds.startX) {
                neighbors.Add(gridMap.allNodes[new Vector3Int(position.x - 1, position.y, 0)]);
            }
            if (position.y > gridMap.bounds.startY) {
                neighbors.Add(gridMap.allNodes[new Vector3Int(position.x, position.y - 1, 0)]);
            }
            if (position.x < gridMap.bounds.endX) {
                neighbors.Add(gridMap.allNodes[new Vector3Int(position.x + 1, position.y, 0)]);
            }
            if (position.y < gridMap.bounds.endY) {
                neighbors.Add(gridMap.allNodes[new Vector3Int(position.x, position.y + 1, 0)]);
            }
        }
    }
}

