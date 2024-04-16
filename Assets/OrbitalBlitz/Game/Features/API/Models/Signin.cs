using System;

namespace OrbitalBlitz.Game.Features.API.Models {
    [Serializable]
    public class SigninRequestData {
        public string username;
        public string password;
    }
    
    [Serializable]
    public class SigninResponse {
        public string message;
    }
}
