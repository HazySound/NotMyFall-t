using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MonoBehaviour
{
	public Collider OUT;
	private void FixedUpdate()
	{
		if (GameManager.Instance.ragdollController.CanControl == false) // FAlling 도중이면
		{
			OUT.isTrigger = true;
		}
		else
		{
			OUT.isTrigger = false;
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		//
		if(GameManager.Instance.ragdollController.CanControl == false)
		{
			OUT.isTrigger = true;
			//StartCoroutine("ColliderOn");
		}
		//
	}
	IEnumerator ColliderOn()
	{
		yield return new WaitForSeconds(5f);
		OUT.isTrigger = false;
	}
}
