using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Robot_Manager : MonoBehaviour
{
    [Header("Flood")]
    public Button Flood_RequestButton;
    public Button Flood_InfoButton;
    public Button Flood_InfoCloseButton;
    public GameObject Flood_Info;

    [Header("Tanah Longsor")]
    public Button TanLong_RequestButton;
    public Button TanLong_InfoButton;
    public Button TanLong_InfoCloseButton;
    public GameObject TanLong_Info;

    [Header("TanLong Game Reference")]
    public GameObject TanLong_GameView;
    public GameObject TanLong_GameCanvas;
    public Tanah_Longsor_Manager LongsorManager;

    // Start is called before the first frame update
    void Start()
    {
        TanLong_Info.gameObject.SetActive(false);
        Flood_Info.gameObject.SetActive(false);

        //TanLong_RequestButton.enabled = false;
        Flood_RequestButton.enabled = false;

        TanLong_RequestButton.onClick.AddListener(StartTanLong_MiniGame);

        TanLong_InfoButton.onClick.AddListener(OpenTanLongInfo);
        TanLong_InfoCloseButton.onClick.AddListener(CloseTanLongInfo);

        Flood_InfoButton.onClick.AddListener(OpenFloodInfo);
        Flood_InfoCloseButton.onClick.AddListener(CloseFloodInfo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateRequest(int i)
    {
        if(i == 0)
        {
            Flood_RequestButton.enabled = true;
        }
        else if (i == 1)
        {
            TanLong_RequestButton.enabled = true;
        }
        else
        {
            Debug.Log("Request Out of Index");
        }
    }

    public void StartTanLong_MiniGame()
    {
        TanLong_GameView.SetActive(true);
        TanLong_GameCanvas.SetActive(true);

        LongsorManager.StartGame();
    }

    private void OpenTanLongInfo()
    {
        TanLong_Info.SetActive(true);
    }

    private void CloseTanLongInfo()
    {
        TanLong_Info.SetActive(false);
    }

    private void OpenFloodInfo()
    {
        Flood_Info.SetActive(true);
    }

    private void CloseFloodInfo()
    {
        Flood_Info.SetActive(false);
    }
}
