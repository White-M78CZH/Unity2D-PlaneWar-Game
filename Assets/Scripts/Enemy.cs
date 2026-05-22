using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //给敌机设置初始速度
    public float speed = 3;

    //给敌机设置血量
    public int hp = 1;

    //设置两个布尔值,判断是否播放敌机受伤和死亡的帧动画
    public bool isPlayDamageAni = false;
    public bool isPlayDeathAni = false;

    //定义受伤帧动画
    public Sprite[] damageSprites;

    //定义死亡帧动画
    public Sprite[] deathSprites;

    //帧率、计时器、当前帧
    public float frameRate = 10;
    private float timer = 0;
    private int currentFrame = 0;

    //控制精灵图渲染的组件
    private SpriteRenderer spriteRenderer;

    //保留未被攻击状态下 的那一帧
    public Sprite idleSprite;

    //敌人死亡给予分数
    public int score = 100;

    //对音效的引用
    public AudioSource deathAudio;

    // Start is called before the first frame update
    void Start()
    {
        //组件的获取
        spriteRenderer = GetComponent<SpriteRenderer>();
        //用spriteRenderer组件获取普通状态的那一帧，并赋值给idleSprite
        idleSprite = spriteRenderer.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        MoveUpdate();
        PlayDamageAnimationUpdate();
        PlayDeathAnimationUpdate();
    }

    //移动控制与自动销毁
    void MoveUpdate()
    {
        if (hp > 0)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }

        if (transform.position.y < -5.45f)
        {
            Destroy(this.gameObject);
        }
    }

    //播放受伤帧动画的方法
    void PlayDamageAnimationUpdate()
    {
        if (isPlayDamageAni == false) return;

        timer += Time.deltaTime;
        if (timer > 1 / frameRate)
        {
            currentFrame++;
            timer -= 1 / frameRate;//保留剩余的时间，避免帧率波动
        }

        //说明播放到了最后一帧，这个动画就放完了，就恢复到正常状态
        if (currentFrame >= damageSprites.Length)
        {
            ResetIdleState();
        }
        else//还没有播放完
        {
            spriteRenderer.sprite = damageSprites[currentFrame];//继续播放当前帧的图
        }
    }
    //播放死亡帧动画的方法
    void PlayDeathAnimationUpdate()
    {
        if (isPlayDeathAni == false) return;

        timer += Time.deltaTime;
        if (timer > 1 / frameRate)
        {
            currentFrame++;
            timer -= 1 / frameRate;
        }
        if (currentFrame >= deathSprites.Length)//说明死亡的帧动画放完了
        {
            spriteRenderer.enabled = false;//不显示已死亡敌人
            Destroy(this.gameObject,5);//于是5秒后消除敌机
        }
        else
        {
            spriteRenderer.sprite=deathSprites[currentFrame];//没放完就继续当前帧的动画
        }
    }


    //重置敌机状态（即受击帧动画不会一直停留在敌机身上）
    void ResetIdleState()
    {
        isPlayDamageAni = false;
        this.spriteRenderer.sprite = idleSprite;//赋予正常帧的敌机状态
        timer = 0;//重置
        currentFrame = 0;
    }
    //不带参数的TakeDamage
    void TakeDamage()
    {
        TakeDamage(1);
    }
    //敌机受到伤害
    public void TakeDamage(int damage=1)
    {
        //可以避免在播放死亡动画时受到攻击又重复播放
        if (hp <= 0) return;

        //被子弹打到血量减1
        hp -= damage;

        if (hp <= 0)
        {
            //死亡处理
            Die();
        }
        else
        {
            //受伤处理
            ResetIdleState();
            isPlayDamageAni = true;
        }
    }
    void Die()
    {
        ResetIdleState();
        isPlayDeathAni = true;
        GetComponent<Collider2D>().enabled = false;//死亡时将碰撞体关闭，这样在播放死亡动画时，子弹就能传过去了
        GameManager.Instance.AddScore(score);//调用全局的GameManager，给玩家增加对应敌机的分数。
        deathAudio.Play();//播放死亡音效
    }
}
