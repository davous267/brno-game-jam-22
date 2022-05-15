using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField]
    GameObject creditsMenu;

    [SerializeField]
    GameObject mainMenu;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Credits()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenuScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
