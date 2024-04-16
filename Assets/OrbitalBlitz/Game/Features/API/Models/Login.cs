using System;

namespace OrbitalBlitz.Game.Features.API.Models {
    
    [Serializable]
    public class LoginRequestData {
        public string username;
        public string password;
    }
    
    [Serializable]
    public class LoginResponseData {
        public string userId;
        public string token;
    }
}
