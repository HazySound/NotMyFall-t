using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollMaster : MonoBehaviour
{
    public GameObject Player_Ragdoll;
    public GameObject Player_anim;

	public void animationSync()
	{

	}
	public void SwitchToanim()
	{
		Player_Ragdoll.gameObject.SetActive(false);
		Player_anim.gameObject.SetActive(true);
	}
	public void SwitchToRagdoll()
	{
		Player_Ragdoll.gameObject.SetActive(true);
		Player_anim.gameObject.SetActive(false);
	}
	private void Start()
	{
		
	}
}
