using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomData : MonoBehaviourPunCallbacks
{
    private TMP_Text RoomInfoText;
    private RoomInfo _roomInfo;
    public bool canJoin = true;

    public RoomInfo RoomInfo
    {
        get
        {
            return _roomInfo;
        }
        set
        {
            _roomInfo = value;
            RoomInfoText.SetText($"{_roomInfo.Name} ({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})");

            canJoin = (_roomInfo.PlayerCount < _roomInfo.MaxPlayers);

            GetComponent<Button>().onClick.RemoveAllListeners();

            GetComponent<Button>().onClick.AddListener(() => LobbyManager.Instance.OnEnterRoom(_roomInfo.Name));            
        }
    }

    private void Awake()
    {
        RoomInfoText = GetComponentInChildren<TMP_Text>();
    }
}
