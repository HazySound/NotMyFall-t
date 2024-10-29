using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    #region **���� ���� ��Ʈ ...
    //���콺�� Ȱ���Ͽ� ���� �������� �����ϰ�, Ű���� A�� DŰ�� Ȱ���Ͽ� �ٸ��� �������� �����Ѵ�.
    //����Ƽ���� �⺻ �����Ǵ� Slider�� Ȱ���Ѵ�.
    [SerializeField]
    private Slider slider_L, slider_R;
    [SerializeField]
    private Slider slider_leg_L, slider_leg_R;

    private readonly float closed_leg = 0;
    private readonly float open_leg = 100;
    private float mousePos;

    private PhotonView pv;

    //�ִϸ����͸� Ȱ���� ��/�ٸ��� �������ش�.
    //���� ������Ʈ�� �ִϸ��̼��� ����ϰ� Ragdoll ������Ʈ�� �ش� ���� ������Ʈ�� �ִϸ��̼��� �����ϵ��� �Ͽ� �������� �����Ѵ�.
    private Animator anim;
    private int player;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();

        //�ִϸ��̼� ��� ���� ���ǵ�� 0���� ����.
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

    //���� ���� ��üȭ������ �����ϰų� �ػ󵵰� ����Ǿ��� �� �ش� �Լ��� ����Ǿ� ���콺�������� ���������� ���� ȭ�� ���̷� �������ش�.
    //Scene ���� UI -> Opt_game -> OptionPanel -> Video ������Ʈ ������ �����ϴ� [SelectResolution]�� [FullScreen] ������Ʈ�� On Value Changed �гο� �� �Լ��� ��ϵǾ� �ִ�.
    public void UpdateScreenSize()
    {
        slider_L.maxValue = Screen.height;
        slider_R.maxValue = Screen.height;
    }

    void Update()
    {
        //Pause ������ ��쿡�� ���콺 ��ġ�� �� �̻� ���� �ʰ� Update �Լ��� ������.
        if (GameManager.Instance.isPause) return;

        //���콺 ��ǲ���� �����̴�������
        mousePos = Input.mousePosition.y;

        /* �Ʒ� �ڵ�� ���õ� ����
         * �ش� �ڵ�� 1P�� 2P Ŭ���̾�Ʈ ��� �۵��ϹǷ�, ���� �� Ŭ���̾�Ʈ�� 1P���� 2P���� Ȯ�� �� �׿� �´� ������ �ȴٸ��� �����ϵ��� ��.
         
         * �ٸ��� ��� �Ȱ� ���� A �Ǵ� DŰ�� �� ������ �� �ٸ��� ������ �������� ������ ����� ���� �̼��� ��Ʈ���� �����ϵ��� ��ȹ�߾�����,
           �׽�Ʈ ��� ���۰��� �ſ� ���� �ʰ� �ν��͸� �۵���Ű�� �ð��̳� �ȸ����ε� �̼��� ��Ʈ���� ������ ���� Ȯ�εǾ���. 
           ���� On/Off ����� Ȱ���Ͽ� �� ���� �Է¸����� �ٸ��� �����ų� ���Ƿ������� �����Ͽ���.
        */
        if (player == 1) //1P�� ���
        {
            slider_L.value = mousePos;
            if (Input.GetKey(KeyCode.A))
            {
                slider_leg_L.value = open_leg;//����
            }
            else if (Input.GetKey(KeyCode.D))
            {
                slider_leg_L.value = closed_leg;//����
            }
        }
        else //2P�� ���
        {
            slider_R.value = mousePos;
            if (Input.GetKey(KeyCode.A))
            {
                slider_leg_R.value = closed_leg;//����
            }
            else if (Input.GetKey(KeyCode.D))
            {
                slider_leg_R.value = open_leg;//����
            }
        }
    }

    //RPC �Լ��� �۵���Ų��. ���� ������, ������ ���
    /* normalizedValue ��� ����
         Slider ���� 0~1 ���̷� ����ȭ ���� ���� Ŭ���̾�Ʈ�� �ش� ���� �����Ѵ�.
         �ִϸ��̼��� ��� ����Ʈ ���� 0~1 �����̹Ƿ� Slider�� ����ȭ ���� Ȱ���ϸ�
         Slider ���� ������ �´� �ִϸ��̼� ����� �����ϴ�.
         �ִϸ��̼��� ���� : 0, �ִϸ��̼��� ������ : 1 
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
