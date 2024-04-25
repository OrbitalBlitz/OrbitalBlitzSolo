using System;
using System.Collections;
using System.Collections.Generic;
using OrbitalBlitz.Game.Features.Player;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using OrbitalBlitz.Game.Scenes.Race.Scripts;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class HudController : MonoBehaviour
{
    [FormerlySerializedAs("_view")] [SerializeField] private HudView view;
    private Circuit.MedalType current_medal = Circuit.MedalType.NoMedal;
    private void Start() {
        if (RaceStateManager.Instance.raceMode != RaceStateManager.RaceMode.Classic)
            view.medal.style.display = DisplayStyle.None;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (RaceStateManager.Instance.TrainingMode != RaceStateManager.TrainingModeTypes.Disabled) return;
        var timespan = TimeSpan.FromSeconds(RaceStateManager.Instance.HumanPlayer.Info.timer);            
        view.timer.text = timespan.ToString(@"mm\'ss\'\'ff\'\'\'");
        view.speed.text = math.round(10* RaceStateManager.Instance.HumanPlayer.AbstractShipController.GetSpeed()).ToString();

        
        if (RaceStateManager.Instance.raceMode != RaceStateManager.RaceMode.Classic) return;
        
        var winnable_medal = RaceStateManager.Instance.CurrentWinnableMedal;
        if (current_medal != winnable_medal) updateMedal(winnable_medal);
    }

    private void updateMedal(Circuit.MedalType medal) {
        current_medal = medal;
        
        Color medal_color;      
        switch(current_medal)
        {
            case Circuit.MedalType.Gold:
                medal_color = Circuit.GoldColor;
                break;
            case Circuit.MedalType.Silver:
                medal_color = Circuit.SilverColor;
                break;
            case Circuit.MedalType.Bronze:
                medal_color = Circuit.BronzeColor;
                break;
            default:
                medal_color = Circuit.DefaultColor;
                break;
        }
        view.medal.style.backgroundColor = medal_color;

    }
}
