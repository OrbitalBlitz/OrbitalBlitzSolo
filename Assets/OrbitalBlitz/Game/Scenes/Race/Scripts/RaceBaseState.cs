namespace OrbitalBlitz.Game.Scenes.Race.Scripts {
    public abstract class RaceBaseState {
        public abstract void UpdateState(RaceStateManager context);
        public virtual void EnterState(RaceStateManager context) {}
        public virtual void ExitState(RaceStateManager context) {}
        public virtual void Update(RaceStateManager context) {}
    }
}