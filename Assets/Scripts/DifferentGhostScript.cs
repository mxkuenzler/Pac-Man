using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

//scenthound
public class DifferentGhostScript : GhostScript
{
    [SerializeField]
    bool tracking = false;

    [SerializeField]
    private int maxQueueSize = 20;


    [SerializeField]
    private GameObject queueMarker;
    private List<Vector2Int> queueMarkers = new List<Vector2Int>();

    private Vector2Int backOfQueue;

    private void Update()
    {
        previousPos = transform.position;

        if (nav.isValidPath(v3toNearestV2Int(transform.position + nav.directions[direction] / 2)))
        {
            transform.position += nav.directions[direction] * movespeed * Time.deltaTime;
        }

        buildScentTrail();
        //showQueue();

        if (tracking)
        {
            if (turnQueue.Count > 0)
            {
                var here = v3toNearestV2Int(transform.position);
                if (turnQueue.Contains(here))
                {
                    turnQueue.Dequeue();
                    if (turnQueue.Contains(here))
                    {
                        while (turnQueue.Contains(here)) { turnQueue.Dequeue(); }
                    }
                }
                if((here - turnQueue.Peek()).magnitude > 1.2)
                {
                    tracking = false;
                }
                else
                {
                    directFollow(turnQueue.Peek());
                }
            }
        }
        else
        {
            if (turnQueue.Contains(v3toNearestV2Int(transform.position)))
            {
                tracking = true;
            }
            else
            {
                wander();
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

    private void buildScentTrail()
    {
        Vector2Int pacPos = v3toNearestV2Int(pacMan.transform.position);
        if (pacPos != backOfQueue)
        {
            turnQueue.Enqueue(pacPos);
            backOfQueue = pacPos;
        }
    }

    public override void Reset()
    {
        turnQueue.Clear();
    }

    public void showQueue()
    {
        foreach(Vector2Int q in turnQueue)
        {
            if (!queueMarkers.Contains(q)) {
                queueMarkers.Add(q);
                Instantiate(queueMarker, v2IntToV3(q), Quaternion.identity);
            }
        }
    }
}
