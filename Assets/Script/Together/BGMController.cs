using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    private AudioSource source;
    [SerializeField]
    private VoiceManager.Clip map;
    private RagdollController ragdoll;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        ragdoll = GameManager.Instance.ragdollController;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6) //layer = player
        {
            if (!source.isPlaying) { source.Play(); }
            VoiceManager.Instance.currentMap = map;
            if (ragdoll.CanControl) { VoiceManager.Instance.hitMap = map; }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 6) //layer = player
        {
            if (source.isPlaying) { source.Stop(); }
        }
    }
}
