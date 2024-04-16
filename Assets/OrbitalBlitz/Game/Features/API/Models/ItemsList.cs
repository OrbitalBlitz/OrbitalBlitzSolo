using System;
using System.Collections.Generic;

namespace OrbitalBlitz.Game.Features.API.Models {
    [Serializable]
    public class ItemsList<T> {
        public List<T> items;
    }
}
