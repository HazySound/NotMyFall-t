using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightColorRGB : MonoBehaviour
{
    private Light rgblight;
    private float lerpTime = 0.7f;
    private Color[] mycolor = new Color[] { Color.red, Color.green, Color.blue };
    
    private int colorIndex = 0;
    private float timer = 0f;

    void Start()
    {
        rgblight = transform.GetComponent<Light>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rgblight.color = Color.Lerp(rgblight.color, mycolor[colorIndex], lerpTime * Time.deltaTime);
        timer = Mathf.Lerp(timer, 1f, lerpTime * Time.deltaTime);
		if (timer > .9f)
		{
            timer = 0f;
            colorIndex++;
            colorIndex = (colorIndex >= mycolor.Length) ? 0 : colorIndex;
		}
    }
}
