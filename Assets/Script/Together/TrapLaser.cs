using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapLaser : MonoBehaviour
{
    public Animator Trapanim;
	private void OnTriggerEnter(Collider other)
	{
		StartCoroutine(LaserHit());
	}
	IEnumerator LaserHit()
	{
		yield return new WaitForSecondsRealtime(5f);
		Trapanim.SetBool("Enter", false);
	}
}
