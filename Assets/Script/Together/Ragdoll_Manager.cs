using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ragdoll_Manager : MonoBehaviour
{
	private enum PlayerState 
	{
		Playing,
		Ragdoll,
		WakingUp
	}
	private PlayerState currentState = PlayerState.Playing;
	public GameObject Arm_L; //회전시 축이 되는 오브젝트 
	public GameObject Arm_R;
	//팔 부분은 중력 끄고 Kinematic 체크
	//다리도?
	public GameObject Leg_L;
	public GameObject Leg_R;
	public List<GameObject> Ragdollbits = new List<GameObject>();
	public List<GameObject> PlayerableJoint = new List<GameObject>();
	public  Animator animator;
	private Transform hipsBone;
	public float WakeUpTime;
	public bool IsRagdollOn;
	public GameObject Player_Ragdoll;
	//디버깅용
	public TextMeshProUGUI text;
	private void Awake()
	{
		hipsBone = animator.GetBoneTransform(HumanBodyBones.Hips);
	}
	public void Start()
	{
		//스캔
		Rigidbody[] allChildren = Player_Ragdoll.GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody child in allChildren)
		{
			// 자기 자신의 경우엔 무시 
			// (게임오브젝트명이 다 다르다고 가정했을 때 통하는 코드)
			if (child.name == transform.name)
				return;

			Ragdollbits.Add(child.gameObject);
			if(child.tag == "PlayerableJoint")
			{
				PlayerableJoint.Add(child.gameObject);
			}
		}
		GameModeOn();
	}
	private void Update()
	{
		switch (currentState)
		{
			case PlayerState.Playing: // 플레이어가 조작하는 상태 //게임모드 ON, 애니메이터 OFF
				PlayingBehaviour();
				break;
			case PlayerState.Ragdoll: // 플레이어 조작 불가 // 게임모드 OFF, 애니메이터 OFF
				RadgollBehaviour();
				break;
			case PlayerState.WakingUp: // 래그돌에서 일어나는 애니메이션 // 애니메이터 ON // Ragdoll -> WakingUp -> Playing 이런 순서
				WakingUpBehaviour();
				break;
		}
		text.text = currentState.ToString();
	}
	public void RagdollButton()
	{
		IsRagdollOn = true;
	}
	public void RagdollOFF()
	{
		//전신 래그돌 해제 -> 쓸일 있나?
		//위에 오브젝트에서 콜라이더 끄고, 리지드바디 중력 끄고 애니메이터 켜기
		IsRagdollOn = false;
		for (int i =0; i < Ragdollbits.Count; i++)
		{
			Ragdollbits[i].gameObject.GetComponent<Rigidbody>().useGravity = false;
			Ragdollbits[i].gameObject.GetComponent<Rigidbody>().isKinematic = true;
			Ragdollbits[i].gameObject.GetComponent<Collider>().enabled = false;
		}
		animator.enabled = true;
	}
	public void RagdollOn()
	{
		//전신 래그돌
		//위에 오브젝트에서 콜라이더 키고, 중력 키고. 애니메이터 끄기
		for (int i = 0; i < Ragdollbits.Count; i++)
		{
			Ragdollbits[i].gameObject.GetComponent<Rigidbody>().useGravity = true;
			Ragdollbits[i].gameObject.GetComponent<Rigidbody>().isKinematic = false;
			Ragdollbits[i].gameObject.GetComponent<Collider>().enabled = true;
		}
		animator.enabled = false;
		//일어나기 
		WakeUpTime = Random.Range(3, 5);
	}
	public void GameModeOn()
	{
		//처음 시작시, 넘어졌다가 일어난 이후
		//조작할수 있게 팔과 다리부분 리지드바디 조정
		for (int i = 0; i < PlayerableJoint.Count; i++)
		{
			PlayerableJoint[i].gameObject.GetComponent<Rigidbody>().useGravity = false;
			PlayerableJoint[i].gameObject.GetComponent<Rigidbody>().isKinematic = true;
			//PlayerableJoint[i].gameObject.GetComponent<Collider>().enabled = false;
		}
	}
	public void GameModeOff()
	{
		//장애물에 부딛히거나 기절할 경우// 전체 래그돌화 하는것과 같은 시점
		for (int i = 0; i < PlayerableJoint.Count; i++)
		{
			PlayerableJoint[i].gameObject.GetComponent<Rigidbody>().useGravity = true;
			PlayerableJoint[i].gameObject.GetComponent<Rigidbody>().isKinematic = false;
			PlayerableJoint[i].gameObject.GetComponent<Collider>().enabled = true;
		}
	}
	public void PlayingBehaviour() 
	{
		//플레이어가 조작중인 상태

		//마우스 입력 받으면 애니메이터 끄기?
		if (Input.GetMouseButton(0))
		{
			animator.enabled = false;
		}
		else
		{
			animator.enabled = true;
		}

		if (IsRagdollOn)
		{
			RagdollOn();
			currentState = PlayerState.Ragdoll;
		}
	}
	public void RadgollBehaviour()
	{
		//장애물 등에 닿아서 래그돌화 한 상태
		WakeUpTime -= Time.deltaTime;
		if(WakeUpTime <= 0)
		{
			AlignPositionToHips();
			//Hips 의 z 로테이션을 기반으로 어느 상황인지 
			StandingDetect();
			currentState = PlayerState.WakingUp;
			//애니메이터 On 
			RagdollOFF();
			animator.SetTrigger("WakeUp");
		}
	}
	public void WakingUpBehaviour()
	{
		//일어나는 애니메이션 재생 후
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
		{
			//AlignPositionToHips();
			currentState = PlayerState.Playing;
			GameModeOn();
		}
	}
	private void AlignPositionToHips()
	{
		//엉덩이 위치 맞춰줘서 애니메이션 위치 똑같게
		Vector3 OriginalHipsPosition = hipsBone.position;
		transform.position = hipsBone.position;

		if(Physics.Raycast(transform.position,Vector3.down,out RaycastHit hitInfo))
		{
			transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
		}
		hipsBone.position = OriginalHipsPosition;
	}
	private void StandingDetect()
	{
		Vector3 rootBoneForward = hipsBone.rotation * Vector3.forward;
		if (Vector3.Dot(rootBoneForward, Vector3.down) >= 0f)
		{
			Debug.Log("앞으로 넘어짐");
			animator.SetInteger("ID", 1);
		}
		else
		{
			Debug.Log("뒤로 넘어짐");
			animator.SetInteger("ID", 2);
		}
		/**
		if (Vector3.Dot(rootBoneForward, Vector3.down) >= 0f) // Check if ragdoll is lying on its back or front, then transition to getup animation
		{
			if (!animator.GetCurrentAnimatorStateInfo(0).fullPathHash.Equals(hash.getupFront))
				animator.SetBool(hash.frontTrigger, true);
			else // if (!anim.GetCurrentAnimatorStateInfo(0).IsName("GetupFrontMirror"))
				animator.SetBool(hash.frontMirrorTrigger, true);
		}
		else
		{
			if (!animator.GetCurrentAnimatorStateInfo(0).fullPathHash.Equals(hash.getupBack))
				animator.SetBool(hash.backTrigger, true);
			else // if (!anim.GetCurrentAnimatorStateInfo(0).IsName("GetupFrontMirror"))
				animator.SetBool(hash.backMirrorTrigger, true);
		}
		**/
	}
}
