using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DrawingCircle : MonoBehaviour
{
    public GameObject particle;
    public GameObject[] tempCircles;
    private int numOfParticles;
    public float particleSpacing;
    private float particleSize;

    private CollisionDetection cd;

    // Start is called before the first frame update
    void Start()
    {
        cd = particle.GetComponent<CollisionDetection>();
        
        numOfParticles = tempCircles.Length;
        particleSize = cd.particleSize;

        int particlesPerRow = (int)Mathf.Sqrt(numOfParticles);
        int particlesPerCol = (numOfParticles - 1) / particlesPerRow + 1;
        float spacing = particleSize * 2 + particleSpacing;
        
        for (int i = 0; i < numOfParticles; i++)
        {
            float x = (i % particlesPerRow - particlesPerRow / 2f + 0.5f) * spacing;
            float y = (i / particlesPerRow - particlesPerCol / 2f + 0.5f) * spacing;
            tempCircles[i] = Instantiate(particle, new Vector3(x, y, 0), Quaternion.identity);
        }
    }

    void Update()
    {
        
    }
}
