using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AnimationSync : MonoBehaviour,IPunObservable
{
    private Animator anim; // 애니메이터
    public float frame;// 애니메이션 키 프레임값
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) //내 로컬플레이어일때 - 지금 1p가 로컬플레이어인 상태가 지속되고 있어서
        {
            //1p 지금 애니메이션 재생 프레임을 2p로 줍
            stream.SendNext(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }
        else //내 로컬플레이어가 아닐때 - 2p가 데이터를 받아오기만 함.
        {
            //2P 프레임 받아서 저장
            //this.frame = (float)stream.ReceiveNext();
            anim.Play(0, -1, (float)stream.ReceiveNext());
        }
    }
	private void Start()
	{
        anim = transform.GetComponent<Animator>();
        PhotonNetwork.SendRate = 100;
        if (PhotonNetwork.IsMasterClient)
		{
            //1P 애니 시작
            anim.PlayInFixedTime(0);
        }
    }
    /***
	private void Update()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
            //2p
            anim.Play(0, -1, frame);
		}
	}
    ***/
}
