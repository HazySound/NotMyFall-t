using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WindManager : MonoBehaviour
{
	public bool WindIsOn;
	public float power; // 초기값
	public float powerMultiplier; // 레베루
	public Vector3 direction; //x값 -1은 ->방향, 1은 <-방향
	private Rigidbody hips;
	private void Start()
	{
		hips = GameManager.Instance.hips.GetComponent<Rigidbody>();
	}
	private void FixedUpdate()
	{
		if (WindIsOn)
		{
			if (hips != null)
			{
				hips.AddForce(direction * power* powerMultiplier * Time.fixedDeltaTime);
			}
		}
	}
}
