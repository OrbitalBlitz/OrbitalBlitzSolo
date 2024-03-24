using System;
using System.Collections;
using System.Collections.Generic;
using OrbitalBlitz.Game.Features.Player;
using Unity.Mathematics;
using UnityEngine;

public class HudController : MonoBehaviour
{
    [SerializeField] private HudView _view;
    private void Start() {
    }

    // Update is called once per frame
    void FixedUpdate() {
        var timespan = TimeSpan.FromSeconds(PlayerSingleton.Singleton.RaceInfo.timer);            
        _view.timer.text = timespan.ToString(@"mm\'ss\'\'ff\'\'\'");
        _view.speed.text = math.round(10* PlayerSingleton.Singleton.ShipController.GetSpeed()).ToString();
    }
}
