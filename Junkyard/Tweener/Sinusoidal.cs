//
//http://xnatweener.codeplex.com/
//

#region

using System;

#endregion

namespace XNATweener
{
    public static class Sinusoidal
    {
        #region Public static methods

        public static float EaseIn(float t, float b, float c, float d)
        {
            return -c*(float) Math.Cos(t/d*(Math.PI/2)) + c + b;
        }

        public static float EaseInOut(float t, float b, float c, float d)
        {
            return -c/2*((float) Math.Cos(Math.PI*t/d) - 1) + b;
        }

        public static float EaseOut(float t, float b, float c, float d)
        {
            return c*(float) Math.Sin(t/d*(Math.PI/2)) + b;
        }

        #endregion
    }
}