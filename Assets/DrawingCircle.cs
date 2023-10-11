using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DrawingCircle : MonoBehaviour
{
    public GameObject circle;
    public GameObject[] tempCircles;
    
    public Vector2 min = new Vector2(-4, -4);
    public Vector2 max = new Vector2(5, 5);

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < tempCircles.Length; i++)
        {
            tempCircles[i] = Instantiate(circle, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }

    void Update()
    {
        calculateCircleCollision();
    }

    public Vector2 boundsSize = new Vector2(10, 10);
    private Vector2 particleSize = new Vector2(1, 1);

    void calculateCircleCollision()
    {
        // for (int i = 0; i < tempCircles.Length; i++)
        // {
        //     tempCircles[i].transform.position = new Vector3(
        //         Mathf.Clamp(tempCircles[i].transform.position.x, min.x, max.x),
        //         Mathf.Clamp(tempCircles[i].transform.position.y, min.y, max.y),
        //         0
        //     );
        // }

        Vector2 halfBoundsSize = boundsSize / 2 - Vector2.one * particleSize;

        for (int i = 0; i < tempCircles.Length; i++)
        {
            float pX = tempCircles[i].transform.position.x;
            float pY = tempCircles[i].transform.position.y;
            
            float vX = tempCircles[i].GetComponent<Rigidbody>().velocity.x;
            float vY = tempCircles[i].GetComponent<Rigidbody>().velocity.y;
            
            if (Mathf.Abs(tempCircles[i].transform.position.x) > halfBoundsSize.x)
            {
                pX =
                    halfBoundsSize.x * Mathf.Sign(tempCircles[i].transform.position.x);
                vX *= -1;
            }

            if (Mathf.Abs(tempCircles[i].transform.position.y) > halfBoundsSize.y)
            {
                pY = halfBoundsSize.y * Mathf.Sign(tempCircles[i].transform.position.y);
                vY *= -1;
            }

            tempCircles[i].transform.position = new Vector3(pX, pY, 0);
            tempCircles[i].GetComponent<Rigidbody>().velocity = new Vector3(vX, vY,0);
        }
    }
}
