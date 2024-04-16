using System;
using System.Collections.Generic;

namespace OrbitalBlitz.Game.Features.API.Models {
    [Serializable]
    public class MedalData {
        public string _id;
        public string userId;
        public int circuitId;
        public string medal;
        public int __v;    
    }
    
    [Serializable]
    public class MedalToSave {
        public string userId;
        public string circuitId;
        private string medal;

        private List<string> correct_medals = new() { "bronze", "silver", "gold" }; 

        public MedalToSave(string user_id, string circuit_id, string medal) {
            if (!correct_medals.Contains(medal))
                throw new Exception($"medal param should be in {correct_medals}");
            userId = user_id;
            circuitId = circuit_id;
        }
    }
    
    [Serializable]
    public class MedalSavedResponse {
        public string message;
    }
}