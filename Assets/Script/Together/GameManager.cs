using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class GameManager : MonoBehaviourPunCallbacks
{   
    private static GameManager instance = null;

    public RagdollController ragdollController;
    public GameObject hips;
    public CinemachineBrain maincamera;
    public Texture2D GameCusor;
    public JetController jetCon;
    public Transform eagleTarget;

    public bool isEnding = false;
    public bool isPause = false;

    [HideInInspector]
    public int fallCount = 0;

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
    private void Start()
    {
        //Cursor.visible = false;
        Cursor.SetCursor(GameCusor,Vector2.zero,CursorMode.Auto);
    }
    public static GameManager Instance
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
