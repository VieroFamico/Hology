using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class General_Game_Manager : MonoBehaviour
{
    public static General_Game_Manager instance;

    [Header("Reference")]
    public Day_Manager day_manager;

    [Header("UI References")]
    public List<GameObject> tutorialPanels;
    public Button closeTutorial;
    public GameObject pauseMenu;
    private int currTutorialIndex;

    [Header("Wake Up and Sleep Animator")]
    private Camera_Manager cameraManager;
    private Animator cameraAnimator;

    [Header("Environment Variables")]
    public Light regularLight;
    public Light emergencyLight;

    [Header("Mini-Games to Solve")]
    public GameObject randomLocToSpawnDisaster;
    public bool isInMiniGames = false;
    public List<GameObject> activeDisaster = new List<GameObject>();

    public BanSos_Game_Manager banSosGameManager;
    public Tanah_Longsor_Manager tanahLongsorManager;

    public bool banSosIsCompleted;
    public bool tanahLongsorIsCompleted;

    [Header("Day Has Started")]
    public bool dayHasStarted;

    [Header("Difficulty Variables")]
    public float miniGames_TimeLimitDecrease_PerDay = 3f; // in seconds
    public float flood_WaveSpeedIncrease_PerDay = 1f;

    private bool hasDoneBeginnerTutorial = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        cameraManager = Camera_Manager.instance;
        cameraAnimator = cameraManager.virtualCamera.GetComponent<Animator>();

        closeTutorial.onClick.AddListener(CloseTutorial);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Pre_Initialize()
    {
        cameraAnimator.SetTrigger("StartDay");

        StartCoroutine(DisableAnimatorAfter(1.5f));
    }

    private IEnumerator DisableAnimatorAfter(float time)
    {
        yield return new WaitForSeconds(time);

        cameraAnimator.enabled = false;

        Tutorial();
    }

    private void Tutorial()
    {
        if (hasDoneBeginnerTutorial)
        {
            Initialize();
            return;
        }
        else
        {
            currTutorialIndex = 0;
            ShowAllTutorial();
            hasDoneBeginnerTutorial = true;
        }
    }
    private void ShowAllTutorial()
    {
        foreach(GameObject tutorial in tutorialPanels)
        {
            tutorial.gameObject.SetActive(true);
        }
        closeTutorial.gameObject.SetActive(true);
    }

    private void CloseTutorial()
    {
        if(currTutorialIndex >= tutorialPanels.Count)
        {
            return;
        }
        tutorialPanels[currTutorialIndex].SetActive(false);

        currTutorialIndex += 1;

        if(currTutorialIndex >= tutorialPanels.Count)
        {
            closeTutorial.gameObject.SetActive(false);
            Initialize();
        }
    }

    public void Initialize()
    {
        dayHasStarted = false;
        day_manager.currDay = 1;
        day_manager.Initialize();
    }

    public void CompleteBanSos()
    {
        banSosIsCompleted = true;

        if (tanahLongsorIsCompleted)
        {
            GoToEndScene();
        }
    }

    public void CompleteTanahLongsor()
    {
        tanahLongsorIsCompleted = true;

        if (banSosIsCompleted)
        {
            GoToEndScene();
        }
    }

    public void GoToEndScene()
    {
        SceneManager.LoadSceneAsync(1);
    }

}
