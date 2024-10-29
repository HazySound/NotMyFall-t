using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleCollider : MonoBehaviour
{
    [SerializeField]
    private EagleContainer eagleContainer;
    private AudioSource source;
    private WaitForSecondsRealtime waitSec;
    private RagdollController ragdoll;
    private void OnTriggerEnter(Collider other)
    {
        if (ragdoll.CanControl && !eagleContainer.isTriggered && !source.isPlaying)
        {
            StartCoroutine(nameof(PlaySound));
        }
        else
        {
            if (!source.isPlaying) { source.Play(); }
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        waitSec = new WaitForSecondsRealtime(source.clip.length);
        ragdoll = GameManager.Instance.ragdollController;
    }

    private IEnumerator PlaySound()
    {
        source.Play();
        yield return waitSec;
        eagleContainer.isTriggered = true;
    }
}
