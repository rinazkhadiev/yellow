using UnityEngine;
using UnityEngine.UI;
using System;

public class AllObjects : MonoBehaviour
{
    public static AllObjects Singleton { get; private set; }

    [Header("Another")]

    public JoystickController JoyController;
    public AnalyticsEvent AnalyticsEvent;
    public Review ReviewScript;
    public Part PartManager;
    public int PartNumber;
    public bool IsPause;
    public bool CharacterIsBusy;
    public GameObject VolumeGameObject;

    public SkinnedMeshRenderer[] BabyEyes;
    public Material[] EyesMaterials;

    public Transform BabySecondPartTransform;
    public Transform BabyThirdPartTransform;
    public Transform CharacterThirdStartPos;
    public bool BabySecondIsRun;
    public GameObject[] Doors;
    public Transform CharacterSecondPos;
    public Image RedVinetImage;

    public GameObject[] Steps;
    public GameObject Arrow;

    public GameObject WhatIsFloorDown;
    public GameObject WhatIsFloorUp;

    public Transform[] ThirdPartBabyPosition;
    public GameObject ThirdPartPacifier;

    [Header("UI")]

    public GameObject JoystickUI;
    public GameObject[] BabyButton;
    public GameObject[] BabyButton_Two;

    public GameObject SettingsMenu;
    public Slider SensitivityBar;



    public Text TimerText;

    public GameObject StartEngineButton;

    public GameObject FinishFirstImage;
    public GameObject FinishSecondImage;
    public GameObject FinishThirdImage;

    public GameObject FindAllToysButton;

    public GameObject ScreamImage;
    public Image FailImage;
    public GameObject FailMenu;
    public GameObject[] FailTexts;

    public Text CurrentFindText;

    public GameObject PacifierGiveButton;

    [Header("Audio")]

    public AudioSource MainSource;
    public AudioClip[] JumpClips;

    public AudioClip[] StepClips;
    public AudioClip[] StepClipsHouse;

    public AudioClip SeadDownClip;

    public AudioSource LoseAudio;
    public AudioClip TeleportSound;

    private void Awake()
    {
        Singleton = this;

        if (PlayerPrefs.HasKey("Part"))
        {
            PartNumber = PlayerPrefs.GetInt("Part");
        }
        else
        {
            PartNumber = 1;
        }
    }

    public void PauseAciton(bool pause)
    {
        if (pause)
        {
            IsPause = true;
        }
        else
        {
            IsPause = false;
        }
    }
}
