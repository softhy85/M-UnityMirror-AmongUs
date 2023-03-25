using System;
using UnityEngine;

namespace Player.Information.Structure
{
    [Serializable]
    public struct PlayerPrefab {
        public GameObject prefab;
        public PlayerRole role;
    }
}