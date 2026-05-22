using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//控制背景滑动的脚本,以这种方法控制背景的滚动会导致出现缝隙，因为这样两个背景是由它们各自的Background脚本控制的，导致update执行不同步
public class Background : MonoBehaviour
{
    //设置控制背景滑动的速度变量
    //速度以米为单位（即Unity中的一格）一格为100像素
    public float speed = 2;//背景每秒移动两格

    //对另外一个背景的引用
    public Transform otherBg;//在Unity中设置

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //translate可以控制移动,然后Vector3来定义移动的方向(down向下)
        transform.Translate(Vector3.down * speed * Time.deltaTime);//再乘以速度和帧时间

        //判断是否超出屏幕边界
        if (transform.position.y < -8.52f)
        {
            //如果超出屏幕边界，则移动到另一个背景的上方
            transform.position = new Vector3(otherBg.position.x, otherBg.position.y + 8.52f, otherBg.position.z);
        }
    }
}
