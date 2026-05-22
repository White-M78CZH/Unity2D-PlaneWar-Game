using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //首先做成一个单例模式
    private static AudioManager _instance;
    //再提供一个获取的get属性
    public static AudioManager Instance 
    {
        get 
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<AudioManager>();
            }
            return _instance;
        }
    }

    //添加AudioSource组件控制音效播放
    private AudioSource audioSource;
    //获取音频源文件
    public AudioClip buttonClip;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayButtonClip()
    {
        audioSource.PlayOneShot(buttonClip, 0.1f);
    }
}
