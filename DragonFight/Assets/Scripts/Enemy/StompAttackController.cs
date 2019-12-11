using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompAttackController : MonoBehaviour
{
    [SerializeField]
    

    public bool colliding;

    #region Private Variables

    // the player
    private GameObject PlayerObject;
    private Transform StompWarning;

    #endregion

    #region Initialization
    void Awake()
    {

        // find the player object

        colliding = false;
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
            colliding = true;
            
            // letting enemycontroller know that player is within stomp attack range
            GetComponentInParent<EnemyController>().ChangeStompAttackRangeStatus(true);
            GetComponentInParent<EnemyController>().ChangeHostilityStatus(true);
        }
    }

    // player runs away
    void OnTriggerExit(Collider other)
    {
        // if it is the player
        if (other.gameObject == PlayerObject)
        {
            colliding = false;
            // letting enemycontroller know that player has left stomp attack range
            GetComponentInParent<EnemyController>().ChangeStompAttackRangeStatus(false);
        }
    }
    #endregion

    #region Stomp Warning
    public void showWarning()
    {
        
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(0).GetComponent<WarningCircle>().reset();
        //Instantiate(StompWarning);
        //Debug.Log("getchild(0)" + gameObject.transform.GetChild(0));
    }
    public void hideWarning()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        //gameObject.transform.GetChild(0).GetComponent<WarningCircle>().resetColor();
    }
    #endregion
}
