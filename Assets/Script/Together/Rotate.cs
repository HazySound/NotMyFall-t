using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
	float timer;
	private void FixedUpdate()
	{
		timer += Time.deltaTime *50;
		transform.localEulerAngles = new Vector3(0, timer, 0);
	}
}
