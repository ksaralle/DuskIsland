using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCamera : MonoBehaviour
{
    public float fullViewTimer;
    public float closeUpTimer;
    public float rotateTimer;

    public float elapsedTime;
    public float speed = 500;

    public Vector3 fullViewStartingPos;
    public Quaternion fullViewStartingRot;

    public Vector3 closeUpPos;
    public Quaternion closeUpRot;

    public Vector3 POVpos;
    public Quaternion POVrot;

    public Vector3 POVfinalPos;
    public Quaternion POVfinalRot;
    

    public bool fullViewState;
    public bool closeUpState;
    public bool rotateState;

    public Vector3 SwordPos;
    // rotate
    // pos: -37, 30, -51
    // rot: 45, 0, 0

    // player:
    // pos: -30.6, 25.1, -36
 

    // Update is called once per frame
    void Awake()
    {
        fullViewState = true;
        closeUpState = false;
        rotateState = false;

        SwordPos = new Vector3(-30.6f, 25.1f, -36);

        fullViewTimer = 2.5f;
        closeUpTimer = 2.5f;
        rotateTimer = 5;
        elapsedTime = 0;
        // full view
        fullViewStartingPos = new Vector3(30, 70, 30);
        fullViewStartingRot.eulerAngles = new Vector3(90, 0, 0);

        // close up shot
        closeUpPos = new Vector3(-60, 13, 5);
        closeUpRot.eulerAngles = new Vector3(30, 0, 0);

        // rotate view
        POVpos = new Vector3(-37, 40, -60);
        POVrot.eulerAngles = new Vector3(45, 0, 0);

        POVfinalPos = new Vector3(-32.1f, 27.5f, -38);
        POVfinalRot.eulerAngles = new Vector3(45, 45, 0);
        // POVfinal rot: 45, 50, 0
        

        transform.position = fullViewStartingPos;
        transform.rotation = fullViewStartingRot;

        

    }
    void Update()
    {
        
        Vector3 pos = transform.position;

        if (fullViewState)
        {
            if (elapsedTime < fullViewTimer)
            {
                elapsedTime += Time.deltaTime;
                pos.x = pos.x - speed * Time.deltaTime * 2.2f;
                pos.z = pos.z - speed * Time.deltaTime * 2.2f;
                transform.position = pos;
                return;
            } else
            {
                // state transition

                elapsedTime = 0;
                fullViewState = false;
                closeUpState = true;

                // new state
                transform.position = closeUpPos;
                transform.rotation = closeUpRot;

            }
        } else if (closeUpState)
        {
            if (elapsedTime <= closeUpTimer)
            {
                elapsedTime += Time.deltaTime;
                pos.x = pos.x + speed * Time.deltaTime * 1.1f;
                transform.position = pos;
                return;
            } else
            {
                // state transition
                elapsedTime = 0;
                closeUpState = false;
                rotateState = true;

                // new state
                transform.position = POVpos;
                transform.rotation = POVrot;
            }
        } else if (rotateState)
        {
            if (elapsedTime <= rotateTimer)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, POVfinalPos, 0.4f * elapsedTime / rotateTimer);
                transform.rotation = Quaternion.Slerp(transform.rotation, POVfinalRot, 0.1f * elapsedTime / rotateTimer);
            } else
            {
                // freeze








            }
        }
        







       
    }



    void resetTime()
    {
        elapsedTime = 0;
    }
}
