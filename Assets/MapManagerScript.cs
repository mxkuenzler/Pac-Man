using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManagerScript : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorMap;

    [SerializeField]
    private TileBase baseTile;

    [SerializeField]
    private TileBase[] tiles = new TileBase[16];

    [SerializeField]
    private int numberOfBoxes, maxBoxSize;

    [SerializeField]
    private GameObject camera;

    private void Start()
    {
        generateMap();
    }


    public HashSet<Vector2Int> map;
    public void paintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        paintTiles(floorPositions, floorMap, baseTile);
    }

    private void paintTiles(IEnumerable<Vector2Int> floorPositions, Tilemap map, TileBase tile)
    {
        foreach (var position in floorPositions) 
        {
            paintSingleTile(floorPositions, map, tile, position);
        }
    }

    private void paintSingleTile(IEnumerable<Vector2Int> positions, Tilemap map, TileBase tile, Vector2Int position)
    {
        var tilePos = map.WorldToCell((Vector3Int)position);
        int correctTile = 0;
        Vector2Int[] directions = {Vector2Int.down, Vector2Int.left, Vector2Int.up, Vector2Int.right };
        for (int i = 0; i <= 3; i++) 
        {
            Vector2Int relativePos = position + directions[i];
            foreach (var pos in positions)
            {
                if(pos == relativePos)
                {
                    correctTile += (int)Math.Pow(2, i);
                }
            }
        }
        //Debug.Log(position + " of " + correctTile);
        map.SetTile(tilePos, tiles[correctTile]);
    }

    [ContextMenu("Print Boxes")]
    public void generateMap()
    {
        clear();
        //map = ProceduralGenerator.generateBoxPerimiterPath(numberOfBoxes, maxBoxSize);
        map = ProceduralGenerator.generateLineCastPath(numberOfBoxes, maxBoxSize);
        paintFloorTiles(map);
        reframeCamera();

    }

    private void reframeCamera()
    {
        int minX = 0, maxX = 0, minY = 0, maxY = 0;
        foreach(Vector2Int tile in map)
        {
            if(tile.x < minX)
            {
                minX = tile.x;
            }
            if (tile.x > maxX)
            {
                maxX = tile.x;
            }
            if (tile.y < minY)
            {
                minY = tile.y;
            }
            if (tile.y > maxY)
            {
                maxY = tile.y;
            }
        }
        int xRange = maxX - minX;
        int yRange = maxY - minY;
        camera.transform.position = new Vector3((minX + maxX) /2, (minY + maxY) /2, camera.transform.position.z);
        camera.GetComponent<Camera>().orthographicSize = (3.5 * xRange > 2 * yRange ? xRange / 2 : yRange / 2) + 2;
    }

    public void clear()
    {
        floorMap.ClearAllTiles();
    }
}
