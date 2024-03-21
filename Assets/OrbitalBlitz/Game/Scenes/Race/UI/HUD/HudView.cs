using System.Collections;
using System.Collections.Generic;
using OrbitalBlitz.Game.Scenes.Race.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class HudView : MonoBehaviour, IHideableView {
    public Label timer;
    public Label speed;

    // Start is called before the first frame update
    void Start() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        timer = root.Q<Label>("timer");
        speed = root.Q<Label>("speed");
    }
    
    public void Show() {
        GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
    }

    public void Hide() {
        GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
    }
}