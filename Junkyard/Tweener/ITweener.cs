//
//http://xnatweener.codeplex.com/
//

#region

using System;
using Microsoft.Xna.Framework;

#endregion

namespace XNATweener
{
    public delegate void PositionChangedHandler<T>(T newPosition);

    public delegate void EndHandler();

    public interface ITweener
    {
        #region Properties

        bool Playing { get; }

        [Obsolete("Use Playing property instead")]
        bool Running { get; }

        #endregion
        #region Events

        event EndHandler Ended;

        #endregion
        #region Public methods

        void Pause();
        void Play();
        void Reset();
        void Restart();
        void Reverse();

        [Obsolete("Use Play method instead")]
        void Start();

        [Obsolete("Use Pause method instead")]
        void Stop();

        void Update(GameTime gameTime);

        #endregion
    }

    public interface ITweener<T> : ITweener
    {
        #region Properties

        T Position { get; }

        #endregion
        #region Events

        event PositionChangedHandler<T> PositionChanged;

        #endregion
        #region Public methods

        void Reset(T to);
        void Reset(T to, TimeSpan duration);
        void Reset(T to, float speed);
        void Reset(T from, T to, TimeSpan duration);
        void Reset(T from, T to, float speed);

        #endregion
    }
}