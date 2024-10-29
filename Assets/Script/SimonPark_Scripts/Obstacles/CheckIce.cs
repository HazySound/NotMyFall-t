using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIce : MonoBehaviour
{
    private RagdollController ragdoll;
    [SerializeField]
    private GameObject Hiteffect;
    private WaitForSecondsRealtime waitRealOne = new WaitForSecondsRealtime(1f);

    private int colliderCnt = 0;

    private bool Effect = true;//이펙트 대기 false 일때는 생성 안댐
    public bool freezeDone = false;
    private bool _isInIce = false;
    public bool isInIce
    {
        get { return _isInIce; }
    }

    private void EffectSpawn(Vector3 origin)
    {
        Effect = false; // 드드드득 방지
        GameObject ragdollEffect = Instantiate(Hiteffect);
        float waitTime;
        if (ragdollEffect.GetComponent<AudioSource>() != null)
        {
            waitTime = ragdollEffect.GetComponent<AudioSource>().clip.length;
        }
        else { waitTime = 2f; }
        ragdollEffect.transform.position = origin;
        //코루틴으로 일정시간 뒤에 Effect true로
        StartCoroutine(nameof(EffectStandby));
        Destroy(ragdollEffect, waitTime);
    }

    IEnumerator EffectStandby()
    {
        yield return waitRealOne;
        Effect = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 || other.gameObject.layer == 7)
        {
            colliderCnt++;
            _isInIce = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 6 || other.gameObject.layer == 7)
        {
            if (freezeDone)
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
                    EffectSpawn(other.transform.position);
                }
                freezeDone = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6 || other.gameObject.layer == 7)
        {
            colliderCnt--;
            if(colliderCnt == 0)
            {
                _isInIce = false;
                freezeDone = false;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        ragdoll = GameManager.Instance.ragdollController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
