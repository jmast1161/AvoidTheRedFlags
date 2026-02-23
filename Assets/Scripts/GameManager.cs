//using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public enum GameState
{
    WaitingGameplayInput,
    StartGame
}

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Tile tile;
    
    [SerializeField]
    private Flag flag;

    [SerializeField]
    private Player player;

    private bool spawnFlag = true;

    [SerializeField]
    private UnityEngine.UI.Button startButton;

    [SerializeField]
    private TMPro.TMP_Text scoreText;

    [SerializeField]
    private int movingObjectSpeed;

    [SerializeField]
    private List<Flag> flags = new List<Flag>();

    [SerializeField]
    private List<Tile> tiles = new List<Tile>();

    [SerializeField]
    private AudioSource music;

    [SerializeField]
    private AudioSource flagDestroySound;

    [SerializeField]
    private AudioSource flagHitSound;

    private int score = 0;

    private bool alwaysSpawnFlag = false;

    private bool spawnTwoFlags = false;

    private readonly Dictionary<int, Vector3> flagSpawnPositions = new Dictionary<int, Vector3>
    {
        {0, new Vector3(0, 0, 195)},
        {1, new Vector3(7.5f, 0, 195)},
        {2, new Vector3(-7.5f, 0, 195)},
        {3, new Vector3(3.75f, 0, 195)},
        {4, new Vector3(-3.75f, 0, 195)}
    };

    private readonly Dictionary<int, Vector3> secondFlagSpawnPositions = new Dictionary<int, Vector3>
    {
        {0, new Vector3(0, 0, 210)},
        {1, new Vector3(7.5f, 0, 210)},
        {2, new Vector3(-7.5f, 0, 210)},
        {3, new Vector3(3.75f, 0, 210)},
        {4, new Vector3(-3.75f, 0, 210)}
    };

    private GameState gameState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        gameState = GameState.StartGame;
        
        player.TriggerNewTile += OnTriggerNewTile;     
        foreach(var flag in flags)
        {
            flag.DestroyFlag += OnDestroyFlag;
            flag.GameOver += OnGameOver;
        }

        foreach(var tile in tiles)
        {
            tile.DestroyTile += OnDestroyTile;
        }
        
        startButton.onClick.AddListener(OnStartButtonClicked);
        music.Play();
    }

    // Update is called once per frame
    private void Update()
    {
        switch(gameState)
        {
            case GameState.WaitingGameplayInput:
                player.MovePlayer();
                foreach(var flag in flags)
                {
                    flag.MoveFlag(movingObjectSpeed);
                }

                foreach(var tile in tiles)
                {
                    tile.MoveTile(movingObjectSpeed);
                }

                break;
            case GameState.StartGame:
                break;
        }
        
    }

    private void SpawnFlag(Dictionary<int, Vector3> flagPositions)
    {
        var flagSpawnPosition = Random.Range(0, 5);
        var newFlag = Instantiate(flag, flagPositions[flagSpawnPosition], Quaternion.Euler(0, 90, 0));
        newFlag.DestroyFlag += OnDestroyFlag;
        newFlag.GameOver += OnGameOver;
        flags.Add(newFlag);   
    }
    
    private void OnTriggerNewTile(Player player)
    {
        var lastTile = tiles.Last();
        var newTile = Instantiate(
            tile, 
            new Vector3(
                lastTile.transform.position.x, 
                lastTile.transform.position.y, 
                lastTile.transform.position.z + 30), 
            Quaternion.identity);
        newTile.DestroyTile += OnDestroyTile;
        newTile.MoveTile(movingObjectSpeed);
        tiles.Add(newTile);
        if (spawnFlag || alwaysSpawnFlag)
        {
            SpawnFlag(flagSpawnPositions);
            if (spawnTwoFlags)
            {
                SpawnFlag(secondFlagSpawnPositions);
            }
        }

        spawnFlag = !spawnFlag;
    }

    private void IncreaseSpeed()
    {        
        if (movingObjectSpeed > -150)
        {
            movingObjectSpeed -= 1;
        }
    }

    private void OnDestroyFlag(Flag flag)
    {
        flags.Remove(flag);
        ++score;

        if (score > 25)
        {
            alwaysSpawnFlag = true;
        }

        if (score > 50)
        {
            spawnTwoFlags = true;
        }

        scoreText.text = score.ToString();
        flagDestroySound.Play();
        IncreaseSpeed();
    }
    
    private void OnDestroyTile(Tile tile) =>
        tiles.Remove(tile);

    private void OnGameOver(Flag flag)
    {
        gameState = GameState.StartGame;        
        startButton.gameObject.SetActive(true);
        flagHitSound.Play();
    }

    private void OnStartButtonClicked()
    {
        gameState = GameState.WaitingGameplayInput;
        startButton.gameObject.SetActive(false);
        score = 0;
        scoreText.text = score.ToString();
        player.InitializePlayer();
        movingObjectSpeed = -50;
        alwaysSpawnFlag = false;
        spawnTwoFlags = false;
        foreach (var flag in flags)
        {
            Destroy(flag.gameObject);
        }

        flags.Clear();
    }
}
