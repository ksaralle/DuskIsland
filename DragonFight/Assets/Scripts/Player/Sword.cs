using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{

    [SerializeField]
    [Tooltip("the playeyr")]
    private Transform m_Player;

    private void Start()
    {
        transform.parent = m_Player.transform;
    }
}
