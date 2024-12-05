 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PacManScript : MonoBehaviour
{
    public float movespeed = 5;
    public float offsetAllowance = 0.5f;
    public MapManagerScript manager;
    public NavigationScript nav;
    public GameManagerScript gameManager;
    public bool immune = false;

    int queuedDirection;
    int direction;
    Vector3 previousPos;
    int[] rotations = { 0, 90, 180, 270 };

    [SerializeField]
    private float dashCooldownTCap = 0.5f;
    private float dashCooldown = 0;

    // Start is called before the first frame update
    void Start()
    {
        previousPos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {

        previousPos = transform.position;

        if (nav.isValidPath(v3toNearestV2Int(transform.position + 100*nav.directions[direction]/199)))
        {
            transform.position += nav.directions[direction] * movespeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, rotations[direction]);
        }
        else
        {
            transform.position = v2IntToV3(v3toNearestV2Int(transform.position));
        }

        if (gameManager.gameActive)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) { queuedDirection = 0; }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) { queuedDirection = 1; }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) { queuedDirection = 2; }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) { queuedDirection = 3; }
            if (Input.GetKeyDown(KeyCode.Space) && dashCooldown == 0) { Dash(); }
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

        //update timers
        if (dashCooldown > 0)
        {
            dashCooldown -= Time.deltaTime;
        }
        else
        {
            dashCooldown = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ghost" && !immune)
        {
            Debug.Log("Collided");
            gameManager.GameOver();
        }
    }

    Vector2Int v3toNearestV2Int(Vector3 vec)
    {
        var retVec = new Vector2Int((int)(vec.x + (vec.x >= 0 ? 0.5 : -0.5)), (int)(vec.y + (vec.y >= 0 ? 0.5 : -0.5)));

        return retVec;
    }

    Vector3 v2IntToV3(Vector2Int vec)
    {
        return new Vector3(vec.x, vec.y);
    }
    public void Reset()
    {
        movespeed = 4;
    }

    public void Dash()
    {
        immune = true;

        StartCoroutine(DashTimer(0.1f));

        dashCooldown = dashCooldownTCap;
        movespeed = 20;
    }

    public IEnumerator DashTimer(float time)
    {
        yield return new WaitForSeconds(time);
        movespeed = 4;
        immune = false;
    }
}
