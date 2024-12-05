using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    private GameObject pacMan;
    public bool gameActive = false;

    [SerializeField]
    private GameObject[] ghosts = new GameObject[3];
    private List<GameObject> activeGhosts = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game Manager Start");
        pacMan = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(pacMan);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameOver()
    {
        gameActive = false;
        pacMan.GetComponent<PacManScript>().movespeed = 0;
        foreach (GameObject g in activeGhosts)
        {
            g.GetComponent<GhostScript>().movespeed = 0;
        }
        Debug.Log("Game Over");
    }

    public IEnumerator StartGame()
    {
        gameActive = true;
        foreach (GameObject g in activeGhosts)
        {
            Destroy(g);
        }
        activeGhosts.Clear();

        pacMan.gameObject.GetComponent<PacManScript>().Reset();

        //pacMan.transform.position = Vector3.zero;
        pacMan.transform.position = Vector3.zero;

        yield return new WaitForSeconds(1);

        foreach (GameObject g in ghosts)
        {
            activeGhosts.Add(Instantiate(g, Vector3.zero, Quaternion.identity));
            try { g.GetComponent<GhostScript>().Reset(); }
            catch { }
        }
    }
}
