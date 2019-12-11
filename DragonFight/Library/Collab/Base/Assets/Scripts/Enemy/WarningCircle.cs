using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningCircle : MonoBehaviour


{
    public SpriteRenderer circleSprite;
    public float transition = 20.0f;
    public float elapsedtime = 0.0f;
    private Color startingColor;
    // Start is called before the first frame update
    void Start()
    {
        transition = 20.0f;
        circleSprite = GetComponent<SpriteRenderer>();
        startingColor = circleSprite.color;

    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(warningCircle());
    }

    IEnumerator warningCircle()
    {
        while (circleSprite.color != Color.red)
        {
            circleSprite.color = Color.Lerp(circleSprite.color, Color.red, elapsedtime / transition);
            elapsedtime += Time.deltaTime;

            yield return null;
        }
        elapsedtime = 0.0f;
        gameObject.SetActive(false);
        circleSprite.color = startingColor;
    }
}
