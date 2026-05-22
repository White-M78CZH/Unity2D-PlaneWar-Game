using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

//该背景用于Player的idle动画
public class Player : MonoBehaviour
{
    //定义一个sprite数组来存放飞机的两帧动画
    public Sprite[] idleSprites;

    //动画播放速率
    public int frameRate = 10;

    //代码里也要有组件才能通过spriteRenderer修改
    private SpriteRenderer spriteRenderer;

    //计时器
    private float timer = 0;

    //创建变量保存当前帧
    private int currentFrame = 0;

    //记录每帧鼠标的坐标
    public Vector3 lastMousePosition = Vector3.zero;//上一帧按下时鼠标的坐标

    //记录鼠标按下的状态
    public bool isMouseDown = false;

    //超级武器的持续时间
    public float superGunDuration = 3;
    //判断超级武器是否到时间
    private float superGunTimer = 0;

    //三个枪的引用
    public GameObject GunTop;
    public GameObject GunLeft;
    public GameObject GunRight;

    //定义玩家血量,开局三条命，最多五条命
    public int currentHp = 3;
    public int maxHp = 5;

    //碰撞敌机后的无敌时间
    private float invincibleTime = 2;
    //布尔变量标识当前是否处于无敌时间
    private bool isInvincible = false;
    //进行无敌计时的计时器
    private float invincibleTimer = 0;

    //闪烁的时间间隔
    public float blinkInterval = 0.2f;

    //玩家死亡帧动画
    public Sprite[] deathSprites;

    //取得音效组件
    public AudioSource getBombAudio;
    public AudioSource getSuperGunAudio;
    public AudioSource getHeartAudio;

    //新增主相机缓存变量
    private Camera mainCamera;

