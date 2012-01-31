using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Junkyard
{
    class CameraManager
    {
        public enum CmState
        {
            Follow,
            Goto,
            Shake
        }

        private FreeCamera _camera;

        public CameraManager(FreeCamera camera)
        {
            _camera = camera;
        }

        private CmState _state;
        public CmState State { get { return _state; } }
    }
}
