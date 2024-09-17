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
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>();
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
        HashSet<Vector2Int> retArr = new HashSet<Vector2Int>();

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
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>();
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

   // public static float angleBetw

    public static int dotProduct(Vector2Int vec1, Vector2Int vec2)
    {
        return vec1.x*vec2.x + vec1.y*vec2.y;
    }
}
