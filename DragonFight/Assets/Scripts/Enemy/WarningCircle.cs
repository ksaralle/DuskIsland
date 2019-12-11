using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningCircle : MonoBehaviour


{
    public SpriteRenderer circleSprite;

    public float elapsedtime;

    public float greenTimer;

    private Color startingColor;
    private Color endColor;

    private GameObject dragon;



    // Start is called before the first frame update
    void Awake()
    {

        elapsedtime = 0;

        startingColor = Color.green;
        endColor = Color.red;

        circleSprite = GetComponent<SpriteRenderer>();

        circleSprite.color = startingColor;

        dragon = GameObject.Find("Dragon");

    }

    void Start()
    {
        greenTimer = dragon.GetComponent<EnemyController>().getStompAttackWindupTime();

    }

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine(warningCircle());
        if (elapsedtime <= greenTimer)
        {
            elapsedtime += Time.deltaTime;
        }
        else
        {
            circleSprite.color = endColor;
        }
    }


    public void reset()
    {
        circleSprite.color = Color.green;
        elapsedtime = 0;

    }

}
