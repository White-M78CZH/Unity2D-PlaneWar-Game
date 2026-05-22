using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //子弹运动的速度
    public float speed = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        if (transform.position.y>4.28f)
        {
            Destroy(this.gameObject);
        }
    }
    //子弹的触发检测
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag=="Enemy")
        {
            collision.SendMessage("TakeDamage");
            Destroy(this.gameObject);
        }
    }
}
