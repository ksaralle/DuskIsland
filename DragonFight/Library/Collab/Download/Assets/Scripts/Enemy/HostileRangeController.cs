using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostileRangeController : MonoBehaviour
{
    #region Private Variables
   
    // the player
    private GameObject PlayerObject;

    #endregion

    #region Initialization
    void Awake()
    {
        
        // find the player object
        PlayerObject = GameObject.Find("Player");
    }
    #endregion

    #region About the Attack
    // within range
    void OnTriggerEnter(Collider other)
    {
       
        // if it is the player
        if (other.gameObject == PlayerObject)
        {
            Debug.Log("player enters the hostile range.");
            // letting enemycontroller know that player is in sight

            // the origin -> dragon's position
            Transform dragonTransform = GetComponentInParent<EnemyController>().GetDragonTransform();
            Vector3 targetDir = PlayerObject.transform.position - dragonTransform.position;
            // calculate the angle between "dragon-player" vector and "dragon's forward direction" vector
            float angle = Vector3.Angle(targetDir, dragonTransform.forward);
            if (angle < 90f)
                // if the player is in the front hemisphere 
            {
                if (!GetComponentInParent<EnemyController>().GetHostilityStatus())
                {
                    GetComponentInParent<EnemyController>().ChangeHostilityStatus(true);
                }
                
            }



            
        }
    }

    // player runs away
    void OnTriggerExit(Collider other)
    {
        
        // if it is the player
        if (other.gameObject == PlayerObject)
        {
            Debug.Log("player leaves the hostile range.");
            // letting enemycontroller know that player is out of sight
            if (GetComponentInParent<EnemyController>().GetHostilityStatus())
            {
                GetComponentInParent<EnemyController>().ChangeHostilityStatus(false);
            }
        }
    }
    #endregion
   
}