    //新增协程引用
    private Coroutine blinkCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        //避免空指针
        spriteRenderer = GetComponent<SpriteRenderer>();
        //缓存主相机
        mainCamera = Camera.main;
        //确保初始武器状态正确
        TransformToNormalGun();
    }

    // Update is called once per frame
    void Update()
    {
        //调用动画方法
        IdleAnimationUpdate();

        //调用飞机移动方法
        MoveUpdate();

        //控制武器
        SuperGunUpdate();

        //无敌
        InvincibleUpdate();

        //死亡动画
        DeathAnimationUpdate();
    }

    //创建一个方法来专门处理idle动画的播放
    void IdleAnimationUpdate()
    {
        //如果已经死亡则直接退出
        if (currentHp <= 0) return;
        //开始计时
        timer += Time.deltaTime;//每秒加1不受设备帧率影响

        //判断时间是否达到一帧，达到就播放下一帧的动画
        if (timer>1f/frameRate)
        { 
            timer-=1f/frameRate;
            currentFrame=(currentFrame+1)%idleSprites.Length;//与数组的长度求余,可以避免超出了数组的长度
            spriteRenderer.sprite = idleSprites[currentFrame];
        }
    }

    //处理死亡动画播放
    void DeathAnimationUpdate()
    {
        //如果没有死亡则直接退出
        if (currentHp > 0) return;
        //开始计时
        timer += Time.deltaTime;//每秒加1不受设备帧率影响

        //判断时间是否达到一帧，达到就播放下一帧的动画
        if (timer > 1f / frameRate)
        {
            timer -= 1f / frameRate;
            //加Min限制，避免currentFrame超过数组长度
            currentFrame = Mathf.Min(currentFrame + 1, deathSprites.Length);
        }
        if (currentFrame >= deathSprites.Length)
        {
            //GameOver
            GameManager.Instance.GameOver();
        }
        else 
        {
            spriteRenderer.sprite = deathSprites[currentFrame];
        }
    }

    //创建一个控制飞机移动的方法
    void MoveUpdate()
    {
        //判断游戏是否处于暂停状态
        if (GameManager.Instance.IsPause()) return;
        if (currentHp <= 0) return;

        //判断鼠标的按下，若是移动端则是手指的按下
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                isMouseDown = true;
                lastMousePosition = mainCamera.ScreenToWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isMouseDown = false;
            }
            else if (touch.phase==TouchPhase.Moved&&isMouseDown)
            {
                Vector3 offset = mainCamera.ScreenToWorldPoint(touch.position) - lastMousePosition;
                transform.position += offset;
                CheckPosition();
                lastMousePosition = mainCamera.ScreenToWorldPoint(touch.position);
            }
        }
        else//鼠标逻辑
        {
            if (Input.GetMouseButtonDown(0))//这里的0代表的是鼠标左键
            {
                isMouseDown = true;//判断鼠标按下时，就把布尔值设置为true
                lastMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);//并记录首次按下的位置
            }
            //如果鼠标抬起了
            if (Input.GetMouseButtonUp(0))
            {
                isMouseDown = false;//则设置为false，即鼠标抬起
            }
            //当按下时触发
            if (isMouseDown)
            {
                //用Camera.main.ScreenToWorldPoint将屏幕坐标转换成世界坐标
                Vector3 offset = mainCamera.ScreenToWorldPoint(Input.mousePosition) - lastMousePosition;//当前鼠标的位置减去上一次鼠标记录的位置，得到偏移

                //将偏移作用于飞机身上
                transform.position += offset;
                //判断飞机是否在界面内
                CheckPosition();

                //每次结束之后更新鼠标最后位置
                lastMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }
    //检查飞机位置有没有超出边界的方法
    void CheckPosition()
    {
        //x:2.15 , y:-3.75到3.45
        Vector3 pos=transform.position;
        //限制左边界
        if (pos.x<-2.15f)
        {
            pos.x = -2.15f;
        }
        //限制右边界
        if (pos.x > 2.15f)
        {
            pos.x = 2.15f;
        }
        //限制下边界
        if (pos.y<-3.75f)
        {
            pos.y = -3.75f;
        }
        //限制上边界
        if (pos.y>3.45f)
        {
            pos.y = 3.45f;
        }
        //应用于飞机
        transform.position = pos;
    }
    //玩家与物品的碰撞检测
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //如果是奖励物品
        if (collision.tag == "Award")
        {
            Award award=collision.GetComponent<Award>();
            if (award.awardType == AwardType.SuperGun)
            {
                TransformToSuperGun();
                //拾取超级武器时
                if (getSuperGunAudio != null)
                    getSuperGunAudio.Play();//播放拾取超级武器音效
            }
            else if (award.awardType == AwardType.Bomb)
            {
                //拾取炸弹时
                if (getBombAudio != null)
                    getBombAudio.Play();//播放拾取炸弹音效
                //碰撞到炸弹时的捡取
                GameManager.Instance.AddBomb();
            }
            else if (award.awardType==AwardType.Heart)
            {
                AddHealth(1);//吃爱心加一条命
            }
            Destroy(collision.gameObject);
        }
        //如果是敌机
        if (collision.tag == "Enemy" && isInvincible==false)
        {
            //敌机也要受到攻击 自身也要受到攻击
            collision.SendMessage("TakeDamage");
            this.currentHp--;//扣血

            //血量不能小于0
            this.currentHp = Mathf.Max(0, this.currentHp);

            //扣血后更新UI
            UIManager.Instance.UpdateHealthUI(this.currentHp);

            if (currentHp <= 0)
            {
                //死亡处理
                TransformToDeath();
            }
            else
            {
                //血量不为0，就闪烁
                TransformToInvincible();
            }
        }
    }

    void SuperGunUpdate()
    {
        if (superGunTimer > 0)
        {
            superGunTimer-= Time.deltaTime;
            if (superGunTimer <= 0)
            {
                TransformToNormalGun();
            }
        }
        
    }

    //碰撞后一段时间切换到无敌状态
    void TransformToInvincible()
    {
        isInvincible = true;
        invincibleTimer = 0;
        //停止旧线程，避免多线程冲突
        if (blinkCoroutine!=null)
        { 
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(BlinkEffect());
    }

    //定义进行武器切换的方法
    void TransformToSuperGun()
    { 
        GunTop.SetActive(true);
        GunLeft.SetActive(true);
        GunRight.SetActive(true);
        superGunTimer = superGunDuration;
    }

    //奖励时间过后回归正常形态的方法
    void TransformToNormalGun()
    {
        GunTop.SetActive(true);
        GunLeft.SetActive(false);
        GunRight.SetActive(false);
    }
    //禁用所有武器
    void DisableAllGun()
    {
        GunTop.SetActive(false);
        GunLeft.SetActive(false);
        GunRight.SetActive(false);
    }
    //播放玩家死亡动画
    void TransformToDeath()
    {
        DisableAllGun();
        timer = 0;
        currentFrame = 0;
    }
    void InvincibleUpdate()
    {
        //if (isInvincible == false) return;

        //invincibleTimer+= Time.deltaTime;
        //if (invincibleTimer > invincibleTime)//大于说明无敌时间结束了
        //{ 
        //    isInvincible= false;
        //}
    }

    //闪烁协程
    IEnumerator BlinkEffect()
    {
        //无敌时间结束之前就闪烁
        while (invincibleTimer<=invincibleTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;//设置成相反的状态
            yield return new WaitForSeconds(blinkInterval);
            invincibleTimer += blinkInterval;
        }
        //循环结束后要确保自身显示
        spriteRenderer.enabled = true;
        isInvincible = false;
    }

    //玩家加血方法
    public void AddHealth(int amount=1)
    {
        //血量不能超过最大值
        currentHp = Mathf.Min(currentHp + amount, maxHp);
        //更新UI显示
        UIManager.Instance.UpdateHealthUI(currentHp);
        //播放加血音效
        if (getHeartAudio != null)
        {
            getHeartAudio.Play();
        }
    }
}
