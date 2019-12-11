using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffectsController : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 StartingPosition;

    private GameObject Camera;
    private Vector3 PosOffset;


    // fire
    // pos: 0, 10, -100
    // rot: 0, 50, 0

    // camera
    // pos: 0, 1.6, 0
    // rot: 0, 90, 0


    void Awake()
    {
        Camera = GameObject.Find("Main Camera");
        PosOffset = new Vector3(0, 1.6f-10, 100);
        

    }


    void Start()
    {

        StartingPosition = Camera.transform.position + PosOffset;



    }

    // Update is called once per frame
    void Update()
    {
        transform.position = StartingPosition;

        
    }
}
