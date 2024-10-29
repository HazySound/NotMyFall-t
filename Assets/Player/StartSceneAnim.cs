using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneAnim : MonoBehaviour
{
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetInteger("Dance",Random.Range(0,3));
    }
    public void StartbuttonClick()
	{
        anim.SetTrigger("Start");
	}
}
