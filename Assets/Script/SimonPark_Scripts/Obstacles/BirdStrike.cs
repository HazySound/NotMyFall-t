using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdStrike : MonoBehaviour
{
	private BirdArea laser;
	[SerializeField]
	private float speed;
    [SerializeField]
    private float maxTime;
    private Vector3 originPos;
    [HideInInspector]
    public bool isDone = false;
    [HideInInspector]
    public float timer = 0f;

    private void Start()
    {
        laser = GetComponentInParent<BirdArea>();
        originPos = transform.position;
    }

    private void Update()
    {
        if (laser.isTriggered)
        {
            if(timer < maxTime)
            {
                timer += Time.deltaTime;
                transform.Translate(-transform.right * speed * Time.deltaTime);
            }
            else if(!isDone)
            {
                transform.position = originPos;
                isDone = true;
                laser.IsAllBirdReturned();
            }
        }
    }
}
