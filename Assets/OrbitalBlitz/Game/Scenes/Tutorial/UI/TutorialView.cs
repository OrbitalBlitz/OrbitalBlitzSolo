using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using OrbitalBlitz.Game.Scenes.Race.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialView : MonoBehaviour, IHideableView {
    private Label drift_timer;

    private VisualElement boost_checkbox;
    private Label boost_text;
    private VisualElement drift_checkbox;
    private Label drift_text;
    private VisualElement respawn_checkbox;
    private Label respawn_text;
    private VisualElement restart_checkbox;
    private Label restart_text;
    private VisualElement finish_checkbox;
    private Label finish_text;

    public enum TutorialChallenges {
        Boost,
        Drift,
        Respawn,
        Restart,
        Finish
    }

    void Awake() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        boost_checkbox = root.Q<VisualElement>("boost_checkbox");
        boost_text = root.Q<Label>("boost_text");

        drift_checkbox = root.Q<VisualElement>("drift_checkbox");
        drift_text = root.Q<Label>("drift_text");

        respawn_checkbox = root.Q<VisualElement>("respawn_checkbox");
        respawn_text = root.Q<Label>("respawn_text");

        restart_checkbox = root.Q<VisualElement>("restart_checkbox");
        restart_text = root.Q<Label>("restart_text");

        finish_checkbox = root.Q<VisualElement>("finish_checkbox");
        finish_text = root.Q<Label>("finish_text");
        
        drift_timer = root.Q<Label>("drift_time");
    }

    public void ValidateChallenge(TutorialChallenges challenge) {
        VisualElement checkbox;
        Label text;
        switch (challenge) {
            case TutorialChallenges.Boost:
                checkbox = boost_checkbox;
                text = boost_text;
                break;
            case TutorialChallenges.Drift:
                checkbox = drift_checkbox;
                text = drift_text;
                break;
            case TutorialChallenges.Respawn:
                checkbox = respawn_checkbox;
                text = respawn_text;
                break;
            case TutorialChallenges.Restart:
                checkbox = restart_checkbox;
                text = restart_text;
                break;
            case TutorialChallenges.Finish:
                checkbox = finish_checkbox;
                text = finish_text;
                break;
            default:
                throw new Exception($"Challenge {challenge} unknown.");
        }
        checkbox.style.backgroundColor = new StyleColor(Color.green);
        text.style.color = new StyleColor(Color.grey);
    }

    public void SetDriftTimer(float seconds) {
        drift_timer.text = $"{seconds:F2}'s";
    }

    public void Show() {
        GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
    }

    public void Hide() {
        GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
    }
}