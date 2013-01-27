//
//http://xnatweener.codeplex.com/
//

namespace XNATweener
{
    public static class Linear
    {
        #region Public static methods

        public static float EaseIn(float t, float b, float c, float d)
        {
            return c*t/d + b;
        }

        public static float EaseInOut(float t, float b, float c, float d)
        {
            return c*t/d + b;
        }

        public static float EaseNone(float t, float b, float c, float d)
        {
            return c*t/d + b;
        }

        public static float EaseOut(float t, float b, float c, float d)
        {
            return c*t/d + b;
        }

        #endregion
    }
}