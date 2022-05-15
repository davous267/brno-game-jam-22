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
    private EnemySpawner enemySpawner;

    [SerializeField]
    private GameObject gameOverPanel;

    [SerializeField]
    private GameObject gameWonPanel;

    public Player Player => player;

    public EnemySpawner EnemySpawner => enemySpawner;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            gameOverPanel.SetActive(false);
            gameWonPanel.SetActive(false);
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
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void GameWon()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        if (gameWonPanel != null)
        {
            gameWonPanel.SetActive(true);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
