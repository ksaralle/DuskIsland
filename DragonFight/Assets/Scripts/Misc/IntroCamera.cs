using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroCamera : MonoBehaviour
{
	public GameObject playerObject;

    public float fullViewTimer;
    public float closeUpTimer;
    public float rotateTimer;
    public float subStateTimer;
    public float onFireTimer;

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


    public Vector3 pos1;
    public Quaternion rot1;
    public Vector3 pos2;
    public Quaternion rot2;
    public Vector3 pos3;
    public Quaternion rot3;

    public Vector3 finalPos;
    public Quaternion finalRot;






    public bool fullViewState;
    public bool closeUpState;
    public bool rotateState;

    public bool introSetOver;

    public Vector3 SwordPos;

    public int[] States;
    public int[] subStates;
    

   
    // rotate
    // pos: -37, 30, -51
    // rot: 45, 0, 0

    // player:
    // pos: -30.6, 25.1, -36
 

    // Update is called once per frame
    void Awake()
    {
        playerObject = GameObject.Find("introplayer");

        fullViewState = true;
        closeUpState = false;
        rotateState = false;

        introSetOver = false;

        SwordPos = new Vector3(-30.6f, 27.5f, -33);

        fullViewTimer = 2.5f;
        closeUpTimer = 2.5f;
        rotateTimer = 4;
        subStateTimer = 1.6f;
        onFireTimer = 2.5f;
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

        pos1 = new Vector3(-44.3f, 30.8f, -33.3f);
        pos2 = new Vector3(-41, 30, -44);
        pos3 = new Vector3(-32, 28, -45);
        rot1.eulerAngles = new Vector3(30, 90, 0);
        rot2.eulerAngles = new Vector3(30, 60, 0);
        rot3.eulerAngles = new Vector3(30, 0, 0);

        finalPos = new Vector3(-30, 27.5f, -32);
        finalRot.eulerAngles = new Vector3(30, 180, 0);
        // pos: -30, 27, -32
        // rot: 30, 180, 0

        subStates = new int[3];
        for (int i = 0; i < subStates.Length; i++)
        {
            if (i == 0)
            {
                subStates[i] = 1;
            } else
            {
                subStates[i] = 0;
            }
            
        }
        States = new int[4];
        for (int i = 0; i < States.Length; i++)
        {
            if (i == 0)
            {
                States[i] = 1;
            }
            else
            {
                States[i] = 0;
            }

        }

       


        transform.position = fullViewStartingPos;
        transform.rotation = fullViewStartingRot;

    }
    void Update()
    {
        
        Vector3 pos = transform.position;

        if (States[0] == 1)
        {

            // state 0 - extreme wide shot
            if (elapsedTime < fullViewTimer)
            {
                elapsedTime += Time.deltaTime;
                pos.x = pos.x - speed * Time.deltaTime * 2.2f;
                pos.z = pos.z - speed * Time.deltaTime * 2.2f;
                transform.position = pos;
                return;
            }
            else
            {
                // state transition

                elapsedTime = 0;
                States[0] = 0;
                States[1] = 1;

                // new state
                transform.position = closeUpPos;
                transform.rotation = closeUpRot;

            }
        }
        else if (States[1] == 1)
        {
            // state 1 - medium close up
            if (elapsedTime <= closeUpTimer)
            {
                elapsedTime += Time.deltaTime;
                pos.x = pos.x + speed * Time.deltaTime * 1.1f;
                transform.position = pos;
                return;
            }
            else
            {
                // state transition
                elapsedTime = 0;
                States[1] = 0;
                States[2] = 1;

                // new state
                transform.position = pos1;
                transform.rotation = rot1;
            }
        }
        else if (States[2] == 1)
        // state 2 - camera rotate around player + pick up sword
        {
            if (!introSetOver)
            {
                // freeze
                playerObject.GetComponent<IntroPlayerController>().introIsOver();
                introSetOver = true;
            }


            if (subStates[0] == 1)
            {
                if (elapsedTime <= subStateTimer)
                {
                    elapsedTime += Time.deltaTime;
                    transform.position = Vector3.Lerp(transform.position, pos2, Time.deltaTime * elapsedTime / subStateTimer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot2, Time.deltaTime * elapsedTime / subStateTimer);
                }
                else
                {
                    // state transition
                    subStates[0] = 0;
                    subStates[1] = 1;
                    elapsedTime = 0;
                }

            }
            else if (subStates[1] == 1)
            {
                if (elapsedTime <= subStateTimer)
                {
                    elapsedTime += Time.deltaTime;
                    transform.position = Vector3.Lerp(transform.position, pos3, Time.deltaTime * elapsedTime / subStateTimer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot3, Time.deltaTime * elapsedTime / subStateTimer);
                }
                else
                {
                    // state transition
                    subStates[1] = 0;
                    subStates[2] = 1;
                    elapsedTime = 0;
                }
            }
            else if (subStates[2] == 1)
            {
                if (elapsedTime <= subStateTimer)
                {
                    elapsedTime += Time.deltaTime;
                    transform.position = Vector3.Lerp(transform.position, POVfinalPos, Time.deltaTime * elapsedTime / subStateTimer);
                    transform.rotation = Quaternion.Slerp(transform.rotation, POVfinalRot, Time.deltaTime * elapsedTime / subStateTimer);
                }
                else
                {
                    // do nothing
                }
            }
        }
        else if (States[3] == 1)
        {
            // state 3 - sword been picked up

            // pos: -30, 27, -32
            // rot: 30, 180, 0
            
            if (elapsedTime < onFireTimer)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, SwordPos, 0.8f * Time.deltaTime);
            } else
            {
                // do nothing
            }
            
        }
        
    }






    public void swordIsPickedUp()
    {
        Debug.Log("called once");
        States[2] = 0;
        States[3] = 1;
        elapsedTime = 0;
        
        transform.position = finalPos;
        transform.rotation = finalRot;
    }


    
}
