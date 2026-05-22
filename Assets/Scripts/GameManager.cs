using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//定义一个枚举类型专门保存游戏的状态
public enum GameState
{ 
    Playing,
    Pause,
    GameOver
}

public class GameManager : MonoBehaviour
{
    //私有的静态的实例
    private static GameManager _instance;

    //访问的入口
    public static GameManager Instance
    {
        get 
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<GameManager>();//根据类型查找组件并赋值
            }
            return _instance;
        }
    }

    //存储炸弹个数
    private int bombCount = 0;
    //分数
    private int score = 0;

    //游戏正在运行时的状态
    private GameState gameState = GameState.Playing;//初始化是运行状态

    //定义一个时间间隔运用于判断双击事件
    private float doubleClickThreshold = 0.2f;
    //保存上一次点击的时间
    private float lastClickTime = 0;

    //对使用炸弹音效的引用
    public AudioSource useBombAudio;

    //对游戏结束音效的引用
    public AudioSource gameOverAudio;

    // Start is called before the first frame update
    void Start()
    {
        //游戏结束时调用了暂停，所以如果是重新开始就要调用继续
        ResumeGame();
    }

    // Update is called once per frame
    void Update()
    {
        UseBombUpdate();
    }
    //增加炸弹数量
    public void AddBomb()
    { 
        bombCount++;
        UIManager.Instance.UpdateBombCountUI(bombCount);
    }
    //减少炸弹数量
    public void SubBomb()
    {
        bombCount--;
        UIManager.Instance.UpdateBombCountUI(bombCount);
    }
    //增加分数
    public void AddScore(int count)
    {
        this.score += count;
        UIManager.Instance.UpdateScoreUI(score);
    }
    //暂停游戏
    public void PauseGame()
    {
        Time.timeScale = 0;
        gameState = GameState.Pause;
    }
    //继续游戏
    public void ResumeGame()
    {
        Time.timeScale = 1;
        gameState = GameState.Playing;
    }

    //定义一个方法让外界能够获得游戏是否暂停
    public bool IsPause()
    { 
        return gameState==GameState.Pause;
    }
    //使用炸弹的方法
    void UseBombUpdate()
    {
        //检测双击事件
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastClickTime < doubleClickThreshold)
            {
                //双击处理
                //Debug.Log("DoubleClick");
                //仅在游戏运行中可使用炸弹
                if (gameState == GameState.Playing)
                {
                    UseBomb();
                }
            }
            else
            {
                lastClickTime = Time.time;
            }
        }
    }
    void UseBomb()
    {
        //如果炸弹小于等于0则无事发生
        if (this.bombCount <= 0) return;

        //如果炸弹大于0，则直接调用炸弹减少方法
        SubBomb();

        //炸弹的使用，即消灭掉屏幕上所有的敌机
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);//获取屏幕上所有敌机

        foreach (Enemy e in enemies)
        {
            e.TakeDamage(99999);
        }
        //调用使用炸弹的音效
        useBombAudio.Play();
    }
    //游戏结束处理
    public void GameOver()
    {
        //保证GameOver在游戏结束时只调用一遍
        if (gameState == GameState.GameOver) return;

        //游戏结束时先暂停
        PauseGame();

        //状态设置为游戏结束
        gameState = GameState.GameOver;

        //游戏结束音效的调用
        gameOverAudio.Play();

        //获取最高分
        int bestScore = PlayerPrefs.GetInt("BestScore",0);
        UIManager.Instance.ShowGameOverPanel(bestScore,score);
        //如果当前得分超过历史最高分,保存覆盖
        if (score>bestScore)
        {
            PlayerPrefs.SetInt("BestScore", score);
        }
    }

    //重新开始游戏
    public void RestartGame()
    {
        //重新加载场景即可
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //退出游戏
    public void QuitGame()
    { 
        //在Unity中不起作用，只有发布之后才有用
        Application.Quit();
    }
}
