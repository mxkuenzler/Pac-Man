 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PacManScript : MonoBehaviour
{
    public float movespeed = 5;
    public float offsetAllowance = 0.5f;
    public MapManagerScript manager;
    public NavigationScript nav;

    int queuedDirection;
    int direction;
    Vector3 previousPos;
    //Vector3[] directions = new Vector3[4];
    int[] rotations = { 0, 90, 180, 270 };


    // Start is called before the first frame update
    void Start()
    {
        previousPos = transform.position;

        //directions[0] = new Vector3(1, 0);
        //directions[1] = new Vector3(0, 1);
        //directions[2] = new Vector3(-1, 0);
        //directions[3] = new Vector3(0, -1);

    }

    // Update is called once per frame
    void Update()
    {

        previousPos = transform.position;
        //Debug.Log("intent" + v3toNearestV2Int(transform.position + directions[direction]));
        if (nav.isValidPath(v3toNearestV2Int(transform.position + nav.directions[direction]/2)))
        {
            transform.position += nav.directions[direction] * movespeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, rotations[direction]);
        }
        else
        {
            transform.position = v2IntToV3(v3toNearestV2Int(transform.position));
        }
        if (Mathf.Abs(transform.position.x) % 1 < offsetAllowance && Mathf.Abs(transform.position.y) % 1 < offsetAllowance)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow)) { queuedDirection = 0; }
            if (Input.GetKeyDown(KeyCode.UpArrow)) { queuedDirection = 1; }
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { queuedDirection = 2; }
            if (Input.GetKeyDown(KeyCode.DownArrow)) { queuedDirection = 3; }
        }

        if (direction == queuedDirection + 2 || direction == queuedDirection - 2)
        {
            direction = queuedDirection;
        }
        else if (direction != queuedDirection)
        {
            if (transform.position == previousPos)
            {
                nav.tryStillTurn(ref direction, queuedDirection, transform.position);
            }
            else
            {
                nav.tryMovingTurn(ref direction, queuedDirection, transform.position, previousPos);
            }
        }


    }
    /*
    void tryStillTurn()
    {
        if (directions[direction].x != 0)
        {
            Vector2Int newVec = new Vector2Int((int)previousPos.x, (int)previousPos.y)
                                + new Vector2Int((int)directions[queuedDirection].x, (int)directions[queuedDirection].y);

            if (isValidPath(newVec))
            {
                direction = queuedDirection;
            }
            
        }
        else
        {
            Vector2Int newVec = new Vector2Int((int)previousPos.x, (int)previousPos.y)
                                    + new Vector2Int((int)directions[queuedDirection].x, (int)directions[queuedDirection].y);

            if (isValidPath(newVec))
            {
                direction = queuedDirection;
            }
            
        }
    }

    void tryMovingTurn()
    {
        if (directions[direction].x != 0)
        {
            int currentSign = transform.position.x > 0 ? 1 : -1;
            int prevSign = previousPos.x > 0 ? 1 : -1;
            int current = (int)(transform.position.x);
            int prev = (int)(previousPos.x);
            if (current != prev || currentSign != prevSign)
            {
                Vector2Int newVec = new Vector2Int((Mathf.Abs(current) > Mathf.Abs(prev) ? current : prev), (int)transform.position.y) 
                                    + new Vector2Int((int)directions[queuedDirection].x, (int)directions[queuedDirection].y);

                if (isValidPath(newVec))
                {
                    transform.position = new Vector3((Mathf.Abs(current) > Mathf.Abs(prev) ? current : prev), transform.position.y);
                    direction = queuedDirection;
                }
            }
        }
        else
        {
            int currentSign = transform.position.y > 0 ? 1 : -1;
            int prevSign = previousPos.y > 0 ? 1 : -1;
            int current = (int)(transform.position.y);
            int prev = (int)(previousPos.y);
            if (current != prev || currentSign != prevSign)
            {
                Vector2Int newVec = new Vector2Int((int)transform.position.x, (Mathf.Abs(current) > Mathf.Abs(prev) ? current : prev)) 
                                     + new Vector2Int((int)directions[queuedDirection].x, (int)directions[queuedDirection].y);

                if ( isValidPath(newVec))
                {
                    transform.position = new Vector3(transform.position.x, (Mathf.Abs(current) > Mathf.Abs(prev) ? current : prev));
                    direction = queuedDirection;
                }
            }
        }
    }

    bool isValidPath(Vector2Int intendedMove)
    {
        //Debug.Log("Intent; " + intendedMove + " " + queuedDirection);
        IEnumerable<Vector2Int> map = manager.map;
        foreach (Vector2Int v in map)
        {
            if(intendedMove == v)
            {
                //Debug.Log("Valid");
                return true;
            }
        }
        return false;
    }
    */
    bool movingOut()
    {
        if (nav.directions[direction].x * transform.position.x < 0 || nav.directions[direction].y * transform.position.y < 0)
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
    }
}
