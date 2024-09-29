using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class ProceduralGenerator
{
    public static HashSet<Vector2Int> generateBoxPerimiterPath(int numberOfBoxes, int maxSize)
    {
        HashSet<Vector2Int> positions = new();
        for (int i = 0; i < numberOfBoxes; i++)
        {
            int x = UnityEngine.Random.Range(0, maxSize);
            int y = UnityEngine.Random.Range(0, maxSize);
            int size = UnityEngine.Random.Range(1, maxSize);
            var point = new Vector2Int(x, y);
            positions.UnionWith(perimiter(point, size));
        }
        return positions;
    }
    
    static HashSet<Vector2Int> perimiter(Vector2Int center, int size)
    {
        int length = (size - 1) * 8;
        HashSet<Vector2Int> retArr = new();

        for( int x = -size; x <= size; x++)
        {
            for (int y = -size; y <= size; y++)
            {
                if(Math.Abs(x) == size || Math.Abs(y) == size)
                {
                    retArr.Add(center + new Vector2Int(x, y));
                }
            }
        }

        return retArr;
    }

    public static HashSet<Vector2Int> generateLineCastPath(int numberOfLines, int maxLength)
    {
        HashSet<Vector2Int> positions = new();
        positions.Add(new Vector2Int(0, 0));
        for (int i = 0; i < numberOfLines; i++)
        {
            Vector2Int start;
            Vector2Int shift = Directions.random();
            do
            {
                int r = UnityEngine.Random.Range(0, positions.Count);
                start = positions.ElementAt(r) + shift;
            } while (HashSetContains(positions, start));
            for(int j = 0; j < UnityEngine.Random.Range(0, maxLength); j++)
            {
                positions.Add(start);
                start += shift;
            }
        }


        return positions;
    }

    //use bigger maxSize numbers for better results
    public static HashSet<Vector2Int> GenerateConnectedBoxes(int numberOfBoxes, int maxSize, int numberOfTrails)
    {
        Debug.Log("Generating...");
        HashSet<Vector2Int> positions = new();
        //adds the boxes
        for (int i = 0; i < numberOfBoxes; i++)
        {
            int x = UnityEngine.Random.Range(-maxSize, maxSize);
            int y = UnityEngine.Random.Range(-maxSize, maxSize);
            int size = UnityEngine.Random.Range(3, maxSize);
            var point = new Vector2Int(x, y);
            //check for adjacency
            positions.UnionWith(perimiter(point, size));
        }

        Debug.Log("Trails");
        //adds trails within and between boxes
        int fails = 0;
        for (int i = 0; i < numberOfTrails && fails < 20; i++)
        {
            int x = UnityEngine.Random.Range(-maxSize * 2, maxSize * 2);
            int y = UnityEngine.Random.Range(-maxSize * 2, maxSize * 2);
            var point = new Vector2Int(x, y);
            //find the closest tiles along primary axes, at least 2
            if (!positions.Contains(point) && IsSpaced(positions, point))
            {
                Debug.Log(point);
                if (!NearestPaths(point, positions, maxSize * 4))
                {
                    Debug.Log("Nah");
                    --i;
                }
            }
            else { i--; fails++;  }

        }

        return positions;
    }

    static bool NearestPaths(Vector2Int pos, HashSet<Vector2Int> positions, int maxSize)
    {
        HashSet<Vector2Int> left = new();
        HashSet<Vector2Int> right = new();
        HashSet<Vector2Int> up = new();
        HashSet<Vector2Int> down = new();

        left.Add(pos);
        right.Add(pos);
        up.Add(pos);
        down.Add(pos);

        int a = 0;
        bool l = true, r = true, u = true, d = true;

        while (a < 2 && left.Count() < maxSize && right.Count() < maxSize)
        {
            if (l)
            {
                if (!positions.Contains(left.Last() + Directions.left()))
                {
                    left.Add(left.Last() + Directions.left());
                }
                else
                {
                    a++;
                    l = false;
                }
            }

            if (r)
            {
                if (!positions.Contains(right.Last() + Directions.right()))
                {
                    right.Add(right.Last() + Directions.right());
                }
                else
                {
                    positions.UnionWith(right);
                    a++;
                    r = false;
                }
            }

            if (u)
            {
                if (!positions.Contains(up.Last() + Directions.up()))
                {
                    up.Add(up.Last() + Directions.up());
                }
                else
                {
                    positions.UnionWith(up);
                    a++;
                    u = false;
                }
            }

            if (d)
            {
                if (!positions.Contains(down.Last() + Directions.down()))
                {
                    down.Add(down.Last() + Directions.down());
                }
                else
                {
                    positions.UnionWith(down);
                    a++;
                    d = false;
                }
            }

        }

        if (a > 1)
        {
            if (!l) { positions.UnionWith(left); }

            if (!r) { positions.UnionWith(right); }

            if (!u) { positions.UnionWith(up); }

            if (!d) { positions.UnionWith(down); }

            return true;
        }

        return false;
    }

    private static bool HashSetContains(HashSet<Vector2Int> set, Vector2Int pos)
    {
        foreach (Vector2Int v in set)
        {
            if ( v == pos)
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsSpaced(HashSet<Vector2Int> set, Vector2Int pos)
    {
        foreach (Vector2Int v in set)
        {
            int deltaX = v.x - pos.x;
            int deltaY = v.y - pos.y;

            if(deltaX  < 2 && deltaX > -2 && deltaY < 2 && deltaY > -2)
            {
                return false;
            }
        }

        return true;
    }
}

public static class Directions
{
    public static Vector2Int up()
    {
        return new Vector2Int(0, 1);
    }

    public static Vector2Int down()
    {
        return new Vector2Int(0, -1);
    }

    public static Vector2Int left()
    {
        return new Vector2Int(-1, 0);
    }

    public static Vector2Int right()
    {
        return new Vector2Int(1, 0);
    }

    public static Vector2Int random()
    {
        int r = UnityEngine.Random.Range(0, 4);
        switch (r)
        {
            case 0:
                return right();
            case 1:
                return up();
            case 2:
                return left();
            case 3:
                return down();
            default:
                return up();
        }
    }

    //returns the cardinal directions sorted in order of their closeness to vec
    public static Vector2Int[] nearestDirection(Vector2Int vec)
    {
        Vector2Int[] dirs = {right(), up(), left(), down() };
        float[] angles = new float [4];

        for (int i = 0; i < dirs.Length; i++) {
            angles[i] = MathF.Abs( MathF.Acos(dotProduct(dirs[i], vec) / vec.magnitude));
        }

        for(int i = 0; i < dirs.Length - 1; i++ )
        {
            for (int j = 0; j < dirs.Length - 1; j++)
            {
                if (angles[j] > angles[j + 1])
                {
                    float temp = angles[j];
                    angles[j] = angles[j + 1];
                    angles[j + 1] = temp;

                    Vector2Int tempVec = dirs[j];
                    dirs[j] = dirs[j + 1];
                    dirs[j + 1] = tempVec;
                }
            }
        }
        //Debug.Log(dirs[0] + " " + dirs[1]);
        return dirs;
    }

    public static int dotProduct(Vector2Int vec1, Vector2Int vec2)
    {
        return vec1.x*vec2.x + vec1.y*vec2.y;
    }
}
