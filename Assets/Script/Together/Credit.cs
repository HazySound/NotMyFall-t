using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Credit : MonoBehaviour
{
	public void CreditEnd()
	{
		StartCoroutine(nameof(BackToLobby));
	}

	private IEnumerator BackToLobby()
    {
		VoiceManager.Instance.EndCreditMusic(); //Å©·¹µ÷ À½¾Ç ÆäÀÌµå¾Æ¿ô
		yield return new WaitForSecondsRealtime(1f);
		//·Îºñ ¾ÀÀ¸·Î µ¹¾Æ°¡
		PhotonNetwork.LeaveRoom();
	}

	public void CreditStart()
    {
		StartCoroutine(VoiceManager.Instance.StartCreditMusic());
    }
}
