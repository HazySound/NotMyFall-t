using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RagdollController : MonoBehaviourPunCallbacks
{
	//public float tempTimer = 0f;

	public enum PlayerState
	{
		Onground,
		Flying,
		Falling,
	}

	[SerializeField]
	private Portrait portrait;

	public PlayerState currentState;

	[HideInInspector]
	public List<ConfigurableJoint> Joints = new List<ConfigurableJoint>();

	public bool CanControl = true;
	public bool OnHit = false;

	public PhotonView pv;
	public Cameracontroller Cam;

	//public float timer = 0f;

	[HideInInspector]
	public float HitHeight;

	private float GroundHeight;
	private float Failamount;

	private void Start()
	{
		ConfigurableJoint[] allChildren = GetComponentsInChildren<ConfigurableJoint>();
		foreach (ConfigurableJoint child in allChildren)
		{
			// 자기 자신의 경우엔 무시 
			// (게임오브젝트명이 다 다르다고 가정했을 때 통하는 코드)
			if (child.name == transform.name)
				return;

			Joints.Add(child);
		}
		OnGroundBehaviour();
	}

	public void OnGroundBehaviour()
	{
		pv.RPC(nameof(SpringOnRPC), RpcTarget.All);

		transform.GetComponent<FaceContoller>().MouthControl(0f);
		Cam.SetCameraMain();
		portrait.PortraitOFF();

		GroundHeight = UIManager.Instance.heightcheck.height;
	}

	public void FlytoGround()
    {
		currentState = PlayerState.Onground;

		transform.GetComponent<FaceContoller>().MouthControl(0f);
		Cam.SetCameraMain();
		portrait.PortraitOFF();

		GroundHeight = UIManager.Instance.heightcheck.height;
	}

	[PunRPC]
	public void SpringOnRPC()
    {
		StartCoroutine(nameof(SpringOn));
	}

	public void FlyingBehaviour()
    {
		currentState = PlayerState.Flying;
		Cam.SetCameraFly();
    }

	public void FallingBehaviour()
    {
		pv.RPC(nameof(PlayFallingMusic), RpcTarget.All);

		pv.RPC(nameof(SpringOFF), RpcTarget.All);

		transform.GetComponent<FaceContoller>().MouthControl(100f);
		Cam.SetCameraFalling();
		portrait.PortraitON();
	}

	[PunRPC]
	public void SpringOFF()
	{
		CanControl = false;
		OnHit = true;
		currentState = PlayerState.Falling;

		for (int i = 0; i < Joints.Count; i++)
		{
			JointDrive jointXDrive = Joints[i].angularXDrive;
			jointXDrive.positionSpring = 20f;
			Joints[i].angularXDrive = jointXDrive;

			JointDrive jointYZDrive = Joints[i].angularYZDrive;
			jointYZDrive.positionSpring = 20f;
			Joints[i].angularYZDrive = jointYZDrive;
			//키네마틱도 꺼주자
			Joints[i].GetComponent<Rigidbody>().isKinematic = false;
		}
	}

	IEnumerator SpringOn()
    {
		JointDrive temp = Joints[0].angularXDrive;
		if (temp.positionSpring == 500f) //Fly에서 Ground로
		{
			currentState = PlayerState.Onground;
			CanControl = true;
			OnHit = false;
			yield break;
		}
		else if(temp.positionSpring == 20f)//Falling에서 Ground로 
		{
			StartCoroutine(nameof(GetUp_temp));
			//코루틴을 돌려놓고 다음으로 넘어가버리면 스프링 값이 다 돌아오기 전에 CanControl이 켜져버림
			//이걸 방지하기 위해서 GetUp을 돌린 마지막 조인트 스프링 값 설정이 완료될 때까지 기다리는 yield return 을 실행.
			yield return new WaitUntil(() => Joints[Joints.Count - 1].angularYZDrive.positionSpring == 500);

			currentState = PlayerState.Onground;
			CanControl = true;
			OnHit = false;

			GameManager.Instance.fallCount++;
			pv.RPC(nameof(SpeakShit), RpcTarget.All);
		}
	}

	IEnumerator GetUp_temp()
	{
		int startValue = 20;
		int endValue = 500;
		float lerpTime = 0.5f;
		float currentTime = 0;
		float currentValue = startValue;

		JointDrive jointXDrive, jointYZDrive;
		
		while (currentValue < endValue)
		{
			currentTime += Time.deltaTime;
			if (currentTime >= lerpTime)
			{
				currentTime = lerpTime;
			}
			currentValue = Mathf.Lerp(startValue, endValue, currentTime / lerpTime);
			for(int i=0; i < Joints.Count; i++)
            {
				jointXDrive = Joints[i].angularXDrive;
				jointYZDrive = Joints[i].angularYZDrive;

				jointXDrive.positionSpring = currentValue;
				Joints[i].angularXDrive = jointXDrive;
				jointYZDrive.positionSpring = currentValue;
				Joints[i].angularYZDrive = jointYZDrive;
			}
			yield return null;
		}
		//여기서 CanControl을 해버리면 첫 번재 조인트 마무리 되고 나서 바로 조종 가능하게 됨
		//이전 코드에서는 조인트 개수만큼 CanControl이 true가 됐었다고 생각하면 될 듯
	}

	[PunRPC]
	private void PlayFallingMusic()
    {
		StopAllCoroutines();
		//StopCoroutine(nameof(GetUp_temp));
		//StopCoroutine(nameof(SpringOn));
		VoiceManager.Instance.StartFallingMusic();
		//StartCoroutine(VoiceManager.Instance.FallingMusic());
	}

	[PunRPC]
	public void SpeakShit()
	{
		//대사 치기
		if (HitHeight - GroundHeight >= 150f) //원래 150
		{
			if (GroundHeight < 1f)
			{
				VoiceManager.Instance.playVoice = true;
				if (SubtitleManager.Instance.CheckIsLeft("Reset"))
				{
					VoiceManager.Instance.subtitleKey = "Reset";
				}
				else
				{
					VoiceManager.Instance.subtitleKey = "Eliot";
				}
			}
			else
			{
				VoiceManager.Instance.playVoice = true;
				VoiceManager.Instance.subtitleKey = "SlowWalker";
			}
		}
		else if (GameManager.Instance.fallCount >= 20f) //원래 20
		{
			VoiceManager.Instance.playVoice = true;
			VoiceManager.Instance.subtitleKey = "Person";
		}
		//원래 브금 되돌리기
		//VoiceManager.Instance.BackToBGM();
		StartCoroutine(VoiceManager.Instance.BackToBGM());

		//높이 리셋
		HitHeight = 0;
		GroundHeight = 0;
	}
	public void noGravity()
	{
		//Spring off 하고 그래비티 꺼준다
		for (int i = 0; i < Joints.Count; i++)
		{
			Joints[i].GetComponent<Rigidbody>().isKinematic = false;
			Joints[i].GetComponent<Rigidbody>().useGravity = false;
		}
	}
}
