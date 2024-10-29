using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System;

public class Option_Lobby : MonoBehaviour
{
    [Header("AudioMixer")]
    public AudioMixer mixer;

    [Header("Slider")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfSlider;
    public Slider voiceSlider;

    [Header("Input")]
    public TMP_Dropdown selectResol;

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

        masterSlider.value = mastervol;
        bgmSlider.value = bgmvol;
        sfSlider.value = sfvol;
        voiceSlider.value = voicevol;

        MasterVolumeChange(mastervol);
        BGMVolumeChange(bgmvol);
        SFVolumeChange(sfvol);
        VoiceVolumeChange(voicevol);

        masterSlider.onValueChanged.AddListener(delegate { MasterVolumeChange(masterSlider.value); });
        bgmSlider.onValueChanged.AddListener(delegate { BGMVolumeChange(bgmSlider.value); });
        sfSlider.onValueChanged.AddListener(delegate { SFVolumeChange(sfSlider.value); });
        voiceSlider.onValueChanged.AddListener(delegate { VoiceVolumeChange(voiceSlider.value); });

        selectResol.onValueChanged.AddListener(delegate { SetResolution(selectResol.value); });
    }

    private void SetResolution(int opt)
    {
        Screen.SetResolution(setWidth[opt], setHeight[opt], Screen.fullScreen);

        if (!Screen.fullScreen)
        {
            PlayerPrefs.SetInt("isfull", 1);
        }
        else
        {
            PlayerPrefs.SetInt("isfull", 0);
        }
        
        PlayerPrefs.SetInt("resolution", opt);
    }

    public void GameSceneLoad()
    {
        PlayerPrefs.Save();
    }

    #region VolumeChange

    private void MasterVolumeChange(float vol)
    {
        if(vol == -20)
        {
            mixer.SetFloat("Master", -80);
        }            
        else
        {
            mixer.SetFloat("Master", vol);
        }

        PlayerPrefs.SetFloat("mastervol", vol);
    }

    private void BGMVolumeChange(float vol)
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

    private void SFVolumeChange(float vol)
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

    private void VoiceVolumeChange(float vol)
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
    #endregion
}
