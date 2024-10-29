using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    #region **변수 선언 파트 ...
    //마우스를 활용하여 팔의 움직임을 구현하고, 키보드 A와 D키를 활용하여 다리의 움직임을 구현한다.
    //유니티에서 기본 제공되는 Slider를 활용한다.
    [SerializeField]
    private Slider slider_L, slider_R;
    [SerializeField]
    private Slider slider_leg_L, slider_leg_R;

    private readonly float closed_leg = 0;
    private readonly float open_leg = 100;
    private float mousePos;

    private PhotonView pv;

    //애니메이터를 활용해 팔/다리를 움직여준다.
    //더미 오브젝트의 애니메이션을 재생하고 Ragdoll 오브젝트는 해당 더미 오브젝트의 애니메이션을 따라하도록 하여 움직임을 구현한다.
    private Animator anim;
    private int player;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();

        //애니메이션 제어를 위해 스피드는 0으로 고정.
        anim.speed = 0;

        slider_L.minValue = 0;
        slider_L.maxValue = Screen.height;

        slider_R.minValue = 0;
        slider_R.maxValue = Screen.height;

        slider_leg_L.minValue = closed_leg;
        slider_leg_L.maxValue = open_leg;

        slider_leg_R.minValue = closed_leg;
        slider_leg_R.maxValue = open_leg;

        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isleft"]) //left
        {
            player = 1;

            slider_L.onValueChanged.AddListener(delegate { PushData_PL(); });
            slider_leg_L.onValueChanged.AddListener(delegate { PushData_PL(); });
        }
        else //right
        {
            player = 2;

            slider_R.onValueChanged.AddListener(delegate { PushData_PR(); });
            slider_leg_R.onValueChanged.AddListener(delegate { PushData_PR(); });
        }
    }

    //게임 도중 전체화면으로 변경하거나 해상도가 변경되었을 때 해당 함수가 실행되어 마우스포지션의 범위제한을 현재 화면 높이로 갱신해준다.
    //Scene 내의 UI -> Opt_game -> OptionPanel -> Video 오브젝트 하위에 존재하는 [SelectResolution]과 [FullScreen] 오브젝트의 On Value Changed 패널에 본 함수가 등록되어 있다.
    public void UpdateScreenSize()
    {
        slider_L.maxValue = Screen.height;
        slider_R.maxValue = Screen.height;
    }

    void Update()
    {
        //Pause 상태일 경우에는 마우스 위치를 더 이상 받지 않고 Update 함수를 종료함.
        if (GameManager.Instance.isPause) return;

        //마우스 인풋으로 슬라이더값조정
        mousePos = Input.mousePosition.y;

        /* 아래 코드와 관련된 설명
         * 해당 코드는 1P와 2P 클라이언트 모두 작동하므로, 현재 내 클라이언트가 1P인지 2P인지 확인 후 그에 맞는 방향의 팔다리를 조종하도록 함.
         
         * 다리의 경우 팔과 같이 A 또는 D키를 꾹 눌렀을 때 다리가 서서히 벌어지고 닫히는 방식을 통해 미세한 컨트롤이 가능하도록 기획했었으나,
           테스트 결과 조작감이 매우 좋지 않고 부스터를 작동시키는 시간이나 팔만으로도 미세한 컨트롤이 가능한 것이 확인되었다. 
           따라서 On/Off 방식을 활용하여 한 번의 입력만으로 다리가 펴지거나 오므려지도록 변경하였다.
        */
        if (player == 1) //1P의 경우
        {
            slider_L.value = mousePos;
            if (Input.GetKey(KeyCode.A))
            {
                slider_leg_L.value = open_leg;//딸깍
            }
            else if (Input.GetKey(KeyCode.D))
            {
                slider_leg_L.value = closed_leg;//딸깍
            }
        }
        else //2P의 경우
        {
            slider_R.value = mousePos;
            if (Input.GetKey(KeyCode.A))
            {
                slider_leg_R.value = closed_leg;//딸깍
            }
            else if (Input.GetKey(KeyCode.D))
            {
                slider_leg_R.value = open_leg;//딸깍
            }
        }
    }

    //RPC 함수를 작동시킨다. 각각 오른쪽, 왼쪽을 담당
    /* normalizedValue 사용 이유
         Slider 값을 0~1 사이로 정규화 시켜 양쪽 클라이언트에 해당 값을 전송한다.
         애니메이션의 재생 포인트 값이 0~1 사이이므로 Slider의 정규화 값을 활용하면
         Slider 값의 비율에 맞는 애니메이션 재생이 가능하다.
         애니메이션의 시작 : 0, 애니메이션의 마지막 : 1 
    */
    void PushData_PL()
    {
        pv.RPC("MoveHand_R", RpcTarget.AllViaServer, slider_L.normalizedValue);
        pv.RPC("MoveLeg_R", RpcTarget.AllViaServer, slider_leg_L.normalizedValue);
    }

    void PushData_PR()
    {
        pv.RPC("MoveHand_L", RpcTarget.AllViaServer, slider_R.normalizedValue);
        pv.RPC("MoveLeg_L", RpcTarget.AllViaServer, slider_leg_R.normalizedValue);
    }

    #region PunRPC
    [PunRPC]
    void MoveHand_L(float value)
    {
        anim.Play("hand_L", -1, value);
    }

    [PunRPC]
    void MoveHand_R(float value)
    {
        anim.Play("hand_R", -1, value);
    }

    [PunRPC]
    void MoveLeg_L(float value)
    {
        anim.Play("LegL", -1, value);
    }

    [PunRPC]
    void MoveLeg_R(float value)
    {
        anim.Play("LegR", -1, value);
    }
    #endregion
}
