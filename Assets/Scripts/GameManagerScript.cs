using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    private GameObject pacMan;
    public bool gameActive = false;

    [SerializeField]
    private MapManagerScript mapManager;
    [SerializeField]
    private GameObject[] ghosts = new GameObject[3];
    private List<GameObject> activeGhosts = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        pacMan = GameObject.FindGameObjectWithTag("Player");
        StartGame();
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
    }

    public void StartGame()
    {
        mapManager.generateMap();

        gameActive = true;
        foreach (GameObject g in activeGhosts)
        {
            Destroy(g);
        }
        activeGhosts.Clear();

        pacMan.gameObject.GetComponent<PacManScript>().Reset();

        //pacMan.transform.position = Vector3.zero;
        pacMan.transform.position = Vector3.zero;

        populateGhosts();
    }

    public void populateGhosts()
    {
        foreach (GameObject g in ghosts)
        {
            var q = mapManager.map.ElementAt(Random.Range(0, mapManager.map.Count));
            activeGhosts.Add(Instantiate(g, VectorConverter.v2IntToV3(q), Quaternion.identity));
            try { g.GetComponent<GhostScript>().Reset(); }
            catch { }
        }
    }
}
