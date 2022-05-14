using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; } = null;

    [SerializeField]
    private Player player;

    [SerializeField]
    private GameObject gameOverPanel;

    public Player Player
    {
        get => player;
        set => player = value;
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            gameOverPanel.SetActive(false);
            Time.timeScale = 1;
        } 
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        gameOverPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
