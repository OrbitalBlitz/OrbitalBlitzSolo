using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

using UnityEditor;

public class CircuitGenerator : MonoBehaviour
{
    public int numberOfKnots;
    public float circuitLength;

    public int gridX = 100;

    public int gridY = 0; // no evelation
    public int gridZ = 100;

    public int knotMinSpace = 50;
    public int knotMaxSpace = 200;
    public int knotWidth = 10;
    public int maxKnotRotation = 90;

    void Start()
    {
        //GenerateCircuit();
        
    }

    public void GenerateCircuit() {

        // Get the SplineContainer component
        SplineContainer container = gameObject.GetComponent<SplineContainer>();
         // Add a SplineContainer component to this GameObject.
        //var container = gameObject.AddComponent<SplineContainer>();

        // Create a new Spline on the SplineContainer.
        var spline = container.Spline;
        
        // Procedural generation logic here
        var knots = new BezierKnot[numberOfKnots];

        // 1st knot starts at 0, 0, 0
        BezierKnot knot = new BezierKnot(new float3(0, 0, 0));
        for (int i = 0; i < numberOfKnots; i++) {
            knots[i] = knot;
            knot = createKnot(knot);
        }

        

        // Create spline using the knots
        spline.Knots = knots;        
    }

    BezierKnot createKnot(BezierKnot lastKnot){
        // Randomize the location of the next knot based on the last knot orientation

        // 1. get last knot forward direction (BezierKnot is coded in Unity.Mathematics)
        float3 forwardDirection = math.mul(lastKnot.Rotation, new float3(0, 0, 1));

        // 2. Randomize the Direction Within 90 Degrees
        float randomAngle = UnityEngine.Random.Range(-maxKnotRotation, maxKnotRotation);
        quaternion randomRotation = quaternion.EulerXYZ(new float3(0, math.radians(randomAngle), 0));
        float3 newDirection = math.mul(randomRotation, forwardDirection);

        // 3. Calculate the New Position
        float distance = UnityEngine.Random.Range(knotMinSpace, knotMaxSpace);
        float3 newPosition = lastKnot.Position + math.normalize(newDirection) * distance;

        return new BezierKnot(newPosition);        
    }
}
