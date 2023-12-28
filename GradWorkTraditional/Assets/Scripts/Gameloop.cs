using UnityEngine;

namespace Vital
{
    public class Gameloop
    {
        public static Gameloop Instance;
        public SCRMO_CharacterController[] players;

        public Gameloop()
        {
            Instance = this;
            var players = Object.FindObjectsOfType<SCRMO_CharacterController>();
            this.players = players;
        }
    }
}