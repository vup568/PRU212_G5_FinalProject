using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5;

    private Vector2 movement; //Vector 2 ( vertical + horizontal ) when player move the distance will calculate per Unit 

    public Animator animator;
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude); //calculate power of Vector
    }

    private void FixedUpdate()
      //  cứ đúng 0.02 giây(tương đương 50 lần/giây) thì FixedUpdate sẽ được gọi một lần. Regardless strong or weak device
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}
