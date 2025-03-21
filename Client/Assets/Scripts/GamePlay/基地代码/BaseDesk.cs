using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDesk : MonoBehaviour
{
    private BoxCollider2D _boxCollider2D;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Todo 实现玩家打开桌子的逻辑，并保证不会重复开启，做好时间逻辑计算
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
