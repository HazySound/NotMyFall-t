using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Audio;
using Photon.Pun;
using Photon.Realtime;

public class Option_Game : MonoBehaviourPunCallbacks
{
    [Header("GameObject")]
    public GameObject optionPanel;
    public GameObject quitPanel;

    [Header("AudioMixer")]
    public AudioMixer mixer;

    [Header("Slider")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfSlider;
    public Slider voiceSlider;

    [Header("Input")]
    public TMP_Dropdown selectResol;

    public Toggle fullScreen;
    public Image img;
    public TMP_Text txt;

    private readonly int[] setWidth = { 2580, 1920, 1366, 1280 };
    private readonly int[] setHeight = { 1440, 1080, 768, 720 };

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("resolution"))
            PlayerPrefs.SetInt("resolution", 2);

        if (!PlayerPrefs.HasKey("isfull"))
            PlayerPrefs.SetInt("isfull", 1); // 0 is full 1 is window

        if (!PlayerPrefs.HasKey("mastervol"))
            PlayerPrefs.SetFloat("mastervol", 0);

        if (!PlayerPrefs.HasKey("bgmvol"))
            PlayerPrefs.SetFloat("bgmvol", 0);

        if (!PlayerPrefs.HasKey("sfvol"))
            PlayerPrefs.SetFloat("sfvol", 0);

        if (!PlayerPrefs.HasKey("voicevol"))
            PlayerPrefs.SetFloat("voicevol", 0);

        int resol = PlayerPrefs.GetInt("resolution");
        int isfull = PlayerPrefs.GetInt("isfull");

        float mastervol = PlayerPrefs.GetFloat("mastervol");
        float bgmvol = PlayerPrefs.GetFloat("bgmvol");
        float sfvol = PlayerPrefs.GetFloat("sfvol");
        float voicevol = PlayerPrefs.GetFloat("voicevol");

        Screen.SetResolution(setWidth[resol], setHeight[resol], isfull == 0);

        selectResol.value = resol;
        fullScreen.isOn = isfull == 0;

        masterSlider.value = mastervol;
        bgmSlider.value = bgmvol;
        sfSlider.value = sfvol;
        voiceSlider.value = voicevol;

        MasterVolumeChange(mastervol);
        BGMVolumeChange(bgmvol);
        SFVolumeChange(sfvol);
        VoiceVolumeChange(voicevol);
        MapVolumeInit();

        masterSlider.onValueChanged.AddListener(delegate { MasterVolumeChange(masterSlider.value); });
        bgmSlider.onValueChanged.AddListener(delegate { BGMVolumeChange(bgmSlider.value); });
        sfSlider.onValueChanged.AddListener(delegate { SFVolumeChange(sfSlider.value); });
        voiceSlider.onValueChanged.AddListener(delegate { VoiceVolumeChange(voiceSlider.value); });

        selectResol.onValueChanged.AddListener(delegate { SetResolution(selectResol.value); });
        fullScreen.onValueChanged.AddListener(delegate { SetResolution(selectResol.value); });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!GameManager.Instance.isPause) //not paused
            {
                Cursor.visible = true;
                Cursor.SetCursor(default, Vector2.zero, CursorMode.Auto);
                optionPanel.SetActive(true);
                GameManager.Instance.isPause = true;
            }
            else
            {
                //Cursor.visible = false;
                Cursor.SetCursor(GameManager.Instance.GameCusor, Vector2.zero, CursorMode.Auto);
                optionPanel.SetActive(false);
                GameManager.Instance.isPause = false;
            }
        }
    }

    public void SetResolution(int opt)
    {
        if(fullScreen.isOn)
        {
            Screen.SetResolution(setWidth[opt], setHeight[opt], true);
            PlayerPrefs.SetInt("isfull", 0);
        }
        else
        {
            Screen.SetResolution(setWidth[opt], setHeight[opt], false);
            PlayerPrefs.SetInt("isfull", 1);
        }

        PlayerPrefs.SetInt("resolution", opt);
    }

    #region Quit
    public void GameQuit()
    {
        quitPanel.SetActive(true);
    }

    public void OnClickQuit()
    {
        PlayerPrefs.Save();
        PhotonNetwork.LeaveRoom();
    }

    public void OnCancelQuit()
    {
        quitPanel.SetActive(false);
    }

    public void LobbySceneLoad()
    {
        mixer.SetFloat("Map", 0);
        PlayerPrefs.Save();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        txt.SetText(otherPlayer.NickName + " 님이 방에서 나가셨습니다. \n자동으로 로비로 나가집니다.");
        img.gameObject.SetActive(true);

        StartCoroutine(LeftGame());
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }

    IEnumerator LeftGame()
    {
        yield return new WaitForSeconds(2f);
        img.gameObject.SetActive(false);
        LobbySceneLoad();
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region VolumeChange
    public void MasterVolumeChange(float vol)
    {
        if (vol == -20)
        {
            mixer.SetFloat("Master", -80);
        }
        else
        {
            mixer.SetFloat("Master", vol);
        }

        PlayerPrefs.SetFloat("mastervol", vol);
    }

    public void BGMVolumeChange(float vol)
    {
        if (vol == -20)
        {
            mixer.SetFloat("BGM", -80);
        }
        else
        {
            mixer.SetFloat("BGM", vol);
        }

        PlayerPrefs.SetFloat("bgmvol", vol);
    }

    public void SFVolumeChange(float vol)
    {
        if (vol == -20)
        {
            mixer.SetFloat("SF", -80);
        }
        else
        {
            mixer.SetFloat("SF", vol);
        }

        PlayerPrefs.SetFloat("sfvol", vol);
    }

    public void VoiceVolumeChange(float vol)
    {
        if (vol == -20)
        {
            mixer.SetFloat("Voice", -80);
        }
        else
        {
            mixer.SetFloat("Voice", vol);
        }

        PlayerPrefs.SetFloat("voicevol", vol);
    }

    private void MapVolumeInit()
    {
        mixer.SetFloat("Map", 0);
    }
    #endregion
}
