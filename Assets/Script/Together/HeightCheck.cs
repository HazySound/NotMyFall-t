using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeightCheck : MonoBehaviour
{
	[SerializeField]
	public TextMeshProUGUI HeightText;
	public float height;
	private AudioSource bgm;
	private RagdollController ragdoll;
	private bool bgmChanged = false;
	private void FixedUpdate()
	{
		
		height = GameManager.Instance.hips.transform.position.y;
		HeightText.text = ((int)height).ToString();
		/*
        if (ragdoll.currentState != RagdollController.PlayerState.Falling)
        {
			if(34.5f <= height && height <= 45.5f)
            {
				LabToCity();
				VoiceManager.Instance.isChangingClip = true;
            }
			else if(134.5f <= height && height <= 145.5f)
            {
				CityToMountain();
				VoiceManager.Instance.isChangingClip = true;
			}
			else if(239.5f <= height && height <= 256.5f)
            {
				MountainToSpace();
				VoiceManager.Instance.isChangingClip = true;
			}
            else
            {
				VoiceManager.Instance.isChangingClip = false;
			}
        }
        else //함정에 맞고 떨어지고 있는 상황
        {
			if(height < 39.5f)
            {
				VoiceManager.Instance.currentMap = VoiceManager.Clip.Lab;
            }
            else if(height < 139.5f)
            {
				VoiceManager.Instance.currentMap = VoiceManager.Clip.City;
			}
            else
            {
				VoiceManager.Instance.currentMap = VoiceManager.Clip.Mountain;
			}
        }
		*/
	}

	private void ChangeBgmClip()
    {
		bgmChanged = true;
		bgm.volume = 0f;
		if (39.5f <= height && height <= 40.5f)
        {
			if (VoiceManager.Instance.currentMap == VoiceManager.Clip.Lab) //랩에서 도시로 올라가는 경우 도시 브금으로 교체
			{
				bgm.clip = VoiceManager.Instance.audioClips[(int)VoiceManager.Clip.City].clip;
				VoiceManager.Instance.currentMap = VoiceManager.Clip.City;
			}
			else //도시에서 랩으로 내려가는 경우 도시 브금으로 교체
			{
				bgm.clip = VoiceManager.Instance.audioClips[(int)VoiceManager.Clip.Lab].clip;
				VoiceManager.Instance.currentMap = VoiceManager.Clip.Lab;
			}
		}
		else if(139.5f <= height && height <= 140.5f)
        {
			if (VoiceManager.Instance.currentMap == VoiceManager.Clip.City) //도시에서 설산으로 올라가는 경우 도시 브금으로 교체
			{
				bgm.clip = VoiceManager.Instance.audioClips[(int)VoiceManager.Clip.Mountain].clip;
				VoiceManager.Instance.currentMap = VoiceManager.Clip.Mountain;
			}
			else //설산에서 도시로 내려가는 경우 도시 브금으로 교체
			{
				bgm.clip = VoiceManager.Instance.audioClips[(int)VoiceManager.Clip.City].clip;
				VoiceManager.Instance.currentMap = VoiceManager.Clip.City;
			}
		}
		else if (254.5f <= height && height <= 256.5f)
		{
			bgm.clip = VoiceManager.Instance.audioClips[(int)VoiceManager.Clip.Space].clip;
			VoiceManager.Instance.currentMap = VoiceManager.Clip.Space;
		}
		VoiceManager.Instance.hitMap = VoiceManager.Instance.currentMap;
		bgm.Play();
	}

	private void LabToCity()
    {
		if (height >= 34.5f && height < 39.5f)
		{
			//볼륨 다운
			bgmChanged = false;
			bgm.volume = (1 - (height - 34.5f) * 0.2f) * VoiceManager.Instance.audioClips[(int)VoiceManager.Clip.Lab].volume;
		}
		else if (39.5f <= height && height <= 40.5f && !bgmChanged)
		{
			//클립 교체 및 플레이
			ChangeBgmClip();
		}
		else if (height > 40.5f && height <= 44.5f)
		{
			//볼륨 업
			bgmChanged = false;
			bgm.volume = ((height - 40.5f) * 0.25f) * VoiceManager.Instance.audioClips[(int)VoiceManager.Clip.City].volume;
		}
	}

	private void CityToMountain()
	{
		if (height >= 134.5f && height < 139.5f)
		{
			//볼륨 다운
			bgmChanged = false;
			bgm.volume = (1 - (height - 134.5f) * 0.2f) * VoiceManager.Instance.audioClips[(int)VoiceManager.Clip.City].volume;
		}
		else if (139.5f <= height && height <= 140.5f && !bgmChanged)
		{
			//클립 교체 및 플레이
			ChangeBgmClip();
		}
		else if (height > 140.5f && height <= 145.5f)
		{
			//볼륨 업
			bgmChanged = false;
			bgm.volume = ((height - 140.5f) * 0.2f) * VoiceManager.Instance.audioClips[(int)VoiceManager.Clip.Mountain].volume;
		}
	}

	private void MountainToSpace()
	{
		if (height >= 239.5f && height < 244.5f)
		{
			//볼륨 다운
			bgmChanged = false;
			bgm.volume = (1 - (height - 239.5f) * 0.2f) * VoiceManager.Instance.audioClips[(int)VoiceManager.Clip.Mountain].volume;
		}
		else if (244.5f <= height && height <= 254.5f && !bgmChanged)
		{
			//클립 교체 및 플레이
			ChangeBgmClip();
		}
		else if (254.5f <= height && height <= 256.5f)
		{
			//볼륨 업
			bgmChanged = false;
			bgm.volume = ((height - 254.5f) * 0.5f) * VoiceManager.Instance.audioClips[(int)VoiceManager.Clip.Space].volume;
		}
	}

	private void Start()
    {
		//bgm = GameManager.Instance.BGM;
		ragdoll = GameManager.Instance.ragdollController;
	}
}
