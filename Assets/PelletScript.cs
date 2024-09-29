using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PelletScript : MonoBehaviour
{
   /* private void Start()
    {
        Debug.Log("new pellet");
    }*/

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("pelletColl");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("pelletColl");
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("pellet touched");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player touched pellet");
            Destroy(gameObject);
        }
    }
}
