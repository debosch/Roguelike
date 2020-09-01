using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameBehavior : MonoBehaviour
{
    [SerializeField] private BoardCreator boardCreator;
    [SerializeField] private Button restartButton;
    [HideInInspector] public bool playersTurn = true;
    
    public static GameBehavior Instance = null;
    public int playerFoodPoints = 100;
    public float turnDelay = .1f;
    public bool doingSetup;
    
    private float levelStartDelay = 2f;
    private Text levelText;
    private GameObject levelImage;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private int level = 1;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
            
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardCreator = GetComponent<BoardCreator>();
        InitGame();
    }

    private void OnLevelWasLoaded(int index)
    {
        level++;
        
        InitGame();
    }

    private void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved";
        levelImage.SetActive(true);
        restartButton.gameObject.SetActive(true);
        enabled = false;
    }

    private void InitGame()
    {
        doingSetup = true;
        restartButton = GameObject.Find("RestartButton").GetComponent<Button>();
        restartButton.gameObject.SetActive(false);
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke(nameof(HideLevelImage), levelStartDelay);
        
        enemies.Clear();
        boardCreator.SetupScene(level);
        
        restartButton.onClick.AddListener(Restart);
    }

    private void Restart()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
            yield return new WaitForSeconds(turnDelay);

        foreach (var enemy in enemies)
        {
            enemy.MoveEnemy();
            yield return new WaitForSeconds(enemy.moveTime);
        }

        playersTurn = true;

        enemiesMoving = false;
    }

 
}
