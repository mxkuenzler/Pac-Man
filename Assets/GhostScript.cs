using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class GhostScript : MonoBehaviour
{

    public float movespeed = 5;
    //[SerializeField]
    //private float turnChance = 0.5f;
    [SerializeField]
    private MapManagerScript manager;
    [SerializeField]
    private NavigationScript nav;
    [SerializeField]
    private GameObject pacMan;
    [SerializeField]
    private bool following = true;

    private int queuedDirection;
    private int direction;
    private Vector3 previousPos;
    public Queue<Vector2Int> turnQueue;

    public float timerCap = 1;
    private float turnTimer = 0;



    // Start is called before the first frame update
    void Start()
    {
        //newMap();
        turnQueue = new Queue<Vector2Int>();
        turnQueue.Enqueue(Directions.left());
        previousPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        

        previousPos = transform.position;

        turnTimer += Time.deltaTime;
        if (turnTimer >= timerCap)
        {
            turnQueue.Enqueue(v3toNearestV2Int(pacMan.transform.position));
            turnTimer = 0;
        }

        
        if (nav.isValidPath(v3toNearestV2Int(transform.position + nav.directions[direction] / 2)))
        {
            transform.position += nav.directions[direction] * movespeed * Time.deltaTime;
        }

        if (following)
        {
            turnQueue.Clear();
            directFollow();
        }
        else
        {
            if (turnQueue.Count > 0)
            {
                if (v3toNearestV2Int(transform.position) == turnQueue.Peek())
                {
                    turnQueue.Dequeue();
                }
                directFollow(turnQueue.Peek());
            }
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
                nav.tryMovingTurn(ref direction, queuedDirection, transform.position, previousPos, gameObject);
            }
        }
    }

    [ContextMenu("add waypoint")]
    public void addWaypoint(Vector2Int vec)
    {
        turnQueue.Enqueue(vec);
    }

    /*void newMap()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapManagerScript>();
        foreach ( Vector2Int pos in manager.map)
        {
            navGrid.Add(new NavigationGridNode(pos));
        }
    }*/

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

    /* MARK - Pathfinding System ***************************************************** */

    //agent based
    
    //goes straight toward the player, gets stuck on walls
    private void directFollow()
    {
        Vector2Int[] moves = Directions.nearestDirection(v3toNearestV2Int(pacMan.transform.position) - v3toNearestV2Int(transform.position));

        foreach (Vector2Int move in moves)
        {
            if (manager.map.Contains(move + v3toNearestV2Int(transform.position)))
            {
                queuedDirection = nav.getDirectionIndex(move);
                return;
            }
        }
    }

    //
    private void directFollow(Vector2Int target)
    {
        Vector2Int[] moves = Directions.nearestDirection(target - v3toNearestV2Int(transform.position));

        foreach (Vector2Int move in moves)
        {
            if (manager.map.Contains(move + v3toNearestV2Int(transform.position)))
            {
                queuedDirection = nav.getDirectionIndex(move);
                return;
            }
        }
    }

    private void sendAgent()
    {

    }


    //algorithm
    private void noGridFindRoute()
    {
        ArrayList route = new();

    }

    private bool noGridFindRouteStep()
    {



        return true;
    }

    //starts the route search laterally, then vertically
    /*private void findRouteHV(NavigationGridNode target)
    {

        ArrayList route = new ArrayList();
        ArrayList deadEnds = new ArrayList();

        int horizontalDifference;
        int verticalDifference;

        Vector2Int posVector = v3toNearestV2Int(transform.position);
        NavigationGridNode pos = navGrid.findNode(posVector);

        horizontalDifference = target.position.x - pos.position.x;
        verticalDifference = target.position.y - pos.position.y;

        while(pos != target)
        {
            if(navGrid.isOnGrid(horizontalDifference < 0 ? pos.left : pos.right))
            {
                
            } 
        }

    }
    
    private void floodSearch()
    {

    }

    private void findRouteRecursive(NavigationGridNode target)
    {
        ArrayList route = new ArrayList();

        int horizontalDifference;
        int verticalDifference;

        Vector2Int posVector = v3toNearestV2Int(transform.position);
        NavigationGridNode pos = navGrid.findNode(posVector);

        horizontalDifference = target.position.x - pos.position.x;
        verticalDifference = target.position.y - pos.position.y;


    }

    private void findRouteRecursiveStep(NavigationGridNode step, NavigationGridNode target, ArrayList route,  int horizontalDifference, int verticalDifference) 
    {
        if(step == target)
        {
            route.Add(step);
        }
        if (navGrid.isOnGrid(step))
        {
            route.Add(step);

            if (horizontalDifference != 0)
            {
                findRouteRecursiveStep(horizontalDifference < 0 ? step.left : step.right, target, route, horizontalDifference + horizontalDifference < 0 ? -1 : 1, verticalDifference);
            }
            else
            {
                findRouteRecursiveStep(verticalDifference < 0 ? step.down : step.up, target, route, horizontalDifference, verticalDifference + verticalDifference < 0 ? -1 : 1);
            }
        }
    }*/

    private class PathFinder
    {
        ArrayList pathOut;
        ArrayList pathIn;
        Vector2Int target;
        Vector2Int start;
        Vector2Int position;
        MapManagerScript manager;
        
        public PathFinder(Vector2Int vec, Vector2Int pos, MapManagerScript script)
        {
            target = vec;
            start = pos;
            position = pos;
            manager = script;
            pathOut = new();
            pathIn = new();
        }

        public ArrayList getPath()
        {
            return pathOut;
        }

        public void findPath()
        {

            Vector2Int[] moves = Directions.nearestDirection(target - position);


        }

        //this actually moves the agent
        public bool tryMove(Vector2Int[] moves)
        {
            foreach (Vector2Int move in moves)
            {
                if (manager.map.Contains(move))
                {
                    position += move;
                    pathOut.Add(position);
                    return true;
                }
            }

            return false;
        }
    }

}
