using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace extermin4tus.Pathfinding
{
    public class GridMap
    {
        private Vector3 m_Pivot;
        public int columns { get; private set; } = 3;
        public int rows { get; private set; } = 3;

        private Grid m_GridBase;
        private Vector3 m_UnitScale = Vector3.one;
        public GridMapBounds bounds { get; private set; }

        public IDictionary<Vector3Int, ANode> allNodes { get; private set; } = new Dictionary<Vector3Int, ANode>();

        public GridMap(int rows, int columns, Vector3 pivot)
        {
            m_Pivot = pivot;
            this.columns = columns;
            this.rows = rows;

            m_GridBase = MonoBehaviour.FindObjectOfType<Grid>();

            if (m_GridBase != null) {
                m_UnitScale = m_GridBase.cellSize;
            } else {
                Debug.LogError("Unable to assign 'Grid' component to the m_GridBase. Please add 'Grid' to your scene.");
                return;
            }

            bounds = new GridMapBounds(m_GridBase.WorldToCell(m_Pivot), columns, rows);

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Vector3 worldPosition = new Vector3(m_Pivot.x + i * m_UnitScale.x, m_Pivot.y + j * m_UnitScale.y, 0.0f);
                    Vector3Int nodePosition = m_GridBase.WorldToCell(worldPosition);
                    allNodes.Add(nodePosition, new ANode(nodePosition, worldPosition));

                    bool isCollidable = Physics2D.OverlapCircle(worldPosition, 0.05f);
                    if (isCollidable) {
                        allNodes[nodePosition].walkable = false;
                    } else {
                        allNodes[nodePosition].walkable = true;
                    }
                }
            }

            foreach (var node in allNodes.Values)
            {
                node.SetNeighbors(this);
            }
        }

        public ANode GetNode(Vector3 position)
        {
            Vector3Int convertedPos = m_GridBase.WorldToCell(position);

            if (allNodes.ContainsKey(convertedPos)) {
                return allNodes[convertedPos];
            }

            return null;
        }
        
        public bool InBounds(Vector3 position)
        {
            float xStart = m_Pivot.x - m_UnitScale.x / 3;
            float xEnd = m_Pivot.x + m_UnitScale.x * (columns - 1) + m_UnitScale.x / 3;
            bool xBounds = position.x > xStart && position.x < xEnd;

            float yStart = m_Pivot.y - m_UnitScale.y / 3;
            float yEnd = m_Pivot.y + m_UnitScale.y * (rows - 1) + m_UnitScale.y / 3;
            bool yBounds = position.y > yStart && position.y < yEnd;

            return xBounds && yBounds;
        }

        public class GridMapBounds
        {
            public int startX { get; private set; }
            public int startY { get; private set; }
            public int endX { get; private set; }
            public int endY { get; private set; }

            public GridMapBounds(Vector3Int pivot, int columns, int rows)
            {
                startX = pivot.x;
                startY = pivot.y;

                endX = pivot.x + (columns - 1);
                endY = pivot.y + (rows - 1);
            }
        }
    }
}

