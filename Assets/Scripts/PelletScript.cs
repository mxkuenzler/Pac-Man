using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PelletScript : MonoBehaviour
{
    private MapManagerScript m_Script;

    private void Start()
    {
        m_Script = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManagerScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_Script.removePellet(gameObject);
            Destroy(gameObject);
        }
    }
}
