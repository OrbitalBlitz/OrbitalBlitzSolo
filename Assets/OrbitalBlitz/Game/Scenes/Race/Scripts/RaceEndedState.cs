namespace OrbitalBlitz.Game.Scenes.Race.Scripts {
    public class RaceEndedState : RaceBaseState {
        
        public override void UpdateState(RaceStateManager context) {
            throw new System.NotImplementedException();
        }

        public override void EnterState(RaceStateManager context) {
            base.EnterState(context);
            context.EndMenuController.Show();
        }
    }
}