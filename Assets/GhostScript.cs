using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{

    public float movespeed = 5;
    [SerializeField]
    private float turnChance = 0.5f;
    [SerializeField]
    private MapManagerScript manager;
    [SerializeField]
    private NavigationScript nav;


    private int queuedDirection;
    private int direction;
    private Vector3 previousPos;

    private float turnTimer = 0;


    // Start is called before the first frame update
    void Start()
    {
        newMap();
        previousPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        previousPos = transform.position;
        turnTimer += Time.deltaTime;
        Debug.Log(turnTimer + "dir: " + direction);
        if (turnTimer >= 1)
        {
            if(Random.value < turnChance)
            {
                queuedDirection = Random.Range(0, 4);
            }
            turnTimer = 0;
        }

        if (nav.isValidPath(v3toNearestV2Int(transform.position + nav.directions[direction] / 2)))
        {
            transform.position += nav.directions[direction] * movespeed * Time.deltaTime;
        }
        else
        {
            Debug.Log("flip");
            direction += ((direction - 2 < 0) ? 2 : -2);
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

    void newMap()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapManagerScript>();
    }

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
