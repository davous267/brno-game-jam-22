using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; } = null;

    [SerializeField]
    private Player player;

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
        } 
        else
        {
            Destroy(gameObject);
        }
    }
}
