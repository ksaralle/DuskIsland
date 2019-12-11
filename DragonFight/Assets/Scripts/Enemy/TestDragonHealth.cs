using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDragonHealth : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("how much health this enemy has")]
    private int m_MaxHealth;

    #endregion

    #region Private Variables
    private float p_curHealth;


    #endregion

    #region Cached Components
    private Rigidbody cc_Rb;

    #endregion

    #region Cached References
    private Transform cr_Player;

    #endregion

    #region Initialization

    private void Awake()
    {
        p_curHealth = m_MaxHealth;

        cc_Rb = GetComponent<Rigidbody>();

    }
    private void Start()
    {
        cr_Player = FindObjectOfType<PlayerController>().transform;

    }
    #endregion

    private void Update()
    {
        // Debug.Log(p_curHealth);

    }


    #region Health Methods
    public void DecreaseHealth(float amount)
    {
        p_curHealth -= amount;
       
    }

    #endregion

}
