using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //敌机0配置
    public GameObject enemy0Prefab;//获取敌人0预制体
    public float enemy0BaseInterval = 2f;//初始生成间隔
    public float enemy0MinInterval = 0.8f;//最小间隔(防止过快)
    public float enemy0SpeedupRate = 0.02f;//每秒钟间隔减少的值（加速速率）
    private float currentEnemy0Interval;//实时更新的当前间隔

    //敌机1配置
    public GameObject enemy1Prefab;//获取敌人1预制体
    public float enemy1BaseInterval = 8f;//初始生成间隔
    public float enemy1MinInterval = 3f;//最小间隔(防止过快)
    public float enemy1SpeedupRate = 0.03f;//每秒钟间隔减少的值（加速速率）
    private float currentEnemy1Interval;//实时更新的当前间隔

    //敌机2配置
    public GameObject enemy2Prefab;//获取敌人2预制体
    public float enemy2BaseInterval = 15f;//初始生成间隔
    public float enemy2MinInterval = 5f;//最小间隔(防止过快)
    public float enemy2SpeedupRate = 0.05f;//每秒钟间隔减少的值（加速速率）
    private float currentEnemy2Interval;//实时更新的当前间隔

    //奖励配置,定义预制体和生成时间间隔
    public GameObject award1Prefab;
    public float award1SpawnInterval = 20;
    public GameObject award2Prefab;
    public float award2SpawnInterval = 40;

    //爱心奖励配置
    public GameObject awardHeartPrefab;
    public float awardHeartSpawnInterval = 30f;

    //记录游戏运行时间
    private float gameRunTime;

    // Start is called before the first frame update
    void Start()
    {
        //初始化当前间隔为基础间隔
        currentEnemy0Interval = enemy0BaseInterval;
        currentEnemy1Interval = enemy1BaseInterval;
        currentEnemy2Interval = enemy2BaseInterval;

        //启动协程替代InvokeRepeating(动态控制间隔)
        StartCoroutine(SpawnEnemy0Coroutine());
        StartCoroutine(SpawnEnemy1Coroutine());
        StartCoroutine(SpawnEnemy2Coroutine());

        //InvokeRepeating是每调用一次就永久重复执行
        InvokeRepeating("SpawnAward1", 4, award1SpawnInterval);
        InvokeRepeating("SpawnAward2", 7, award2SpawnInterval);
        InvokeRepeating("SpawnAwardHeart",10f,awardHeartSpawnInterval);
    }

    // Update is called once per frame
    void Update()//Update是每帧执行一次
    {
        //累计游戏运行时间
        gameRunTime += Time.deltaTime;
        //动态更新敌机生成间隔(每帧衰减，知道最小间隔)
        UpdateEnemySpawnInterval();
    }

    //动态更新所有敌机的生成间隔
    void UpdateEnemySpawnInterval()
    {
        //敌机0：间隔逐步减小，不低于最小值
        currentEnemy0Interval = Mathf.Max(
            enemy0BaseInterval-(gameRunTime*enemy0SpeedupRate),
            enemy0MinInterval
            );
        //敌机1：间隔逐步减小，不低于最小值
        currentEnemy1Interval = Mathf.Max(
            enemy1BaseInterval - (gameRunTime * enemy1SpeedupRate),
            enemy1MinInterval
            );
        //敌机2：间隔逐步减小，不低于最小值
        currentEnemy2Interval = Mathf.Max(
            enemy2BaseInterval - (gameRunTime * enemy2SpeedupRate),
            enemy2MinInterval
            );
    }

    //敌机0生成协程(循环生成，动态等待间隔)
    IEnumerator SpawnEnemy0Coroutine()
    {
        //初始等待
        yield return new WaitForSeconds(1f);

        while (true)
        { 
            SpawnEnemy0();//生成敌机
            yield return new WaitForSeconds(currentEnemy0Interval);//等待当前动态间隔
        }
    }

    //敌机1生成协程(循环生成，动态等待间隔)
    IEnumerator SpawnEnemy1Coroutine()
    {
        //初始等待
        yield return new WaitForSeconds(5f);

        while (true)
        {
            SpawnEnemy1();//生成敌机
            yield return new WaitForSeconds(currentEnemy1Interval);//等待当前动态间隔
        }
    }

    //敌机2生成协程(循环生成，动态等待间隔)
    IEnumerator SpawnEnemy2Coroutine()
    {
        //初始等待
        yield return new WaitForSeconds(15f);

        while (true)
        {
            SpawnEnemy2();//生成敌机
            yield return new WaitForSeconds(currentEnemy2Interval);//等待当前动态间隔
        }
    }

    //随机X轴位置生成一个敌机预制体
    void SpawnEnemy0()
    {
        float x = Random.Range(-2.25f, 2.25f);
        Instantiate(enemy0Prefab, new Vector3(x, transform.position.y, transform.position.z), transform.rotation);
    }
    void SpawnEnemy1()
    {
        float x = Random.Range(-1.95f, 1.95f);
        Instantiate(enemy1Prefab, new Vector3(x, transform.position.y, transform.position.z), transform.rotation);
    }
    void SpawnEnemy2()
    {
        float x = Random.Range(-1.45f, 1.45f);
        Instantiate(enemy2Prefab, new Vector3(x, transform.position.y, transform.position.z), transform.rotation);
    }

    //随机生成奖励预制体
    void SpawnAward1()
    {
        float x = Random.Range(-1.99f, 1.99f);
        Instantiate(award1Prefab, new Vector3(x, transform.position.y, transform.position.z), transform.rotation);
    }
    void SpawnAward2()
    {
        float x = Random.Range(-1.99f, 1.99f);
        Instantiate(award2Prefab, new Vector3(x, transform.position.y, transform.position.z), transform.rotation);
    }
    //爱心生成方法
    void SpawnAwardHeart()
    {
        if(Random.value<0.2f)//每10秒20%概率生成爱心
        {
            float x = Random.Range(-1.99f, 1.99f);
            Instantiate(awardHeartPrefab, new Vector3(x, transform.position.y, transform.position.z), transform.rotation);
        }
    }
}
