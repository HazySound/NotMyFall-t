using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trapdoor : MonoBehaviour
{
	private Animator anim;
	private void Start()
	{
		anim = transform.GetComponent<Animator>();
	}
	private void OnTriggerEnter(Collider other)
	{
		if (GameManager.Instance.ragdollController.CanControl)
		{
		anim.SetBool("Enter", true);
		}
	}
}
