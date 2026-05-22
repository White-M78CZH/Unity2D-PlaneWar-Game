using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //首先要持有对bullet的prefab的引用
    public GameObject bulletPrefab;

    //子弹的生成速率
    public float spawnRate = 1;//一秒生成一个子弹

    //计时器
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;//使用这个才会让子弹生成的速度不受帧率影响，而是随现实的真实时间

        //判断计时器是否到达了一个子弹生成所需要的时间
        if (timer >= 1 / spawnRate)
        {
            timer -= 1 / spawnRate;
            SpawnBullet();
        }
    }

    //定义生成子弹的方法
    void SpawnBullet() 
    { 
        Instantiate(bulletPrefab,transform.position,transform.rotation);
    }
}
