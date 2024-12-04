using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private TileBase[] tileBorders = new TileBase[16];

    [SerializeField]
    private int numberOfBoxes, maxBoxSize, numberOfTrails;

    [SerializeField]
    private GameObject camera;

    [SerializeField]
    private GameObject pellet;

    [SerializeField]
    private GameManagerScript gameManager;

    public HashSet<Vector2Int> map;

    private ArrayList pellets = new ArrayList();

    private void Start()
    {
        //generateMap();
    }

    private void Update()
    {
        if(pellets.Count == 0)
        {
            generateMap();
        }
    }

    public void removePellet(GameObject pel)
    {
        pellets.Remove(pel);
    }

    /*
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
    }*/

    public void newpaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        HashSet<Vector2Int> tilePositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            tilePositions.Add(position);
            tilePositions.Add(position + Directions.up());
            tilePositions.Add(position + Directions.right());
            tilePositions.Add(position + Directions.up() + Directions.right());
        }

        newpaintTiles(floorPositions, tilePositions, floorMap);
    }

    private void newpaintTiles(IEnumerable<Vector2Int> floorPositions, IEnumerable<Vector2Int> tilePositions, Tilemap map)
    {
        foreach (var position in tilePositions)
        {
            newpaintSingleTile(floorPositions, map, position);
        }
    }

    private void newpaintSingleTile(IEnumerable<Vector2Int> positions, Tilemap map, Vector2Int position)
    {
        //cell origin is + .5,.5 from world origin
        var tilePos = map.WorldToCell((Vector3Int)position);
        int correctTile = 0;

        if (positions.Contains(position)) { correctTile += 2; }
        if (positions.Contains(position + Directions.down())) { correctTile += 8; }
        if (positions.Contains(position + Directions.left())) { correctTile += 1; }
        if (positions.Contains(position + Directions.down() + Directions.left())) { correctTile += 4; }

        map.SetTile(tilePos, tileBorders[correctTile]);
    }

    [ContextMenu("Print Boxes")]
    public void generateMap()
    {
        clear();
        Quaternion q = new Quaternion();
        //map = ProceduralGenerator.generateBoxPerimiterPath(numberOfBoxes, maxBoxSize);
        //map = ProceduralGenerator.generateLineCastPath(numberOfBoxes, maxBoxSize);
        map = ProceduralGenerator.GenerateConnectedBoxes(numberOfBoxes, maxBoxSize, numberOfTrails);
        foreach(Vector2Int pos in map)
        {
            pellets.Add(Instantiate(pellet, new Vector3(pos.x, pos.y), q));
        }
        newpaintFloorTiles(map);

        StartCoroutine(gameManager.StartGame());
        //reframeCamera();

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

    [ContextMenu("Clear Map")]
    public void clear()
    {
        floorMap.ClearAllTiles();
        pellets.Clear();
    }
}
