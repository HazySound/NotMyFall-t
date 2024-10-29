using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    [SerializeField]
    private GameObject ForceField_prefab;//ÇÁ¸®ÆÕ
	private GameObject forcefield;
	public RagdollController temp;
	public bool isOn;
	private void Start()
	{
		temp = GameManager.Instance.ragdollController;
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (temp != null && temp == collision.transform.GetComponentInParent<RagdollController>())
		{
			if (temp.transform.tag == "Player")
			{
			
				if(isOn == false)
				{
					forcefield = GameObject.Instantiate(ForceField_prefab);
					ContactPoint contact = collision.GetContact(0);
					forcefield.transform.position = contact.point;
					isOn = true;
				}
			
			}
		}
	}
	private void OnCollisionExit(Collision collision)
	{
		if (temp != null && temp == collision.transform.GetComponentInParent<RagdollController>())
		{
			if (temp.transform.tag == "Player")
			{
				isOn = false;
			}
		}
	}

}
