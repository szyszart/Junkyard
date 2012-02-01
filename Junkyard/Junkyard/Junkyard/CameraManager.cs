using System;
using Microsoft.Xna.Framework;

namespace Junkyard
{
    [Flags]
    public enum CameraState
    {
        Static,
        Follow,
        Goto,
        Shake
    }

    internal class CameraManager
    {
        #region Private fields

        private float _progress;
        private ScaledSprite3D _objectToFollow;

        #endregion
        #region Properties

        public FreeCamera Camera { get; private set; }

        public Vector3 Origin { get; private set; }
        protected float Speed { get; set; }

        public CameraState State { get; private set; }
        public Vector3 Target { get; set; }

        #endregion
        #region Ctors

        public CameraManager(FreeCamera camera)
        {
            Camera = camera;
            State = CameraState.Static;
        }

        #endregion
        #region Public methods

        public void Follow(ScaledSprite3D objectToFollow, float speed = 0.2f, bool overrideGoto = false)
        {
            if (objectToFollow == null)
            {
                throw new ArgumentNullException("objectToFollow");
            }
            if (_objectToFollow == objectToFollow || (State & CameraState.Goto) == CameraState.Goto) return; 

            _objectToFollow = objectToFollow;
            Target = new Vector3(objectToFollow.Position.X, Camera.Position.Y, Camera.Position.Z);
            Speed = speed;
            State = CameraState.Goto | CameraState.Follow;
            Goto();
        }

        /// <summary>
        ///  Issue a request to move camera to target location
        /// </summary>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        public void Goto(Vector3 target, float speed = 0.2f)
        {
            Target = target;
            Speed = speed;
            _objectToFollow = null;
            State = CameraState.Goto;
            Goto();
        }

        public void Update(GameTime gameTime)
        {
            switch (State)
            {
                case CameraState.Goto | CameraState.Follow:
                    Target = new Vector3(_objectToFollow.Position.X, Camera.Position.Y, Camera.Position.Z);
                    goto case CameraState.Goto;
                case CameraState.Goto:
                    {                                                                                                   
                        var egt = (float) gameTime.ElapsedGameTime.TotalSeconds;
                        egt = egt == .0f ? 1.0f/33.0f : egt;
                        _progress += Speed*egt;
                        _progress = MathHelper.Clamp(_progress, .0f, 1.0f);
                        Camera.Position = Vector3.SmoothStep(Origin, Target, _progress);
                        if (_progress == 1.0f)
                        {
                            State = _objectToFollow != null ? CameraState.Follow : CameraState.Static;
                        }
                    }
                    break;
                case CameraState.Follow:                    
                    Camera.Position = new Vector3(_objectToFollow.Position.X, Camera.Position.Y, Camera.Position.Z);                    
                    break;
            }

            Camera.Update();
        }

        #endregion
        #region Private methods

        private void Goto()
        {            
            Origin = Origin = Camera.Position;
            _progress = .0f;
        }

        #endregion
    }
}