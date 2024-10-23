using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu_Manager : MonoBehaviour
{
    public GameObject mainMenu;
    public Button startGame;
    public Button quitGame;

    public GameObject settingPanel;
    public Button closeSetting;
    public Button returnToMainMenu;

    private CanvasGroup mainMenuCanvasGroup;

    private bool isInMainMenu = true;
    private bool isInSetting = false;
    // Start is called before the first frame update
    void Start()
    {
        mainMenuCanvasGroup = mainMenu.GetComponent<CanvasGroup>();

        mainMenu.SetActive(true);

        startGame.onClick.AddListener(StartGame);
        quitGame.onClick.AddListener(QuitGame);

        closeSetting.onClick.AddListener(CloseSetting);
        returnToMainMenu.onClick.AddListener(Restart);
    }

    public void StartGame()
    {
        General_Game_Manager.instance.Pre_Initialize();
        StartCoroutine(StartProcess());
    }

    private void Update()
    {
        if (isInMainMenu)
        {
            return;
        }
        else
        {
            if(Input.GetKeyUp(KeyCode.Escape) && !isInSetting)
            {
                OpenSetting();
            }
        }
    }

    private void OpenSetting()
    {
        settingPanel.SetActive(true);
        isInSetting = true;
    }

    private void CloseSetting()
    {
        settingPanel.SetActive(false);
        isInSetting = false;
    }

    private void Restart()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator StartProcess()
    {
        float timeNeeded = 1f;
        float timer = 0f;

        while (mainMenuCanvasGroup.alpha > 0f)
        {
            mainMenuCanvasGroup.alpha = Mathf.Lerp(mainMenuCanvasGroup.alpha, 0f, timer / timeNeeded);

            timer += Time.deltaTime;
            yield return null;
        }

        mainMenuCanvasGroup.alpha = 0;
        mainMenuCanvasGroup.interactable = false;
        mainMenuCanvasGroup.blocksRaycasts = false;

        isInMainMenu = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
