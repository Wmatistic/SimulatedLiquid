using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public Vector2 boundsSize = new Vector2(11, 11);
    public float particleSize = 1f;
    public float collisionDamping = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        calculateCircleCollision();
    }

    void calculateCircleCollision()
    {
        Vector2 halfBoundsSize = boundsSize / 2 - Vector2.one * particleSize;

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();

        Vector2 position = gameObject.transform.position;
        Vector2 velocity = rb.velocity;
        
        float pX = position.x;
        float pY = position.y;
            
        float vX = velocity.x;
        float vY = velocity.y;

        if (Mathf.Abs(position.x) > halfBoundsSize.x)
        {
            pX = halfBoundsSize.x * Mathf.Sign(position.x);
            vX *= -1 * collisionDamping;
        }

        if (Mathf.Abs(position.y) > halfBoundsSize.y)
        {
            pY = halfBoundsSize.y * Mathf.Sign(position.y);
            vY *= -1 * collisionDamping;
        }

        gameObject.transform.position = new Vector3(pX, pY, 0);
        rb.velocity = new Vector3(vX, vY,0);
    }
}
