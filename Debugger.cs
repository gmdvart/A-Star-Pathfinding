using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using extermin4tus.Pathfinding;

public class Debugger : MonoBehaviour
{
    private enum TileType 
    { 
        BASE,
        START,
        GOAL,
        PATH, 
    }

    private TileType tileType;
    
    private Color m_ClearColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    private Color m_BaseColor = Color.white;
    private Color m_StartColor = Color.green;
    private Color m_GoalColor = Color.red;
    private Color m_PathColor = Color.yellow;
    
    private Vector3Int m_CachedStart;
    private Vector3Int m_CachedGoal;
    private List<Vector3Int> m_CahcedPath = new List<Vector3Int>();

    [Header("Vizualization")]
    [SerializeField] private Tile m_DebugTile;
    [SerializeField] private Tilemap m_DebugTilemap;
    
    private GridMap gridMap;
    private Pathfinder pathfinder;
    private Vector3 firstClickPos;
    private Vector3 lastClickPos;
    private bool first = false, last = false;
    

    private void Start()
    {
        gridMap = new GridMap(5, 5, transform.position);
        pathfinder = new Pathfinder(gridMap);

        foreach (var item in gridMap.allNodes)
        {
            m_CachedStart = item.Value.position;
            m_CachedGoal = item.Value.position;
            m_CahcedPath.Add(item.Value.position);

            SetTile(item.Value.position, TileType.BASE);
        }
    }

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (gridMap.InBounds(mousePosition))
        {
            ANode clickedTile = gridMap.GetNode(mousePosition);            
            Vector3Int tilePos = clickedTile.position;

            if (Input.GetMouseButton(0))
            {
                if (clickedTile.walkable)
                {
                    firstClickPos = mousePosition;

                    if (m_CachedStart != null)
                        SetTile(m_CachedStart, TileType.BASE);
                        
                    SetTile(tilePos, TileType.START);
                    m_CachedStart = tilePos;
                    first = true;
                }
            }
            else if (Input.GetMouseButton(1))
            {
                if (clickedTile.walkable)
                {
                    lastClickPos = mousePosition;

                    if (m_CachedGoal != null)
                        SetTile(m_CachedGoal, TileType.BASE);

                    SetTile(tilePos, TileType.GOAL);
                    m_CachedGoal = tilePos;
                    last = true;
                }
            }
        }

        if (first && last)
        {
            if (m_CahcedPath.Count > 0)
            {
                foreach (var nodePos in m_CahcedPath)
                    SetTile(nodePos, TileType.BASE);
            }

            var path = pathfinder.CalculatePath(firstClickPos, lastClickPos);

            foreach (var node in path)
            {
                SetTile(node.position, TileType.PATH);
                m_CahcedPath.Add(node.position);
            }
        }
    }

    private void SetTile(Vector3Int position, TileType tileType)
    {
        m_DebugTilemap.SetTile(position, m_DebugTile);
        m_DebugTilemap.SetTileFlags(position, TileFlags.None);

        if (tileType == TileType.BASE)
        {
            m_DebugTile.color = m_BaseColor;
        }
        else if (tileType == TileType.START)
        {
            m_DebugTile.color = m_StartColor;
        }
        else if (tileType == TileType.GOAL)
        {
            m_DebugTile.color = m_GoalColor;
        }
        else if (tileType == TileType.PATH)
        {
            m_DebugTile.color = m_PathColor;
        }

        m_DebugTilemap.SetColor(position, m_DebugTile.color);
    }
}
