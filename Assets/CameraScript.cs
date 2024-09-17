using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private GameObject pacMan;

    private Vector3 depthMod = new Vector3(0, 0, -10);
    // Start is called before the first frame update
    void Start()
    {
        center();
    }

    // Update is called once per frame
    void Update()
    {
        center();
    }

    void center()
    {
        transform.position = pacMan.transform.position + depthMod;
    }

}
