using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Text")]
    public TMP_Text statusText;
    public TMP_Text loadingText;
    public TMP_Text passwordText;
    public TMP_Text roomnameText;
    public TMP_Text readyWaitText;

    [Header("InputField")]
    public TMP_InputField roomnameInput;
    public TMP_InputField roompasswordInput;
    public TMP_InputField passwordInput;
    
    [Header("Button")]
    public Button startBtn;
    
    public Button makeRoomBtn;
    public Button makeRoomCancelBtn;

    public Button joinRoomBtn;
    public Button joinRoomCancelBtn;
    
    public Button createRoomBtn;

    public Button selectSide;
    public Button readyBtn;
    public Button okBtn;
    public Button backBtn;
    public Button roomExitBtn;
    
    [Header("Toggle")]
    public Toggle Secret;
    
    [Header("Image")]
    public Image Left;
    public Image Right;

    [Header("GameObject")]
    public GameObject loadingPanel;
    public GameObject passwordPanel;
    public GameObject lobbyPanel;
    public GameObject createRoomPanel;

    public GameObject roomContainer;
    public GameObject startContainer;

    public GameObject RoomList;
    public GameObject playerMask;

    public GameObject Warning;
    public GameObject WrongPasswordText;
    
    public GameObject roomPrefab;
    public Transform scrollContent;

    private Option_Lobby optLobby;
    private PhotonView pv;

    private bool joinroomclicked = false;
    private bool alreadystart = false;

    private string roomnamesave;
    private readonly string gameVersion = "1.0";

    private readonly WaitForSeconds WaitASec = new(1f);

    private Dictionary<string, GameObject> roomDict = new();

    private Hashtable roomProps = new() { { "roomname", null }, { "password", null } };
    private Hashtable playerProps = new() { { "isready", false }, { "isleft", false } };

    private static LobbyManager instance = null;
    public static LobbyManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; //모든 씬을(누구 클라이언트인지 상관없이) 마스터 서버에 동기화
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.SerializationRate = 200;
        PhotonNetwork.SendRate = 60;

        Application.targetFrameRate = 60;

        startBtn.interactable = false;

        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Cursor.SetCursor(default, Vector2.zero, CursorMode.Auto);

        RoomListOFF();
        pv = GetComponent<PhotonView>();
        optLobby = GetComponentInChildren<Option_Lobby>();

        startBtn.onClick.AddListener(() => OnStartBtnClick());

        joinRoomBtn.onClick.AddListener(() => OnJoinRoomBtnClick());
        joinRoomCancelBtn.onClick.AddListener(() => OnJoinRoomCancelBtnClick());

        makeRoomBtn.onClick.AddListener(() => OnMakeRoomBtnClick());
        makeRoomCancelBtn.onClick.AddListener(() => OnMakeRoomCancelBtnClick());
        
        readyBtn.onClick.AddListener(() => OnReadyBtnClick());
        okBtn.onClick.AddListener(() => OnOkBtnClick());
        backBtn.onClick.AddListener(() => OnBackBtnClick());

        createRoomBtn.onClick.AddListener(() => OnCreateRoomBtnClick());

        roomExitBtn.onClick.AddListener(() => OnRoomExitBtnClick());

        startBtn.GetComponentInChildren<TMP_Text>().SetText("Waiting for Start");

        if (!alreadystart)
        {
            PhotonNetwork.ConnectUsingSettings(); //네트워크에 연결 시도
        }        
        statusText.SetText("서버 연결중...");
    }

    #region PUNCallBack
    public override void OnConnectedToMaster() //마스터 서버에 연결되면 실행됨
    {
        startBtn.GetComponentInChildren<TMP_Text>().SetText("Click to Start");
        startBtn.interactable = true;
        statusText.SetText("서버에 연결 성공");

        if (alreadystart) //이미 로그인했다면
        {
            //플레이어 커스텀 프로퍼티 초기화
            PlayerReset();

            //방목록 초기화 및 방입장 버튼 초기화
            roomDict.Clear();
            foreach (Transform child in scrollContent)
            {
                Destroy(child.gameObject);
            }

            joinroomclicked = false;

            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby() //로비에 연결되면 실행됨
    {
        
    }

    public override void OnDisconnected(DisconnectCause cause) //연결이 끊기면 호출됨
    {
        statusText.SetText("연결에 실패했습니다. 재시도 중...");
        PhotonNetwork.ConnectUsingSettings(); //다시 연결 시도
    }

    public override void OnJoinedRoom() //플레이어가 방에 들어가면 호출됨(해당 플레이어에게만)
    {
        //중복 클릭 방지
        createRoomBtn.interactable = false;
        makeRoomCancelBtn.interactable = false;
        roomExitBtn.interactable = true;
        readyBtn.interactable = true;

        createRoomPanel.SetActive(false);
        loadingPanel.SetActive(true);

        //방 이름 및 비밀번호 알려주기
        roomnameText.SetText("방 이름 : " + roomProps["roomname"]);

        if (string.IsNullOrEmpty(roomProps["password"].ToString())) //비밀번호가 없다면
        {
            passwordText.SetText("방 비밀번호 : 없음");
        }
        else //비밀번호가 있다면
        {
            passwordText.SetText("방 비밀번호 : " + roomProps["password"]);
        }

        //플레이어가 가득 찼다면
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            RoomIsFull();
        }
        //플레이어가 가득 차지 않았다면
        else
        {
            playerMask.SetActive(false);
            loadingText.SetText("다른 플레이어를 기다리는 중...");          
        }

        RoomListOFF(); //방 목록 투명하게
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //플레이어가 방에 들어가면 방에 있던 플레이어들에게 호출됨
    {
        loadingPanel.SetActive(true);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) //방이 가득 찼다면
        {
            RoomIsFull();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        NotReady();
        PlayerReset();
        playerMask.SetActive(false);
        loadingText.SetText(otherPlayer.NickName + " 님이 방에서 퇴장하셨습니다.\n다른 플레이어를 기다리는 중...");
    }

    public override void OnLeftRoom()
    {
        NotReady();
        readyWaitText.SetText("");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) //플레이어의 커스텀 프로퍼티가 업데이트되면 실행됨
    {
        Player[] players = PhotonNetwork.PlayerList;
        int count = 0;

        foreach (Player p in players)
        {
            if (p.CustomProperties.TryGetValue("isready", out object result))
            {
                if ((bool)result)
                {
                    count++;
                }
            }
        }

        if (count == 2)
        {
            selectSide.interactable = false;
            roomExitBtn.interactable = false;
            readyBtn.interactable = false;

            readyWaitText.SetText("게임이 곧 시작됩니다. 잠시 기다려주세요!");

            optLobby.GameSceneLoad();

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(1);
            }
        }
        else if (count == 0)
        {
            readyWaitText.SetText("");
        }
        else if(count == 1 && !(bool)PhotonNetwork.LocalPlayer.CustomProperties["isready"])
        {
            readyWaitText.SetText("다른 플레이어의 준비가 완료되었습니다.\n준비가 되면 버튼을 눌러주세요!");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (returnCode == 32760 && !string.IsNullOrEmpty(passwordInput.text))
        {
            StartCoroutine(WrongPassword());
        }

        passwordPanel.SetActive(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        StartCoroutine(AlreadyExist());
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) //방 목록이 업데이트되면 실행됨
    {
        GameObject tempRoom;

        foreach (var room in roomList)
        {
            if (room.RemovedFromList) //방이 삭제된다면(방이 없어짐)
            {
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
            }
            else
            {
                if (!roomDict.ContainsKey(room.Name)) //이미 생성된 방과 이름이 다르다면(새로 생성된 방이라면)
                {
                    GameObject _room = Instantiate(roomPrefab, scrollContent);
                    _room.GetComponent<RoomData>().RoomInfo = room;
                    roomDict.Add(room.Name, _room);
                }
                else //이미 생성된 방이라면
                {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                }
            }
        }

        if (joinroomclicked)
        {
            RoomListON();
        }
        else
        {
            RoomListOFF();
        }
    }
    #endregion

    #region ButtonClick
    public void OnStartBtnClick()
    {
        startBtn.interactable = false;
        alreadystart = true;
        Invoke(nameof(ActiveRoomContainer), 2f);
        Invoke(nameof(Connect), 2f);
    }

    public void OnMakeRoomBtnClick()
    {
        createRoomPanel.SetActive(true);
        roomContainer.SetActive(false);

        Secret.isOn = false;
        createRoomBtn.interactable = true;
        makeRoomCancelBtn.interactable = true;

        roomnameInput.text = "";
        roompasswordInput.text = "";
    }

    public void OnMakeRoomCancelBtnClick()
    {
        createRoomPanel.SetActive(false);
        roomContainer.SetActive(true);
    }

    public void OnJoinRoomBtnClick()
    {
        roomContainer.SetActive(false);
        joinRoomCancelBtn.gameObject.SetActive(true);

        RoomListON();

        joinroomclicked = true;
    }

    public void OnJoinRoomCancelBtnClick()
    {
        roomContainer.SetActive(true);
        joinRoomCancelBtn.gameObject.SetActive(false);

        RoomListOFF();

        joinroomclicked = false;
    }

    public void OnCreateRoomBtnClick() //방 생성 버튼을 클릭했다면
    {
        createRoomBtn.interactable = false;
        makeRoomCancelBtn.interactable = false;

        RoomOptions ro = new()
        {
            IsOpen = true,
            IsVisible = true,
            MaxPlayers = 2,
            PlayerTtl = 0,
            EmptyRoomTtl = 0
        };

        if (string.IsNullOrEmpty(roomnameInput.text))
        {
            roomnameInput.text = $"ROOM_{Random.Range(0,1000):0000}";
        }

        roomProps["roomname"] = roomnameInput.text;
        roomProps["password"] = roompasswordInput.text;

        ro.CustomRoomProperties = roomProps;
        ro.CustomRoomPropertiesForLobby = new string[] { "roomname", "password" };
        
        PhotonNetwork.CreateRoom(roomnameInput.text, ro, TypedLobby.Default);
    }

    public void OnReadyBtnClick()
    {
        Ready();

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    }

    public void OnEnterRoom(string roomName)
    {
        roomnamesave = roomName;
        passwordInput.text = "";

        roomProps["roomname"] = roomnamesave;
        roomProps["password"] = passwordInput.text;

        joinRoomCancelBtn.gameObject.SetActive(false);
        roomContainer.SetActive(false);

        PhotonNetwork.JoinRandomRoom(roomProps, 0);
    }

    public void OnBackBtnClick()
    {
        joinRoomCancelBtn.gameObject.SetActive(true);
        passwordPanel.SetActive(false);
    }

    public void OnRoomExitBtnClick()
    {
        loadingPanel.SetActive(false);
        roomContainer.SetActive(true);
        OnJoinRoomCancelBtnClick();

        PhotonNetwork.LeaveRoom();
    }

    public void OnOkBtnClick()
    {
        roomProps["roomname"] = roomnamesave;
        roomProps["password"] = passwordInput.text;

        if (string.IsNullOrEmpty(passwordInput.text))
        {
            StartCoroutine(WrongPassword());
            passwordPanel.SetActive(true);
        }
        else
        {
            passwordPanel.SetActive(false);
            PhotonNetwork.JoinRandomRoom(roomProps, 0);
        }
    }
    public void SwitchSide()
    {
        //PlayerMask.GetComponent<Image>().alphaHitTestMinimumThreshold = 1f;
        //플레이어 실루엣 클릭하면 호출
        
        NotReady();

        if (Left.isActiveAndEnabled)
        {
            pv.RPC("LeftClick", RpcTarget.Others);

            playerProps["isleft"] = false;

            Left.gameObject.SetActive(false);
            Right.gameObject.SetActive(true);
        }
        else if (Right.isActiveAndEnabled)
        {
            pv.RPC("RightClick", RpcTarget.Others);

            playerProps["isleft"] = true;

            Left.gameObject.SetActive(true);
            Right.gameObject.SetActive(false);
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    }
    #endregion

    #region PunRPC
    [PunRPC]
    public void LeftClick()
    {
        NotReady();

        playerProps["isleft"] = true;

        Left.gameObject.SetActive(true);
        Right.gameObject.SetActive(false);

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);        
    }

    [PunRPC]
    public void RightClick()
    {
        NotReady();

        playerProps["isleft"] = false;

        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(true);

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    }
    #endregion

    #region Coroutine
    IEnumerator WrongPassword()
    {
        WrongPasswordText.SetActive(true);
        passwordInput.text = "";

        yield return WaitASec;

        WrongPasswordText.SetActive(false);
    }

    IEnumerator AlreadyExist()
    {
        Warning.SetActive(true);

        yield return WaitASec;

        Warning.SetActive(false);
        createRoomBtn.interactable = true;
        makeRoomCancelBtn.interactable = true;
    }
    #endregion

    #region Function
    public void Connect() //로그인 버튼 누르면 실행될거임. 만약 닉네임 입력했으면, 그 닉대로 로그인되고 안 입력했으면 랜덤 아이디 생성됨 - 이건 조정가능
    {
        PhotonNetwork.LocalPlayer.NickName = $"USER_{Random.Range(0, 100):00}";

        PlayerReset();

        if (PhotonNetwork.IsConnected) //네트워크에 연결된 상태라면 로비에 입장시킴(로비에 입장해야 방 목록을 볼 수 있음)
        {
            statusText.SetText("로비에 접속하는 중...");

            roomDict.Clear();
            foreach (Transform child in scrollContent)
            {
                Destroy(child.gameObject);
            }

            PhotonNetwork.JoinLobby();
        }
        else //네트워크에 연결되지 않은 상태라면 다시 네트워크에 접속을 시도함.
        {
            statusText.SetText("연결에 실패했습니다. 재시도 중...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void RoomIsFull()
    {
        playerMask.SetActive(true);
        selectSide.interactable = true;
        loadingText.SetText("");

        if (PhotonNetwork.LocalPlayer.IsMasterClient) //마스터 클라이언트(방장)에게 왼쪽을 먼저 줌
        {
            Left.gameObject.SetActive(true);
            Right.gameObject.SetActive(false);

            playerProps["isleft"] = true;
        }
        else //방장이 아니면 오른쪽부터 시작
        {
            Left.gameObject.SetActive(false);
            Right.gameObject.SetActive(true);

            playerProps["isleft"] = false;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    }

    public void ActiveRoomContainer()
	{
        startContainer.SetActive(false);
        roomContainer.SetActive(true);
    }

    public void PlayerReset()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
    }

    private void Ready()
    {
        playerProps["isready"] = true;

        readyWaitText.SetText("상대방이 완료하기를 기다리는 중...");

        readyBtn.GetComponentInChildren<TMP_Text>().SetText("준비 완료");
        readyBtn.GetComponentInChildren<TMP_Text>().color = Color.green;
    }

    private void NotReady()
    {
        playerProps["isready"] = false;

        readyBtn.GetComponentInChildren<TMP_Text>().SetText("선택 완료");
        readyBtn.GetComponentInChildren<TMP_Text>().color = Color.white;
    }

    public void RoomListON()
    {
        RoomList.GetComponent<Image>().raycastTarget = true;
        RoomList.GetComponent<CanvasRenderer>().SetAlpha(200f);
        try
        {
            TMP_Text[] roomlisttext = RoomList.GetComponentsInChildren<TMP_Text>();
            Button[] enterroom = RoomList.GetComponentsInChildren<Button>();
            CanvasRenderer[] roomlistcanvas = RoomList.GetComponentsInChildren<CanvasRenderer>();

            foreach (TMP_Text text in roomlisttext)
            {
                text.color = Color.white;
            }

            foreach (Button enter in enterroom)
            {
                enter.interactable = enter.GetComponent<RoomData>().canJoin;
            }

            foreach (CanvasRenderer canvas in roomlistcanvas)
            {
                canvas.SetAlpha(255f);
            }
        }
        catch { }
    }

    public void RoomListOFF()
    {
        RoomList.GetComponent<CanvasRenderer>().SetAlpha(0f);
        RoomList.GetComponent<Image>().raycastTarget = false;
        try
        {
            TMP_Text[] roomlisttext = RoomList.GetComponentsInChildren<TMP_Text>();
            Button[] enterroom = RoomList.GetComponentsInChildren<Button>();
            CanvasRenderer[] roomlistcanvas = RoomList.GetComponentsInChildren<CanvasRenderer>();

            foreach (TMP_Text text in roomlisttext)
            {
                text.color = Color.clear;
            }

            foreach (Button enter in enterroom)
            {
                enter.interactable = false;
            }

            foreach (CanvasRenderer canvas in roomlistcanvas)
            {
                canvas.SetAlpha(0f);
            }
        }
        catch { }
    }
    #endregion
}
