using System.Collections;
using System.Collections.Generic;//需要List来管理爱心
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance==null)
            {
                _instance = FindAnyObjectByType<UIManager>();
            }
            return _instance;
        }
    }

    //分数与炸弹显示
    public TextMeshProUGUI scoreTMP;
    public TextMeshProUGUI bombCountTMP;

    //暂停与开始按钮
    public Button pauseButton;
    public Button resumeButton;

    //游戏结束面板、历史最高、本局分数显示
    public GameObject gameOverPanel;
    public TextMeshProUGUI bestScoreTMP;
    public TextMeshProUGUI currentScoreTMP;

    //获取结束界面的重新开始按钮与退出按钮
    public Button restartButton;
    public Button quitButton;

    //血量显示的爱心预制体
    public GameObject heartUIPrefab;
    //爱心的父容器
    public Transform heartContainer;
    //存储当前显示的爱心
    private List<GameObject> currentHearts=new List<GameObject>();
    //最多显示5颗爱心
    private int maxHealth = 5;

    private void Start()
    {
        //再调用之前先进行移除，避免被多次调用
        pauseButton.onClick.RemoveListener(this.OnPauseButtonClick);
        resumeButton.onClick.RemoveListener(this.OnResumeButtonClick);
        //给按钮的点击加上监听事件
        pauseButton.onClick.AddListener(this.OnPauseButtonClick);
        resumeButton.onClick.AddListener (this.OnResumeButtonClick);

        //同样的调用之前先移除，避免被多次调用
        restartButton.onClick.RemoveListener(this.OnRestartButtonClick);
        quitButton.onClick.RemoveListener(this.OnQuitButtonClick);
        //结束界面的重新开始与退出按钮也要记得加入监听器
        restartButton.onClick.AddListener(this.OnRestartButtonClick);
        quitButton.onClick.AddListener(this.OnQuitButtonClick);

        //初始化血量UI
        UpdateHealthUI(3);
    }

    //更新血量显示方法
    public void UpdateHealthUI(int currentHealth)
    {
        //第一步：先清空所有现有爱心
        foreach (GameObject heart in currentHearts)
        {
            Destroy(heart);
        }
        currentHearts.Clear();

        //强制限制血量范围(0-5)
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        //第三步：生成当前血量对应的爱心数量
        for (int i=0;i<currentHealth;i++)
        {
            GameObject heart = Instantiate(heartUIPrefab, heartContainer);
            heart.name = $"Heart_{i}";
            //强制设置爱心的宽高为100x80，覆盖布局的任何修改
            RectTransform heartRect=heart.GetComponent<RectTransform>();
            heartRect.anchorMin = new Vector2(0.5f, 0.5f);
            heartRect.anchorMax = new Vector2(0.5f, 0.5f);
            heartRect.sizeDelta = new Vector2(100, 80);
            heartRect.localScale = Vector3.one;
            currentHearts.Add(heart);
        }
    }

    public void UpdateScoreUI(int score)
    {
        this.scoreTMP.text = score + "";
    }
    public void UpdateBombCountUI(int count)
    {
        this.bombCountTMP.text = count + "";
    }

    //点击暂停事件
    void OnPauseButtonClick()
    {
        pauseButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(true);
        GameManager.Instance.PauseGame();
        AudioManager.Instance.PlayButtonClip();
    }
    //点击继续事件
    void OnResumeButtonClick()
    {
        resumeButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
        GameManager.Instance.ResumeGame();
        AudioManager.Instance.PlayButtonClip();
    }
    //游戏结束面板显示
    public void ShowGameOverPanel(int bestScore,int currentScore)
    { 
        //展示结束面板
        gameOverPanel.SetActive(true);
        //更新最高得分
        this.bestScoreTMP.text = bestScore + "";
        //更新本局得分
        this.currentScoreTMP.text = currentScore + "";
    }
    //重新开始按钮的点击
    void OnRestartButtonClick()
    { 
        GameManager.Instance.RestartGame();
        AudioManager.Instance.PlayButtonClip();
    }
    //退出按钮的点击
    void OnQuitButtonClick()
    { 
        GameManager.Instance.QuitGame();
        AudioManager.Instance.PlayButtonClip();
    }

}
