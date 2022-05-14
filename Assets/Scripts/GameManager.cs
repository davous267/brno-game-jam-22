using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
