using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesperationAttackController : MonoBehaviour
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
    // within attack range
    void OnTriggerEnter(Collider other)
    {
        // if it is the player
        if (other.gameObject == PlayerObject)
        {
            Debug.Log("player enters desperation attack range.");
            // letting enemycontroller know that player is within attack range
            GetComponentInParent<EnemyController>().ChangeDesperationAttackRangeStatus(true);
        }
    }

    // player runs away
    void OnTriggerExit(Collider other)
    {
        // if it is the player
        if (other.gameObject == PlayerObject)
        {
            Debug.Log("player exits desperation attack range.");
            // letting enemycontroller know that player has left attack range
            GetComponentInParent<EnemyController>().ChangeDesperationAttackRangeStatus(false);
        }
    }
    #endregion
}
