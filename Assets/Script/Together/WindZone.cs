using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZone : MonoBehaviour
{
	public bool NoWindZone;
	public Vector3 Direction;
	public float PowerLevel = 1f;
	private WindManager windArea;
	private Transform hips;
	public Animator pinwheel;
	private void Start()
	{
		windArea = GetComponentInParent<WindManager>();
		if(pinwheel != null)
		{
			pinwheel.speed = PowerLevel;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject == GameManager.Instance.hips)
		{
			if (NoWindZone)
			{
				windArea.WindIsOn = false;
				windArea.powerMultiplier = 0;
				windArea.direction = Vector3.zero;
			}
			else
			{
				windArea.WindIsOn = true;
				windArea.powerMultiplier = PowerLevel;
				windArea.direction = Direction;
			}
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == GameManager.Instance.hips)
		{
			windArea.WindIsOn = false;
		}
	}
}
