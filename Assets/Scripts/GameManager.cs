using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // 获取预制件
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject exitPrefab;

    // 获取根物体
    public Transform rootTransform;

    // 游戏关卡和难度控制
    public int currentLevel = 1;
    public float levelDifficultyMultiplier = 1.2f;

    // 分数
    private int score = 0;

    // 边界
    public Vector2 mapBounds = new Vector2(10f, 10f);

    // 生成的敌人和障碍数量
    private int numberOfObstacles = 10;
    private int numberOfEnemies = 20;

    // 计时器
    private float startingTime = 60f;
    public float remainingTime;

    // 事件定义
    public event Action<int> OnScoreChanged; // 分数变化事件
    public event Action<float> OnTimeChanged; // 时间变化事件
    public event Action<int> OnLevelChanged; // 关卡变化事件

    private void Awake()
    {
        Instance = this;
        remainingTime = startingTime;
    }

    private void Start()
    {
        InitializeGame();
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        remainingTime -= Time.deltaTime;

        OnTimeChanged?.Invoke(remainingTime);

        if (remainingTime <= 0)
        {
            RestartGame();
        }
    }

    private void InitializeGame()
    {
        ClearRootObjects();

        for (int i = 0; i < numberOfObstacles; i++)
        {
            GameObject obstacle = SpawnObjectAtRandomPosition(obstaclePrefab);
            obstacle.transform.SetParent(rootTransform);
        }

        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject enemy = SpawnObjectAtRandomPosition(enemyPrefab);
            enemy.transform.SetParent(rootTransform);
        }

        GameObject exit = SpawnObjectAtRandomPosition(exitPrefab);
        exit.transform.SetParent(rootTransform);
    }

    private GameObject SpawnObjectAtRandomPosition(GameObject prefab)
    {
        Vector3 randomPosition = new Vector3(
            UnityEngine.Random.Range(-mapBounds.x, mapBounds.x),
            UnityEngine.Random.Range(-mapBounds.y, mapBounds.y),
            0f
        );
        return Instantiate(prefab, randomPosition, Quaternion.identity);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddScore(int points)
    {
        score += points;

        // 触发分数变化事件
        OnScoreChanged?.Invoke(score);
    }

    public void NextLevel()
    {
        AddScore(currentLevel * 10);
        ClearRootObjects();

        currentLevel++;
        numberOfObstacles = Mathf.RoundToInt(numberOfObstacles * levelDifficultyMultiplier);
        numberOfEnemies = Mathf.RoundToInt(numberOfEnemies * levelDifficultyMultiplier);

        OnLevelChanged?.Invoke(currentLevel);

        InitializeGame();

        Debug.Log("Level " + currentLevel + " started!");
    }

    private void ClearRootObjects()
    {
        foreach (Transform child in rootTransform)
        {
            Destroy(child.gameObject);
        }
    }
}


