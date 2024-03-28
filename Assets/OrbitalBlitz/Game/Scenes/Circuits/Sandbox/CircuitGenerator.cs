using System.Collections.Generic;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

// https://docs.unity3d.com/Packages/com.unity.splines@2.0/api/UnityEngine.Splines.html

public class CircuitGenerator : MonoBehaviour
{
    public int numberOfKnots;
    public float circuitLength;

    // public int gridX = 100;
    // public int gridZ = 100;


    public (int Min, int Max) knotHeightRange = (-100, 100); // randomly knots height (Y axis)
    public (int Min, int Max) knotSpaceRange = (100, 300); // space between knots

    public int checkpointInterval = 3; // frequency at which checkpoints are placed in terms of knots
    private List<Checkpoint> checkpoints;
    public GameObject checkpointsParent;

    public int minDistanceBetweenKnots = 90;
    public int maxKnotRotation = 90;

    public GameObject checkpointPrefab;
    public GameObject startLinePrefab;
    //public GameObject endLinePrefab;

    private GameObject circuitParent; // init

    void Awake() 
    {
        
    }
    void Start()
    {
        //GenerateCircuit();
        
    }

    private void init(){
        circuitParent = transform.parent.gameObject;
    }

    public void GenerateCircuit() {
        init();
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
        knots[0] = knot;
        createStartLine(knot);
        
        int i;
        for (i = 1; i < numberOfKnots; i++) {
            knot = createKnot(knot, knots);
            knots[i] = knot;
            
            // Create a checkpoint every x knots
            if (knots.Length % checkpointInterval == 0){
                    createCheckpoint(knot, i);
            }

        }

        // Return to 0, 0, 0 (Close the circuit)
        BezierKnot Endknot = new BezierKnot(new float3(0, 0, -100));
        // to be smooth point to this
        // find a path to it
        //How to not cross links ?

        // Create the finish line
        createFinishLine(knot, i);

        // Add checkpoints to circuitSData
        Circuit circuit = circuitParent.GetComponent<Circuit>();
        circuit.Checkpoints = checkpoints;
        
        // Create spline using the knots
        spline.Knots = knots;   
        spline.SetTangentMode(TangentMode.AutoSmooth);     
    }

    BezierKnot createKnot(BezierKnot lastKnot, BezierKnot[] existingKnots){
        // Randomize the location of the next knot based on the last knot orientation

        float3 newPosition;
        bool positionIsValid;

        do {
            positionIsValid = true;

            // 1. get last knot forward direction (BezierKnot is coded in Unity.Mathematics)
            float3 forwardDirection = math.mul(lastKnot.Rotation, new float3(0, 0, 1));

            // 2. Randomize the Direction Within 90 Degrees
            float randomAngle = UnityEngine.Random.Range(-maxKnotRotation, maxKnotRotation);
            quaternion randomRotation = quaternion.EulerXYZ(new float3(0, math.radians(randomAngle), 0));
            float3 newDirection = math.mul(randomRotation, forwardDirection);

            // 3. Calculate the New Position
            float distance = UnityEngine.Random.Range(knotSpaceRange.Min, knotSpaceRange.Max);
            newPosition = lastKnot.Position + math.normalize(newDirection) * distance;

            // Randomize the height (NOT WORKING)
            // float height = UnityEngine.Random.Range(knotHeightRange.Min, knotSpaceRange.Max);
            // newPosition.y = height;

            // Check if the new position is too close to any existing knot
            foreach (BezierKnot existingKnot in existingKnots) {
                if (math.distance(newPosition, existingKnot.Position) < minDistanceBetweenKnots) {
                    positionIsValid = false;
                    break; // Exit the loop as we need to find a new position
                }
            }


        }  while (!positionIsValid);

        return new BezierKnot(newPosition);
    }

    void createStartLine(BezierKnot lastKnot){
        // Add a checkpoint at the last knot

        // Convert Unity.Mathematics.float3 to UnityEngine.Vector3
        Vector3 position = new Vector3(lastKnot.Position.x, lastKnot.Position.y + 5, lastKnot.Position.z);
        
        GameObject startLine = Instantiate(startLinePrefab);
        startLine.transform.SetParent(circuitParent.transform);
        startLine.transform.position = position;

        startLine.name = "startLine";

        
    }

    void createCheckpoint(BezierKnot lastKnot, int number){
        // Add a checkpoint at the last knot

        // Convert Unity.Mathematics.float3 to UnityEngine.Vector3
        Vector3 checkpointPosition = new Vector3(lastKnot.Position.x, lastKnot.Position.y + 5, lastKnot.Position.z);
        

        GameObject checkpoint = Instantiate(checkpointPrefab);
        checkpoint.transform.SetParent(checkpointsParent.transform);
        checkpoint.transform.position = checkpointPosition;

        checkpoint.name = "checkpoint" + number;
        
        checkpoints.Add(checkpoint.GetComponent<Checkpoint>());
    }

    void createFinishLine(BezierKnot lastKnot, int number){
        // Create the finishline at the last know (last checkpoint)

        // Convert Unity.Mathematics.float3 to UnityEngine.Vector3
        Vector3 checkpointPosition = new Vector3(lastKnot.Position.x, lastKnot.Position.y + 5, lastKnot.Position.z);
        

        GameObject checkpoint = Instantiate(checkpointPrefab);
        checkpoint.transform.SetParent(checkpointsParent.transform);
        checkpoint.transform.position = checkpointPosition;

        checkpoint.name = "checkpoint" + number;
        
        checkpoints.Add(checkpoint.GetComponent<Checkpoint>());
    }
}
