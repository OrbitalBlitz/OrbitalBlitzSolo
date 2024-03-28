using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using Unity.Barracuda;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TrainedModelSO", order = 1)]
public class TrainedModelSO : ScriptableObject {
    public Circuit.MedalType Medal;
    public int Time;
    public NNModel Model;
}