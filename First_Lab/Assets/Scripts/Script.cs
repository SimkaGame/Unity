using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Script : MonoBehaviour
{
    public float speed;
    Rigidbody2D rb;

    public float jumpForce;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");

        Vector2 moving = new Vector2(moveHorizontal * speed, rb.velocity.y);
    
        rb.velocity = moving;

        if (moveHorizontal >0){
            transform.localScale = new Vector2(-1, 1);
        }
        if(moveHorizontal < 0){
            transform.localScale = new Vector2(-1, 1);
        }
    }
}
