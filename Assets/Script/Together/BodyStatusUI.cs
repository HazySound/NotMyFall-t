using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyStatusUI : MonoBehaviour
{
    [SerializeField] Color enableColor;
    [SerializeField] Color disableColor;
	private Color origincolor;
	public Image Left;
	public Image Right;
	public Image Leg_Left;
	public Image Leg_right;
	public Image Head;
	private void Start()
	{
		origincolor = transform.GetComponent<Image>().color;
	}
	public void disablebody(Image bodyparts)
	{
		bodyparts.color = disableColor;
		bodyparts.GetComponent<BodyPartsUI>().isgreen = false;
	}
	public void enablebody(Image bodyparts)
	{
		bodyparts.color = enableColor;
		bodyparts.GetComponent<BodyPartsUI>().isgreen = true;
	}
	public void DebugButton()
	{
		disablebody(Right);
	}
	private void FixedUpdate()
	{
		//can cotroll이 false면 전부 disable
		if (GameManager.Instance.ragdollController.CanControl)
		{
			enablebody(Left);
			enablebody(Right);
			enablebody(Leg_Left);
			enablebody(Leg_right);
			enablebody(Head);
		}
		else
		{	
			disablebody(Left);
			disablebody(Right);
			disablebody(Leg_Left);
			disablebody(Leg_right);
			disablebody(Head);
		}
	}
}
