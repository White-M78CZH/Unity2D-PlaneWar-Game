using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//定义一个枚举类型来表示所有奖励物品的类型
public enum AwardType
{ 
    SuperGun,
    Bomb,
    Heart
}

public class Award : MonoBehaviour
{
    public float speed = 1.4f;
    public AwardType awardType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y<-4.75f)
        {
            Destroy(this.gameObject);//销毁当前脚本挂载的物体
        }
    }
}
