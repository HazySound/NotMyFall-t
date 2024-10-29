using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VoiceManager : MonoBehaviour
{
    [System.Serializable]
    public struct ClipInfo
    {
        public string key;
        public AudioClip clip;
        public float volume;
    }

    public enum Clip
    {
        Falling,
        Lab,
        City,
        Mountain,
        Space,
        Credit
    }

    public ClipInfo[] audioClips;

    public Clip currentMap;
   // [HideInInspector]
    public Clip hitMap;

    private static VoiceManager instance = null;
    [HideInInspector]
    public AudioSource audio;
    [SerializeField]
    private AudioSource fallingAudio;
    private WaitForSeconds waitOne = new WaitForSeconds(1f);
    private RagdollController ragdoll;

    [SerializeField]
    public AudioMixer MapMixer;

    private float downVolume;

    [HideInInspector]
    public bool isChangingClip = false;
    [HideInInspector]
    public bool playVoice = false;
    [HideInInspector]
    public string subtitleKey;
    private bool fallIsOn = false;

    [SerializeField]
    private AudioSource[] BGMSoruce;
    

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static VoiceManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public void PlayVoice(string key)
    {
        if (SubtitleManager.Instance.CheckIsLeft(key))//대사가 아직 남아 있으면
        {
            //보이스 플레이
            foreach (ClipInfo clipInfo in audioClips)
            {
                if(clipInfo.key == key)
                {
                    audio.clip = clipInfo.clip;
                    audio.volume = clipInfo.volume;
                    break;
                }
            }
            float volume;
            if(MapMixer.GetFloat("Map", out volume))
            {
                downVolume = 4f;
            }
            MapMixer.SetFloat("Map", volume - downVolume);
            audio.Play();

            StartCoroutine(ShowSubtitle(audio.clip.length, key));
        }
        playVoice = false;
    }

    public IEnumerator ShowSubtitle(float waitTime, string key)
    {
        //대사 출력
        SubtitleManager.Instance.SetText(key);
        SubtitleManager.Instance.ShowText();
        yield return new WaitForSecondsRealtime(waitTime);
        float volume;
        if (MapMixer.GetFloat("Map", out volume))
        {
            MapMixer.SetFloat("Map", volume + downVolume);
        }
        audio.volume = 0f;
        downVolume = 0f;
        SubtitleManager.Instance.HideText();
    }

    public void StartFallingMusic()
    {
        if(!fallIsOn)
        {
            fallIsOn = true;
            StartCoroutine(nameof(FallingMusic));
        }
    }

    public IEnumerator FallingMusic()
    {
        if (hitMap != Clip.Lab)
        {
            //SoundFadeOut(bgmAudio, 0.02f);
            StartCoroutine(SoundFadeOut(MapMixer));
            yield return waitOne;
            //SoundFadeIn(fallingAudio, Clip.Falling);
            StartCoroutine(SoundFadeIn(fallingAudio, Clip.Falling));
        }
    }

    public IEnumerator BackToBGM()
    {
        if(hitMap != Clip.Lab)
        {
            StartCoroutine(SoundFadeOut(fallingAudio, 0.005f));
            yield return waitOne;
            if(hitMap == currentMap)
            {
                //SoundFadeIn(bgmAudio);
                StartCoroutine(SoundFadeIn(MapMixer, false));
            }
            else
            {
                //SoundFadeIn(bgmAudio, currentMap);
                StartCoroutine(SoundFadeIn(MapMixer, true));
            }
        }
        //혹시몰름
        hitMap = currentMap;
        yield return waitOne;
        if (playVoice)//음성 출력해야 할 경우 플레이
        {
            PlayVoice(subtitleKey);
        }
        fallIsOn = false;
    }

    public IEnumerator StartCreditMusic()
    {
        StartCoroutine(SoundFadeOut(MapMixer, 0.4f));
        yield return waitOne;
        StartCoroutine(SoundFadeIn(fallingAudio, Clip.Credit));
    }

    public void EndCreditMusic()
    {
        StartCoroutine(SoundFadeOut(fallingAudio, 0.005f));
    }

    private IEnumerator SoundFadeOut(AudioSource audio, float step)
    {
        while (audio.volume > 0)
        {
            audio.volume = audio.volume >= step ? audio.volume - step : 0f;
            yield return null;
        }
        if(audio == fallingAudio)
        {
            audio.Stop();
        }
    }

    private IEnumerator SoundFadeOut(AudioMixer audio, float step)
    {
        float volume;
        while (audio.GetFloat("Map", out volume) && volume > -50f)
        {
            volume = volume > -50f ? volume - step : -50f;
            audio.SetFloat("Map", volume);
            yield return null;
        }
    }

    private IEnumerator SoundFadeOut(AudioMixer audio)
    {
        float volume;
        while (audio.GetFloat("Map", out volume) && volume > -50f)
        {
            volume = volume > -50f ? volume - 0.5f : -50f;
            audio.SetFloat("Map", volume);
            yield return null;
        }
    }

    private IEnumerator SoundFadeIn(AudioMixer audio, bool replay)
    {
        if (replay)
        {
            BGMSoruce[(int)currentMap - 1].Stop();
            BGMSoruce[(int)currentMap - 1].Play();
        }
        float volume;
        while (audio.GetFloat("Map", out volume) && volume < 0f)
        {
            volume = volume < 0f ? volume + 1f : 0f;
            audio.SetFloat("Map", volume);
            yield return null;
        }
    }


    private IEnumerator SoundFadeIn(AudioSource audio, Clip index)
    {
        audio.clip = audioClips[(int)index].clip;
        audio.Play();
        if (!isChangingClip)
        {
            audio.volume = 0f;
            float targetVolume = audioClips[(int)index].volume - downVolume;
            while (audio.volume < targetVolume)
            {
                audio.volume = audio.volume <= targetVolume - 0.01f ? audio.volume + 0.01f : targetVolume;
                yield return null;
            }
        }
    }

    private IEnumerator SoundFadeIn(AudioSource audio)
    {
        if (!isChangingClip)
        {
            audio.volume = 0f;
            float targetVolume = audioClips[(int)currentMap].volume - downVolume;
            while (audio.volume < targetVolume)
            {
                audio.volume = audio.volume <= targetVolume - 0.01f ? audio.volume + 0.01f : targetVolume;
                yield return null;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //currentMap = Clip.Lab;
        //hitMap = Clip.Lab;
        ragdoll = GameManager.Instance.ragdollController;
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
