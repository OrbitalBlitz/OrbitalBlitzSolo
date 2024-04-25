using UnityEngine.Networking;

namespace OrbitalBlitz.Game.Features.API {
    public class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) {
            return true; // This disables all certificate checks
        }
    }
}
