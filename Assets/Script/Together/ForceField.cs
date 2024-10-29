using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
	[SerializeField]
	private Animator anim;
	public void TurnOFF()
	{
		Destroy(this.gameObject);
	}
	private void OnTriggerStay(Collider other)
	{
		anim.SetBool("ON", true);
	}
	private void OnTriggerExit(Collider other)
	{
		anim.SetBool("ON", false);
	}
}
