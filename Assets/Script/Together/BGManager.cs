using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BGManager : MonoBehaviour
{
   //스카이박스
   [SerializeField]
   private Material citySB;
	[SerializeField]
	private Material mountianSB;
	[SerializeField]
	private Material spaceSB;
	[SerializeField]
	private GameObject mountain;
	public int height;
	public Transform FirstCloud;
	public Transform SecondCloud;

	public void SetSkybox(Material newOne)
	{
		if(RenderSettings.skybox != newOne)
		{
			RenderSettings.skybox = newOne;
		}
	}
	private void FixedUpdate()
	{
		height = (int)GameManager.Instance.hips.transform.position.y;
		if (height < FirstCloud.position.y)
		{
			SetSkybox(citySB);
			mountain.SetActive(false);
		}
		if (height >= FirstCloud.position.y && height < SecondCloud.position.y)
		{
			SetSkybox(mountianSB);
			mountain.SetActive(true);
		}
		if(height >= SecondCloud.position.y)
		{
			SetSkybox(spaceSB);
			mountain.SetActive(false);
		}

	}
}
