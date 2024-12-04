using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;


/*
 * Possible Ghost Ideas:
 * 
 * - knower : moves slow but can always see you
 * - sensor : moves fast but can only see you in certain areas
 *    - hunter : LoS
 *    - lurker : Web / Slime trail
 * - line dash : fast in a straight line but has a turn cooldown cooldown
 * - twins : try to get on either side of you
 *    - move faster when close?
 * - spectre : moves through walls
 *    - can't see through paths?
 * - hider : invisible to you
 *    - can see if LoS
 *    - y/n knows where you are?
 * - scouter : sends out scouts to find you, then becomes a knower
 *    - y/n moves while scouting?
 *    - way to lose scouts?
 * 
 * 
 * 
 */

public class GhostScript : MonoBehaviour
{

    public float movespeed;
    //[SerializeField]
    //private float turnChance = 0.5f;
    [SerializeField]
    protected bool following = true;

    protected MapManagerScript manager;
    protected NavigationScript nav;
    protected GameObject pacMan;

    protected int queuedDirection;
    protected int direction;
    protected Vector3 previousPos;
    public Queue<Vector2Int> turnQueue;

    [SerializeField]
    public float timerCap = 1;
    protected float timer = 0;



    // Start is called before the first frame update
    void Start()
    {
        //newMap();
        manager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManagerScript>();
        nav = GameObject.FindGameObjectWithTag("NavigationSystem").GetComponent<NavigationScript>();
        pacMan = GameObject.FindGameObjectWithTag("Player");

        turnQueue = new Queue<Vector2Int>();
        turnQueue.Enqueue(v3toNearestV2Int(transform.position));
        previousPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        

        previousPos = transform.position;

        
        timer += Time.deltaTime;
        if (timer >= timerCap)
        {
            findRoute();
            timer = 0;
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
            else
            {
                following = true;
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

    protected Vector2Int v3toNearestV2Int(Vector3 vec)
    {
        var retVec = new Vector2Int((int)(vec.x + (vec.x >= 0 ? 0.5 : -0.5)), (int)(vec.y + (vec.y >= 0 ? 0.5 : -0.5)));

        return retVec;
    }

    protected Vector3 v2IntToV3(Vector2Int vec)
    {
        return new Vector3(vec.x, vec.y);
    }

    /* MARK - Pathfinding System ***************************************************** */

    //goes straight toward the player, gets stuck on walls
    protected void directFollow()
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
    protected void directFollow(Vector2Int target)
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

    //minor bug: gets stuck on double shell. should be fine though. Small issue that resolves itself when the player moves
    //
    [ContextMenu("build route")]
    protected void findRoute()
    {
        Queue<Vector2Int> tempQueue = new();
        Vector2Int pos = v3toNearestV2Int(transform.position);
        Vector2Int target = v3toNearestV2Int(pacMan.transform.position);
        Vector2Int[] moves;
        tempQueue.Enqueue(pos);

        while (pos != target)
        {
            moves = Directions.nearestDirection(target - pos);

            int c = tempQueue.Count;

            foreach (Vector2Int move in moves)
            {
                if (manager.map.Contains(pos + move) && !tempQueue.Contains(pos + move))
                {
                    pos += move;
                    tempQueue.Enqueue(pos);
                    break;
                }
            }

            if(tempQueue.Count == c) { break; }
        }

        if (turnQueue.Count == 0 || (tempQueue.Count() < turnQueue.Count() && tempQueue.Peek() != turnQueue.Peek()))
        {
            turnQueue = tempQueue;
        }
        following = false;
    }

    public virtual void Reset()
    {
    }
}
