using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        UnityEngine.Cursor.visible = true;
        Time.timeScale = 1;
    }
    //Functions for the buttons on the Main Menu UI
    public void PlayGame()
    {
        if (FindObjectOfType<GameManager>() != null) Destroy(FindObjectOfType<GameManager>().gameObject);
        UnityEngine.Cursor.visible = false;
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        //Debug.Log("Quit!");
        Application.Quit();
    }

    /*IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            Debug.Log(progress);
            yield return null;
        }
    }*/
}
