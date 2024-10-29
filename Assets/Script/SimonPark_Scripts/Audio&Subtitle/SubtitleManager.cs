using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class TextDict : SerializableDictionary<string, string> {}

public class SubtitleManager : MonoBehaviour
{
    private static SubtitleManager instance = null;

    [SerializeField]
    private TextMeshProUGUI subtitle;
    private WaitForSeconds wait = new WaitForSeconds(2f);

    private string currentKey;

    
    [SerializeField]
    private TextDict textData;

    public void ShowText()
    {
        StartCoroutine(FadeInText());
    }

    public void HideText()
    {
        StartCoroutine(FadeOutText());
    }

    public IEnumerator FadeOutText()
    {
        Color textColor = subtitle.color;
        while (textColor.a > 0f)
        {
            textColor.a -= 0.01f;
            subtitle.color = textColor;
            yield return null;
        }
        yield return wait;
        subtitle.gameObject.SetActive(false);
        RemoveData();
    }

    public IEnumerator FadeInText()
    {
        subtitle.gameObject.SetActive(true);
        Color textColor = subtitle.color;
        while (textColor.a < 1f)
        {
            textColor.a += 0.01f;
            subtitle.color = textColor;
            yield return null;
        }
    }

    public void RemoveData()
    {
        textData.Remove(currentKey);
    }

    public bool CheckIsLeft(string key)
    {
        return textData.ContainsKey(key);
    }

    public void SetText(string key)
    {
        currentKey = key;
        subtitle.text = textData[key].Replace("\\n", "\n");
    }

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

    public static SubtitleManager Instance
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
}
