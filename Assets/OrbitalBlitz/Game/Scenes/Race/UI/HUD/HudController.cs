using System;
using System.Collections;
using System.Collections.Generic;
using OrbitalBlitz.Game.Features.Player;
using OrbitalBlitz.Game.Scenes.Race.Scripts;
using Unity.Mathematics;
using UnityEngine;

public class HudController : MonoBehaviour
{
    [SerializeField] private HudView _view;
    private void Start() {
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (RaceStateManager.Instance.TrainingMode != RaceStateManager.TrainingModeTypes.Disabled) return;
        var timespan = TimeSpan.FromSeconds(RaceStateManager.Instance.HumanPlayer.Info.timer);            
        _view.timer.text = timespan.ToString(@"mm\'ss\'\'ff\'\'\'");
        _view.speed.text = math.round(10* RaceStateManager.Instance.HumanPlayer.AbstractShipController.GetSpeed()).ToString();
    }
}
