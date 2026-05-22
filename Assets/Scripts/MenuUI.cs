using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    //开始按钮点击事件
    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("Game");
    }
    //退出按钮点击事件
    public void OnQuitButtonClick()
    { 
        Application.Quit();
    }
}
