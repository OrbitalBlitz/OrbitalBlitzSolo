using System;
using System.Collections.Generic;
using System.Linq;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using OrbitalBlitz.Game.Scenes.Race.Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace OrbitalBlitz.Game.Scenes.SelectCircuit.UI {
    public class SelectCircuitView : MonoBehaviour {
        private VisualTreeAsset _circuitElementTemplate;
        private ListView _listView;
        
        public Button PlayClockButton;
        public Button PlayClassicButton;
        public Button BackButton;

        public VisualElement PersonalBestsDiv;
        public VisualElement WorldBestsDiv;

        public event Action<RaceStateManager.RaceMode> OnPlayClicked;
        public event Action<Loader.CircuitInfo> OnCircuitClicked;
        public event Action OnBackClicked;

        private void Awake() {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            
            PlayClockButton = root.Q<Button>("btn_play_clock");
            PlayClockButton.clicked += () => { OnPlayClicked?.Invoke(RaceStateManager.RaceMode.Clock); };

            PlayClassicButton = root.Q<Button>("btn_play_classic");
            PlayClassicButton.clicked += () => { OnPlayClicked?.Invoke(RaceStateManager.RaceMode.Classic); };
            
            BackButton = root.Q<Button>("btn_back");
            BackButton.clicked += () => { OnBackClicked?.Invoke(); };
            
            _listView = root.Q<ListView>("ListView");
            _listView.itemsChosen += objects => {
                Debug.Log($"Objects chosen : {objects.Select(o => o.ToString()).Aggregate((acc, o) => $"{acc},{o}")}");
            };
            _circuitElementTemplate = Resources.Load<VisualTreeAsset>("CircuitElement");
            
        }

        public void setCircuitList(List<Loader.CircuitInfo> circuits) {
            VisualElement MakeItem() {
                return _circuitElementTemplate.CloneTree().contentContainer;
            }
            void BindItem(VisualElement element, int index) {
                
                Loader.CircuitInfo circuit = (Loader.CircuitInfo)_listView.itemsSource[index];
                Debug.Log($"Binding circuit {index} ({circuit.Name})");
                element.Q<Label>("text_name").text = circuit.Name;
                
                
                Button joinButton = element.Q<Button>("btn_select");
                Action clickHandler = () => {
                    Debug.Log($"Clicked on circuit {circuit.Name}");
                    OnCircuitClicked?.Invoke(circuit);
                };
                joinButton.clicked += clickHandler;
                joinButton.userData = clickHandler;
                
                Color medal_color;      
                switch(circuit.Medal)
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
                element.Q<VisualElement>("medal").style.backgroundColor = medal_color;

                Color rank_color;
                switch(circuit.Rank)
                {
                    case 1:
                        rank_color = Circuit.GoldColor;
                        break;
                    case 2:
                        rank_color = Circuit.SilverColor;
                        break;
                    case 3:
                        rank_color = Circuit.BronzeColor;
                        break;
                    default:
                        rank_color = Circuit.DefaultColor;
                        break;
                }
                element.Q<VisualElement>("medal").style.color = rank_color;
                element.Q<Label>("text_ranking").text = circuit.Rank == 0 ? "rank: ---" : $"rank: {circuit.Rank}/{circuit.RecordsCount}";

                for (var i = 0; i < 3; i++) {
                    var pr = circuit.PersonalBests.ElementAtOrDefault(i);
                    var wr = circuit.WorldBests.ElementAtOrDefault(i);
                    
                    element.Q<Label>($"text_pb_{i+1}").text = (pr != null) 
                        ? $"{i+1}.{millisecondsToRecordString(pr.time)}"
                        : $"No Record.";
                    
                    element.Q<Label>($"text_wr_{i+1}").text = (wr != null)
                        ? $"{i+1}.{wr.userId.username} : {millisecondsToRecordString(wr.time)}"
                        : $"No Record.";
                }
                
                if (!UserSession.Instance.IsConnected())
                    element.Q<VisualElement>("div_pbs").style.display = DisplayStyle.None;

            }
            
            void UnbindItem(VisualElement element, int index) {
                Button joinButton = element.Q<Button>("btn_select");
                Action clickHandler = joinButton.userData as Action;
                if (clickHandler != null) {
                    joinButton.clicked -= clickHandler;
                }
            }
        

            try {
                _listView.itemsSource = null;
                _listView.itemsSource = circuits;
                _listView.itemHeight = 150; // set an item height
                _listView.makeItem = MakeItem;
                _listView.bindItem = BindItem;
                _listView.unbindItem = UnbindItem;
                _listView.showBoundCollectionSize = true;
                _listView.Rebuild();
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }

        private string millisecondsToRecordString(int milliseconds) {
            var timespan = TimeSpan.FromMilliseconds(milliseconds);            
            return timespan.ToString(@"mm\'ss\'\'ff\'\'\'");
        }
    }
}