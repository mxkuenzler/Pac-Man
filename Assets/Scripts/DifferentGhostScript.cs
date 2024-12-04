using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//scenthound
public class DifferentGhostScript : GhostScript
{
    [SerializeField]
    bool going = true;
    private Vector2Int backOfQueue;
    private void Update()
    {
        previousPos = transform.position;

        if (nav.isValidPath(v3toNearestV2Int(transform.position + nav.directions[direction] / 2)))
        {
            transform.position += nav.directions[direction] * movespeed * Time.deltaTime;
        }

        buildScentTrail();

        
        if (turnQueue.Count > 0 && going)
        {
            if (v3toNearestV2Int(transform.position) == turnQueue.Peek())
            {
                var here = turnQueue.Peek();
                turnQueue.Dequeue();
                if (turnQueue.Contains(here))
                {
                    while (turnQueue.Contains(here)) { turnQueue.Dequeue(); }
                    turnQueue.Dequeue();
                }
            }
            directFollow(turnQueue.Peek());
        }
        else
        {
            Debug.Log("no queue");
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
}
