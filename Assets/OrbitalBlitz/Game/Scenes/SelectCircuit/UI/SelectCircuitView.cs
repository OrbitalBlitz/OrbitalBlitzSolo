using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.SelectCircuit.UI {
    public class SelectCircuitView : MonoBehaviour {
        private VisualTreeAsset _circuitElementTemplate;
        private ListView _listView;
        private Button _playButton;

        public event Action OnPlayClicked;
        public event Action<Loader.CircuitInfo> OnCircuitClicked;

        private void Awake() {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            _playButton = root.Q<Button>("btn_play");
            _playButton.clicked += () => { OnPlayClicked?.Invoke(); };

            _listView = root.Q<ListView>("ListView");
            _circuitElementTemplate = Resources.Load<VisualTreeAsset>("CircuitElement");
        }
        
        public void setCircuitList(List<Loader.CircuitInfo> circuits) {
            VisualElement MakeItem() {
                return _circuitElementTemplate.CloneTree().contentContainer;
            }
            void BindItem(VisualElement element, int index) {
                Loader.CircuitInfo circuit = (Loader.CircuitInfo)_listView.itemsSource[index];
                element.Q<Label>("text_name").text = circuit.Name;
                // // Attach button event
                Button joinButton = element.Q<Button>("btn_select");
                joinButton.clicked += () => {
                    OnCircuitClicked?.Invoke(circuit);
                };
            }
        

            try {
                _listView.itemsSource = null;
                _listView.itemsSource = circuits;
                _listView.itemHeight = 150; // set an item height
                _listView.makeItem = MakeItem;
                _listView.bindItem = BindItem;
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}