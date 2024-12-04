using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashGhostScript : GhostScript
{
    protected bool moving = false;

    // Update is called once per frame
    void Update()
    {
        previousPos = transform.position;

        if (!moving ) {
            timer += Time.deltaTime;
            if (timer >= timerCap)
            {
                dash();
                timer = 0;
            } 
        }


        if (nav.isValidPath(v3toNearestV2Int(transform.position + nav.directions[direction] / 2)) && moving)
        {
            transform.position += nav.directions[direction] * movespeed * Time.deltaTime;
        }
        else
        {
            transform.position = v2IntToV3(v3toNearestV2Int(transform.position));
            moving = false;
        }
        /*
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
        */

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

    private void dash()
    {
        directFollow();
        moving = true;
    }


}
