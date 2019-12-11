using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("all of the main info about this particular ability")]
    protected AbilityInfo m_Info;
    #endregion

    #region Cached Components
    protected Rigidbody rb_Sword;
    public AudioSource SoundEffects;
    #endregion

    #region Initialization
    private void Awake()
    {
        // rb_Sword = GetComponent<>();
        SoundEffects = GetComponent<AudioSource>();
    }
    #endregion

    #region Use Methods
    public abstract void Use(Vector3 spawnPos);
    #endregion
}
