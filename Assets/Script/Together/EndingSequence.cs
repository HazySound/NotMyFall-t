using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingSequence : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		//HUD 꺼주고 무한 부스터 ON
		UIManager.Instance.boosterUI.gameObject.SetActive(false);
		UIManager.Instance.BodyUI.SetActive(false);
		UIManager.Instance.HUD.SetActive(false);

		GameManager.Instance.jetCon.unlimitedBooster(); //무한 부스터
		GameManager.Instance.ragdollController.noGravity();
	}
	private void OnTriggerExit(Collider other)
	{
		if(GameManager.Instance.hips.transform.position.y < this.transform.position.y)
		{
			//HUD 꺼주고 무한 부스터 ON
			UIManager.Instance.boosterUI.gameObject.SetActive(true);
			UIManager.Instance.BodyUI.SetActive(true);
			UIManager.Instance.HUD.SetActive(true);

			GameManager.Instance.jetCon.debug = false; //무한 부스터
		}

	}
}
