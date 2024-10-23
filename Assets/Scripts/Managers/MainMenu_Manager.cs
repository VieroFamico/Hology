using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu_Manager : MonoBehaviour
{
    public GameObject mainMenu;
    public Button startGame;
    public Button quitGame;

    private CanvasGroup mainMenuCanvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        mainMenuCanvasGroup = mainMenu.GetComponent<CanvasGroup>();

        mainMenu.SetActive(true);

        startGame.onClick.AddListener(StartGame);
        quitGame.onClick.AddListener(QuitGame);
    }

    public void StartGame()
    {
        General_Game_Manager.instance.Pre_Initialize();
        StartCoroutine(StartProcess());
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
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
