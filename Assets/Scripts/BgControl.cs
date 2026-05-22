using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//该脚本实现背景的循环滚动
public class BgControl : MonoBehaviour
{
    //依旧需要滚动的速度,设置为2
    public float speed = 2;

    //创建两个背景的变量
    public Transform bg1;
    public Transform bg2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()//用同一个update控制，就不会出现不同步的情况了
    {
        //这样就保证了它俩移动的同步性
        bg1.Translate(Vector3.down * speed * Time.deltaTime);
        bg2.Translate(Vector3.down * speed * Time.deltaTime);

        //判断bg1是否超出屏幕边界，实现循环
        if (bg1.position.y < -8.52f)
        {
            //如果bg1超出屏幕边界，则移动到bg2的上方
            bg1.position = new Vector3(bg2.position.x, bg2.position.y + 8.52f, bg2.position.z);
        }
        //同上判断bg2是否超出屏幕边界，实现循环
        if (bg2.position.y < -8.52f)
        {
            //如果bg2超出屏幕边界，则移动到bg2的上方
            bg2.position = new Vector3(bg1.position.x, bg1.position.y + 8.52f, bg1.position.z);
        }
    }
}
