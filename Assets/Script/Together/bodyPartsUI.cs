using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyPartsUI : MonoBehaviour
{
    public bool isgreen;
    public GameObject boosterEffect;
	public Image image;
	private void FixedUpdate()
	{
		if (isgreen)
		{
			if (boosterEffect != null && boosterEffect.activeSelf)
			{
				image.enabled = true;
			}
			else
			{
				image.enabled = false;
			}
		}
		else
		{
			image.enabled = true; // 부스터 안눌러도 빨간상태 유지
		}
	}
}
