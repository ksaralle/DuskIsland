using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  

public class Lava : MonoBehaviour
{

    //[SerializeField]
    //[Tooltip("the speed at which the lava rises")]
    //private int Speed;

    [SerializeField]
    [Tooltip("the number of seconds before it rises again")]
    private float TimeToNextRise;

    [SerializeField]
    [Tooltip("how much the lava will rise")]
    private float Height;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Rise());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Rise()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(TimeToNextRise);
            transform.DOMoveY(transform.position.y + Height, 2, false);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")

        {
            collision.gameObject.GetComponent<PlayerController>().setLava();
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().fsetLava();
        }
    }
}
