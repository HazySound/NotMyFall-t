using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleContainer : MonoBehaviour
{
    private Transform target;
    public float speed;
    [HideInInspector]
    public bool isTriggered = false;
    private bool audioPlayed = false;
    [HideInInspector]
    public bool isReturning = false;
    private Vector3 originPos;
    private AudioSource audio;
    private Transform origin;
    private WaitForSecondsRealtime waitAsec = new WaitForSecondsRealtime(2f);
    // Start is called before the first frame update
    void Start()
    {
        target = GameManager.Instance.eagleTarget;
        originPos = transform.localPosition;
        audio = GetComponent<AudioSource>();
        origin = transform.parent.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered)
        {
            if (!audioPlayed)
            {
                audio.Play();
                audioPlayed = true;
            }
            GoEagle();
        }
    }

    private void GoEagle()
    {
        //StopAllCoroutines();
        transform.LookAt(target);
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.Self);
    }

    public void BackToOrigin()
    {
        StartCoroutine(nameof(GoBack));
    }

    IEnumerator GoBack()
    {
        isReturning = true;
        yield return waitAsec;
        isTriggered = false;

        while (Vector3.Distance(transform.localPosition, originPos) > 1f)
        {
            transform.LookAt(origin);
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.Self);
            yield return null;
        }
        transform.localPosition = originPos;
        audioPlayed = false;
        isReturning = false;
    }
}
