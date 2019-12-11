using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instruction : MonoBehaviour
{
    // Start is called before the first frame update
    

    private bool isEnabled;
    public Canvas canva;

    void Start()
    {
        canva.enabled = isEnabled;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isEnabled = !isEnabled;
            canva.enabled = isEnabled;
        }


    }
}
