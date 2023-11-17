namespace OrbitalBlitz.Game.Features.Ship.Controllers {
    public interface IShipController
    {
        public void Accelerate(float input);
        public void Steer(float input);
        public void Brake(int input);
        public void Respawn();
        public void ActivateBlitz();
    }
}
