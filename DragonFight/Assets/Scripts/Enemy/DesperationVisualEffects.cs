using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesperationVisualEffects : MonoBehaviour
{

    [SerializeField]
    [Tooltip("the dragon")]
    private Transform m_Dragon;

    void Awake()
    {
        transform.position = m_Dragon.transform.position;
    }
    void Start()
    {
        
    }
    

    // Update is called once per frame
    void Update()
    {
        transform.position = m_Dragon.transform.position;
    }
}
