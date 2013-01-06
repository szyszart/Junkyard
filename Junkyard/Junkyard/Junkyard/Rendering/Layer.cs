#region

using System;
using System.Collections.Generic;

#endregion

namespace Junkyard.Rendering
{
    internal class Layer : IComparable<Layer>
    {
        #region Properties

        public List<IDrawable> Drawables { get; protected set; }
        public float Z { get; set; }

        #endregion
        #region Ctors

        public Layer(float z)
        {
            Z = z;
            Drawables = new List<IDrawable>();
        }

        #endregion
        #region IComparable<Layer> Members

        public int CompareTo(Layer other)
        {
            return other.Z.CompareTo(Z);
        }

        #endregion
    }
}