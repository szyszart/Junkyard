using Junkyard.Entities;
using Microsoft.Xna.Framework;

namespace Junkyard
{
    public class Player
    {
        #region Properties

        public float Direction { get; set; }
        public int Hp { get; set; }
        public Vector3 InitialPosition { get; set; }
        public string Name { get; set; }
        public Ship Ship { get; set; }

        #endregion
        #region Ctors

        public Player(string name)
        {
            Name = name;
            InitialPosition = Vector3.Zero;
            Direction = 1.0f;
            Hp = 50;
        }

        #endregion
    }
}