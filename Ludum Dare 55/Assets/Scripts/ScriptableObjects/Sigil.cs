using System;
using UnityEngine;

namespace ScriptableObjects
{
    [Serializable]
    public class Sigil : ScriptableObject
    {
        public bool[,] Pixels { get; set; }
    }
}
