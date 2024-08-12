using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationScript : MonoBehaviour
{
    public MapManagerScript manager;

    public Vector3[] directions = { new Vector3(1, 0), new Vector3(0, 1), new Vector3(-1, 0), new Vector3(0, -1) };

    private void Start()
    {

        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapManagerScript>();
    }

    public void tryStillTurn(ref int direction, int queuedDirection, Vector3 pos)
    {
        if (directions[direction].x != 0)
        {
            Vector2Int newVec = new Vector2Int((int)pos.x, (int)pos.y)
                                + new Vector2Int((int)directions[queuedDirection].x, (int)directions[queuedDirection].y);

            if (isValidPath(newVec))
            {
                direction = queuedDirection;
            }

        }
        else
        {
            Vector2Int newVec = new Vector2Int((int)pos.x, (int)pos.y)
                                    + new Vector2Int((int)directions[queuedDirection].x, (int)directions[queuedDirection].y);

            if (isValidPath(newVec))
            {
                direction = queuedDirection;
            }

        }
    }

    public void tryMovingTurn(ref int direction, int queuedDirection, Vector3 pos, Vector3 prevPos)
    {
        if (directions[direction].x != 0)
        {
            int currentSign = pos.x > 0 ? 1 : -1;
            int prevSign = prevPos.x > 0 ? 1 : -1;
            int current = (int)(pos.x);
            int prev = (int)(prevPos.x);
            if (current != prev || currentSign != prevSign)
            {
                Vector2Int newVec = new Vector2Int((Mathf.Abs(current) > Mathf.Abs(prev) ? current : prev), (int)pos.y)
                                    + new Vector2Int((int)directions[queuedDirection].x, (int)directions[queuedDirection].y);

                if (isValidPath(newVec))
                {
                    transform.position = new Vector3((Mathf.Abs(current) > Mathf.Abs(prev) ? current : prev), pos.y);
                    direction = queuedDirection;
                }
            }
        }
        else
        {
            int currentSign = pos.y > 0 ? 1 : -1;
            int prevSign = prevPos.y > 0 ? 1 : -1;
            int current = (int)(pos.y);
            int prev = (int)(prevPos.y);
            if (current != prev || currentSign != prevSign)
            {
                Vector2Int newVec = new Vector2Int((int)pos.x, (Mathf.Abs(current) > Mathf.Abs(prev) ? current : prev))
                                     + new Vector2Int((int)directions[queuedDirection].x, (int)directions[queuedDirection].y);

                if (isValidPath(newVec))
                {
                    transform.position = new Vector3(pos.x, (Mathf.Abs(current) > Mathf.Abs(prev) ? current : prev));
                    direction = queuedDirection;
                }
            }
        }
    }

    public bool isValidPath(Vector2Int intendedMove)
    {
        //Debug.Log("Intent; " + intendedMove + " " + queuedDirection);
        IEnumerable<Vector2Int> map = manager.map;
        foreach (Vector2Int v in map)
        {
            if (intendedMove == v)
            {
                //Debug.Log("Valid");
                return true;
            }
        }
        return false;
    }
    /*
    bool movingOut(int direction)
    {
        if (directions[direction].x * transform.position.x < 0 || directions[direction].y * transform.position.y < 0)
        {
            return false;
        }
        return true;
    }

    Vector2Int v3toNearestV2Int(Vector3 vec)
    {
        //Debug.Log("Vec: " +  vec + " x: " + vec.x + " y: " + vec.y);
        var retVec = new Vector2Int((int)(vec.x + (vec.x >= 0 ? 0.5 : -0.5) - (movingOut() ? 0 : 0.01)), (int)(vec.y + (vec.y >= 0 ? 0.5 : -0.5) - (movingOut() ? 0 : 0.01)));
        //var retVec = new Vector2Int((int)(vec.x + (direction == 2 ? 0.5 : -0.5)), (int)(vec.y + (direction == 3 ? 0.5 : -0.5)));
        //var retVec = new Vector2Int;

        //Debug.Log("ret" + retVec + " x: " + retVec.x + " y: " + retVec.y);
        return retVec;
        //return new Vector2Int((int)vec.x, (int)vec.y);
    }

    Vector3 v2IntToV3(Vector2Int vec)
    {
        return new Vector3(vec.x, vec.y);
    }*/
}
