using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene_Manager : MonoBehaviour
{
    
    void Start()
    {
        StartCoroutine(EndingScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator EndingScene()
    {
        yield return new WaitForSeconds(20f);

        SceneManager.LoadSceneAsync(0);
    }
}
