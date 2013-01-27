//
//http://xnatweener.codeplex.com/
//

namespace XNATweener
{
    /// <summary>
    ///     <para>The Loop class is a static class for easy loop control of the Tweener.</para>
    ///     <para>You can loop continuousely FrontToBack or BackAndForth or for a specific number of times.</para>
    ///     <para>It can be used either by the static methods on this class or by the corresponding methods on the Tweener classes.</para>
    /// </summary>
    public static class Loop
    {
        #region Public static methods

        public static void BackAndForth(ITweener tweener)
        {
            tweener.Ended += tweener.Reverse;
        }

        public static void BackAndForth(ITweener tweener, int times)
        {
            var helper = new TimesLoopingHelper(tweener, times);
            tweener.Ended += helper.BackAndForth;
        }

        public static void FrontToBack(ITweener tweener)
        {
            tweener.Ended += tweener.Restart;
        }

        public static void FrontToBack(ITweener tweener, int times)
        {
            var helper = new TimesLoopingHelper(tweener, times);
            tweener.Ended += helper.FrontToBack;
        }

        #endregion
        private struct TimesLoopingHelper
        {
            #region Private fields

            private int times;
            private readonly ITweener tweener;

            #endregion
            #region Ctors

            public TimesLoopingHelper(ITweener tweener, int times)
            {
                this.tweener = tweener;
                this.times = times;
            }

            #endregion
            #region Event handlers

            public void BackAndForth()
            {
                if (Stop())
                {
                    tweener.Ended -= BackAndForth;
                }
                else
                {
                    tweener.Reverse();
                }
            }

            public void FrontToBack()
            {
                if (Stop())
                {
                    tweener.Ended -= FrontToBack;
                }
                else
                {
                    tweener.Reset();
                }
            }

            #endregion
            #region Private methods

            private bool Stop()
            {
                return --times == 0;
            }

            #endregion
        }
    }
}