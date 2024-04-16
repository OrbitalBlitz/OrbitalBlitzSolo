using System;

namespace OrbitalBlitz.Game.Features.API.Models {
    [Serializable]
    public class Record {
        public string _id;
        public User userId;
        public int circuitId;
        public int time;
        public int __v;
    }
    
    [Serializable]
    public class RecordToSave {
        public string userId;
        public string circuitId;
        public int time;
    }

    [Serializable]
    public class RecordSavedResponse {
        public string message;
    }
}