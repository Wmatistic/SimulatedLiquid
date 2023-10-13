using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class DrawingCircle : MonoBehaviour
{
    public GameObject particle;
    public GameObject[] tempCircles;
    public Rigidbody[] rb;
    public Vector2[] velocities;
    public Vector2[] positions;
    private Vector2[] predictedPositions;
    private int numOfParticles;
    public float particleSpacing;

    private Vector2[] points;
    private float radius;

    private uint[] spatialLookup;
    private int[] startIndices;
    private int[,] cellOffsets;

    public Vector2 boundsSize;
    public float particleSize = 1f;
    public float collisionDamping = 0.7f;
    public float smoothingRadius;
    public float viscosityStrength;

    public float targetDensity;
    public float pressureMultiplier;
    private float[] densities;
    const float mass = 1;
    public float gravity;

    // Start is called before the first frame update
    void Start()
    {
        rb = new Rigidbody[tempCircles.Length];
        velocities = new Vector2[tempCircles.Length];
        positions = new Vector2[tempCircles.Length];
        predictedPositions = new Vector2[tempCircles.Length];
        densities = new float[tempCircles.Length];

        cellOffsets = new int[1, (int)boundsSize.x];

        radius = smoothingRadius;

        numOfParticles = tempCircles.Length;

        int particlesPerRow = (int)Mathf.Sqrt(numOfParticles);
        int particlesPerCol = (numOfParticles - 1) / particlesPerRow + 1;
        float spacing = particleSize * 2 + particleSpacing;
        
        for (int i = 0; i < numOfParticles; i++)
        {
            float x = (i % particlesPerRow - particlesPerRow / 2f + 0.5f) * spacing;
            float y = (i / particlesPerRow - particlesPerCol / 2f + 0.5f) * spacing;

            Debug.Log(i);
            tempCircles[i] = Instantiate(particle, new Vector3(x, y, 0), Quaternion.identity);
            
            rb[i] = tempCircles[i].GetComponent<Rigidbody>();
            velocities[i] = tempCircles[i].GetComponent<Rigidbody>().velocity;
            positions[i] = tempCircles[i].GetComponent<Transform>().position;
            
            //points[i] = positions[i];
        }
    }

    void Update()
    {
        SimulationStep(Time.deltaTime);

        for (int i = 0; i < tempCircles.Length; i++)
        {
            tempCircles[i].transform.position = positions[i];
            rb[i].velocity = velocities[i];
        }
    }

    void SimulationStep(float deltaTime)
    {
        // Apply gravity and predict next positions
        Parallel.For(0, numOfParticles, i =>
        {
            velocities[i] += Vector2.down * gravity * deltaTime;
            predictedPositions[i] = positions[i] + velocities[i] * deltaTime;
        });
        
        // Update spatial lookup with predicted positions
        //UpdateSpatialLookup(predictedPositions, smoothingRadius);

        // Calculate densities
        Parallel.For(0, numOfParticles, i =>
        {
            densities[i] = CalculateDensity(predictedPositions[i]);
        });

        //Calculate, apply pressure forces, and apply viscosity
        Parallel.For((long)0, numOfParticles, i =>
        {
            Vector2 pressureForce = CalculatePressureForce((int)i);
            Vector2 pressureAcceleration = pressureForce / densities[i] + CalculateViscosityForce((int)i);
            velocities[i] += pressureAcceleration * deltaTime;
        });
        
        // Update positions and resolve collisions
        Parallel.For((long)0, numOfParticles, i =>
        {
            positions[i] += velocities[i] * deltaTime;
            CalculateCircleCollision(ref positions[i],  ref velocities[i]);
        });
    }

    // public List<int> ForeachPointWithinRadius(Vector2 samplePoint)
    // {
    //     (int centreX, int centreY) = PositionToCellCord(samplePoint, radius);
    //     float sqrRadius = radius * radius;
    //
    //     List<int> output = new List<int>();
    //
    //     foreach ((int offsetX, int offsetY) in cellOffsets)
    //     {
    //         uint key = GetKeyFromHash(HashCell(centreX + offsetX, centreY + offsetY));
    //         int cellStartIndex = startIndices[key];
    //
    //         for (int i = cellStartIndex; i < spatialLookup.Length; i++)
    //         {
    //             if (spatialLookup[i] != key) break;
    //
    //             int particleIndex = i;
    //             float sqrDst = (points[particleIndex] - samplePoint).sqrMagnitude;
    //
    //             if (sqrDst <= sqrRadius)
    //             {
    //                 output.Add(particleIndex);
    //             }
    //         }
    //         
    //     }
    //
    //     return output;
    // }
    //
    // public void UpdateSpatialLookup(Vector2[] points, float radius)
    // {
    //     this.points = points;
    //     this.radius = radius;
    //
    //     Parallel.For((long)0, points.Length, i => 
    //     {
    //             (int cellX, int cellY) = PositionToCellCord(points[i], radius);
    //             uint cellKey = GetKeyFromHash(HashCell(cellX, cellY));
    //             spatialLookup[i] = cellKey ;
    //             startIndices[i] = int.MaxValue; 
    //     });
    //     
    //     Array.Sort(spatialLookup);
    //
    //     Parallel.For((long)0, points.Length, i =>
    //     {
    //             uint key = spatialLookup[i];
    //             uint keyPrev = i == 0 ? uint.MaxValue : spatialLookup[i - 1];
    //             if (key != keyPrev)
    //             {
    //                 startIndices[key] = (int)i;
    //             }
    //     });
    // }
    //
    // public (int x, int y) PositionToCellCord(Vector2 point, float radius)
    // {
    //     int cellX = (int)(point.x / radius);
    //     int cellY = (int)(point.y / radius);
    //
    //     return (cellX, cellY);
    // }
    //
    // public uint HashCell(int cellX, int cellY)
    // {
    //     uint a = (uint)cellX * 15823;
    //     uint b = (uint)cellY * 9737333;
    //     return a + b;
    // }
    //
    // public uint GetKeyFromHash(uint hash)
    // {
    //     return hash % (uint)spatialLookup.Length;
    // }

    Vector2 CalculateViscosityForce(int particleIndex)
    {
        Vector2 viscosityForce = Vector2.zero;
        Vector2 position = positions[particleIndex];

        for (int i = 0; i < numOfParticles; i++)
        {
            float dst = (position - positions[i]).magnitude;
            float influence = ViscositySmoothingKernel(dst, smoothingRadius);
            viscosityForce += (velocities[i] - velocities[particleIndex]) * influence;
        }

        return viscosityForce * viscosityStrength;
    }

    Vector2 CalculatePressureForce(int particleIndex)
    {
        Vector2 pressureForce = Vector2.zero;

        //List<int> particlesWithinRadius = new List<int>(ForeachPointWithinRadius(positions[particleIndex]));

        for (int i = 0; i < numOfParticles; i++)
        {
            if (particleIndex == i) continue;

            Vector2 offset = predictedPositions[i] - predictedPositions[particleIndex];
            float dst = offset.magnitude;
            Vector2 dir = dst == 0 ? new Vector2(0, 1) : offset / dst;
            
            float slope = SmoothingKernelDerivative(dst, smoothingRadius);
            float density = densities[i];
            float sharedPressure = CalculateSharedPressure(density, densities[particleIndex]);
            pressureForce += sharedPressure * dir * slope * mass / density;
        }

        return pressureForce;
    }

    float CalculateSharedPressure(float densityA, float densityB)
    {
        float pressureA = ConvertDensityToPressure(densityA);
        float pressureB = ConvertDensityToPressure(densityB);
        return (pressureA * pressureB) / 2;
    }

    Vector2 GetRandomDir()
    {
        float random = Random.value * Random.Range(0, 360);
        return new Vector2(Mathf.Cos(random), Mathf.Sin(random));
    }

    float ConvertDensityToPressure(float density)
    {
        float densityError = density - targetDensity;
        float pressure = densityError * pressureMultiplier;
        return pressure;
    }

    float CalculateDensity(Vector2 samplePoint)
    {
        float density = 0;

        foreach (Vector2 position in positions)
        {
            float dst = (position - samplePoint).magnitude;
            float influence = SmoothingKernel(dst, smoothingRadius);
            density += mass * influence;
        }

        return density;
    }

    float ViscositySmoothingKernel(float dst, float radius)
    {
        if (dst >= radius) return 0;

        float value = Mathf.Max(0, radius * radius - dst * dst);
        return value * value * value;
    }

    float SmoothingKernel(float dst, float radius)
    {
        if (dst >= radius) return 0;
        
        float volume = (Mathf.PI * Mathf.Pow(radius, 4)) / 6;
        return (radius - dst) * (radius - dst) / volume;
    }

    float SmoothingKernelDerivative(float dst, float radius)
    {
        if (dst >= radius) return 0;
        
        float scale = 12 / (Mathf.Pow(radius, 4) * Mathf.PI);
        return (dst - radius) * scale;
    }
    
    private void CalculateCircleCollision(ref Vector2 position, ref Vector2 velocity)
    {
        Vector2 halfBoundsSize = boundsSize / 2 - Vector2.one * particleSize;

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

        position = new Vector2(pX, pY);
        velocity = new Vector2(vX, vY);
    }
}
