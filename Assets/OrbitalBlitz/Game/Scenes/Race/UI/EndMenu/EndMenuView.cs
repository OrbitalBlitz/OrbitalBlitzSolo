using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using OrbitalBlitz.Game.Scenes.Circuits.Scripts;
using OrbitalBlitz.Game.Scenes.Race.Scripts;

namespace OrbitalBlitz.Game.Scenes.Race.UI.EndMenu {
    public class EndMenuView : MonoBehaviour, IHideableView {
        private Button restart_Button;
        private Button quit_Button;

        public Label time;
        public Label text_new_record;

        public VisualElement Medal;

        public event Action OnRestartClicked;
        public event Action OnQuitClicked;

        private VisualElement root;

        private void Awake() {
            root = GetComponent<UIDocument>().rootVisualElement;
            restart_Button = root.Q<Button>("btn_restart");
            quit_Button = root.Q<Button>("btn_quit");

            time = root.Q<Label>("time");
            text_new_record = root.Q<Label>("text_new_record");

            Medal = root.Q<VisualElement>("medal");

            restart_Button.clicked += () => { OnRestartClicked?.Invoke(); };
            quit_Button.clicked += () => { OnQuitClicked?.Invoke(); };
        }

        public void Show() {
            // TODO move this logic in controller (+ single source of truth for circuit data + API repository)
            StartCoroutine(UserSession.Instance.GetPlayerRecords(
                RaceStateManager.Instance.circuit.Id.ToString(),
                personal_records => {
                    for (var i = 0; i < 3; i++) {
                        var pr = personal_records.ElementAtOrDefault(i);
                        root.Q<Label>($"text_pb_{i + 1}").text =
                            $"{i + 1}.{millisecondsToRecordString(pr.time)}";
                    }

                    if (RaceStateManager.Instance.HumanPlayer.Info.timer <
                        personal_records.ElementAtOrDefault(0).time) {
                        text_new_record.style.display = DisplayStyle.Flex;
                    }
                },
                e => { Debug.Log(e); }));

            StartCoroutine(UserSession.Instance.GetWorldRecords(
                RaceStateManager.Instance.circuit.Id.ToString(),
                world_records => {
                    for (var i = 0; i < 3; i++) {
                        var wr = world_records.ElementAtOrDefault(i);
                        // Debug.Log("root.Q<Label>('text_wr_" + (i + 1) + "').text = '" + (i + 1) + "." +
                        //           millisecondsToRecordString(wr.time));

                        root.Q<Label>($"text_wr_{i + 1}").text =
                            $"{i + 1}.{wr.userId.username}: {millisecondsToRecordString(wr.time)}";
                    }
                },
                e => { Debug.Log(e); }
            ));

            StartCoroutine(UserSession.Instance.GetPlayerMedal(
                RaceStateManager.Instance.circuit.Id.ToString(),
                medal => {
                    var best_medal = medal > RaceStateManager.Instance.HumanPlayer.Info.wonMedal
                        ? RaceStateManager.Instance.HumanPlayer.Info.wonMedal
                        : medal;
                    Color medal_color;
                    switch (best_medal) {
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

                    Medal.style.backgroundColor = medal_color;
                },
                e => { Debug.Log(e); }));
            GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public void Hide() {
            GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.None;
        }

        private string millisecondsToRecordString(int milliseconds) {
            var timespan = TimeSpan.FromMilliseconds(milliseconds);
            return timespan.ToString(@"mm\'ss\'\'ff\'\'\'");
        }
    }
}