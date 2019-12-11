using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostileRangeController : MonoBehaviour
{
    #region Private Variables
   
    
    private GameObject PlayerObject;


    private bool inRange;

    private Transform dragonTransform;

    #endregion

    #region Initialization
    void Awake()
    {
        
        
        PlayerObject = GameObject.Find("Player");
        inRange = false;
        dragonTransform = GetComponentInParent<EnemyController>().GetDragonTransform();
    }
    #endregion

    private void Update()
    {

       if (inRange)
        {
            // the origin -> dragon's position
            Vector3 targetDir = PlayerObject.transform.position - dragonTransform.position;

            // calculate the angle between "dragon-player" vector and "dragon's forward direction" vector
            float angle = Vector3.Angle(targetDir, dragonTransform.forward);
            if (angle < 160f)
            // if the player is in the front hemisphere 
            {
                if (!GetComponentInParent<EnemyController>().GetHostilityStatus())
                {
                    GetComponentInParent<EnemyController>().ChangeHostilityStatus(true);
                }

            }
        } else
        {
            GetComponentInParent<EnemyController>().ChangeHostilityStatus(false);
        }

        

    }

    #region About the Attack
    // within range
    void OnTriggerEnter(Collider other)
    {
       
        // if it is the player
        if (other.gameObject == PlayerObject)
        {


            inRange = true;

            
        }
    }

    // player runs away
    void OnTriggerExit(Collider other)
    {
        
        // if it is the player
        if (other.gameObject == PlayerObject)
        {
            inRange = false;
        }
    }
    #endregion
   
}
