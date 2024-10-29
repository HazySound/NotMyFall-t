using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdArea : MonoBehaviour
{
	public GameObject Hiteffect;
	private bool Effect = true;//이펙트 대기 false 일때는 생성 안댐
	RagdollController ragdoll;
	[HideInInspector]
	public bool isTriggered = false;
	private AudioSource audio;
	[HideInInspector]
	public float timer = 0f;
	private List<BirdStrike> birds = new List<BirdStrike>();
	[HideInInspector]
	public bool isAllReturned = false;
	private WaitForSecondsRealtime waitRealOne = new WaitForSecondsRealtime(1f);

	private void Start()
    {
		audio = GetComponent<AudioSource>();
		foreach(Transform child in transform)
        {
			birds.Add(child.GetComponent<BirdStrike>());
        }
	}

	private void OnTriggerEnter(Collider collision)
	{
		ragdoll = collision.transform.GetComponentInParent<RagdollController>();
		if (ragdoll != null)
		{
			if (ragdoll.transform.CompareTag("Player"))
			{
				ragdoll.CanControl = false;
				ragdoll.OnHit = true;

				if (!isTriggered)
				{
					isTriggered = true;
					audio.Play();
				}
				if (ragdoll.currentState != RagdollController.PlayerState.Falling) //날아가거나 온그라운드 상태일 때 함정에 맞으면 onHit 켜주기 (falling일때는 이펙트만 출력)
				{
					//VoiceManager.Instance.hitMap = VoiceManager.Instance.currentMap;
					//VoiceManager.Instance.FallingMusic();
					//StartCoroutine(VoiceManager.Instance.FallingMusic());
					ragdoll.currentState = RagdollController.PlayerState.Falling;
					ragdoll.FallingBehaviour();
					ragdoll.HitHeight = Mathf.Max(ragdoll.HitHeight, UIManager.Instance.heightcheck.height);
				}
				else
				{
					ragdoll.FallingBehaviour();
				}
				//이펙트
				if (Hiteffect != null && Effect)
				{
					//충돌한 위치에다가 이펙트 스폰
					EffectSpawn(collision.transform.position);
				}
			}
		}
	}

	public void EffectSpawn(Vector3 origin)
	{
		Effect = false; // 드드드득 방지
		GameObject tempEffect = Instantiate(Hiteffect);
		float waitTime;
		if (tempEffect.GetComponent<AudioSource>() != null)
		{
			waitTime = tempEffect.GetComponent<AudioSource>().clip.length;
		}
		else { waitTime = 2f; }
		tempEffect.transform.position = origin;
		//코루틴으로 일정시간 뒤에 Effect true로
		StartCoroutine(EffectStandby());
		Destroy(tempEffect, waitTime);
	}

	IEnumerator EffectStandby()
	{
		yield return waitRealOne;
		Effect = true;
	}

	public void IsAllBirdReturned()
    {
		foreach(BirdStrike bird in birds)
        {
            if (!bird.isDone) {
				return;
			}
        }
		isTriggered = false;
		foreach (BirdStrike bird in birds)
		{
			bird.isDone = false;
			bird.timer = 0f;
		}
	}
}
