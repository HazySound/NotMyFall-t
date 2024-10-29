using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleShot : MonoBehaviour
{
    public GameObject Hiteffect;
    public bool CanBeTrigger = false;
    private bool Effect = true;//이펙트 대기 false 일때는 생성 안댐
    RagdollController ragdoll;
	private EagleContainer eagle;

	private WaitForSecondsRealtime waitRealOne = new WaitForSecondsRealtime(1f);
	private WaitForSeconds waitTwo = new WaitForSeconds(2f);

	private void Start()
    {
		eagle = GetComponentInParent<EagleContainer>();
    }

	private void OnCollisionEnter(Collision collision)
	{
		ragdoll = collision.transform.GetComponentInParent<RagdollController>();
		if (ragdoll != null)
		{
			if (ragdoll.transform.CompareTag("Player"))
			{
				ragdoll.CanControl = false;
				ragdoll.OnHit = true;

				if (ragdoll.currentState != RagdollController.PlayerState.Falling) //날아가거나 온그라운드 상태일 때 함정에 맞으면 onHit 켜주기 (falling일때는 이펙트만 출력)
				{
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
				//.5초 뒤 트리거 켰다가 .5뒤에 다시 트리거 off 
				if (CanBeTrigger)
				{
					TriggerOnOFF();
				}
				if (!eagle.isReturning)
				{
					eagle.BackToOrigin();
				}
			}
		}
	}

	public void EffectSpawn(Vector3 origin)
	{
		Effect = false; // 드드드득 방지
		GameObject tempEffect = Instantiate(Hiteffect);
		tempEffect.transform.position = origin;
		//코루틴으로 일정시간 뒤에 Effect true로
		StartCoroutine(EffectStandby());
		Destroy(tempEffect, 2f);
	}
	public void TriggerOnOFF()
	{
		StartCoroutine(TriggerOn());
	}
	IEnumerator TriggerOn()
	{
		transform.GetComponent<Collider>().isTrigger = true;
		yield return waitTwo;
		transform.GetComponent<Collider>().isTrigger = false;
	}
	IEnumerator EffectStandby()
	{
		yield return waitRealOne;
		Effect = true;
	}
}
