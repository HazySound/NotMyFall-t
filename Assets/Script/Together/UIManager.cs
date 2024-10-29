using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
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
    public static UIManager Instance
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

    public Portrait portrait;
    public BoosterUI boosterUI;
    public HeightCheck heightcheck;
    public GameObject BodyUI;
    public GameObject HUD;
    public GameObject EndingTitle;
    public GameObject Credit;
}
