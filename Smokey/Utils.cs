using System;

namespace Smokey
{
    public class Utils
    {
        public static bool CheckMainScene(string sceneName, bool unloading)
        {
            if (sceneName == "Main" && unloading)
                return false;
            else if (sceneName == "Main" && !unloading)
                return true;
            return false;
        }

        public class FlipFlop
        {
            private bool _state;

            public FlipFlop(bool initialState = false)
            {
                _state = initialState;
            }

            public bool SetState(bool newState)
            {
                if (newState != _state)
                {
                    _state = newState;
                    return true;
                }
                return false;
            }

            public bool GetState() => _state;
        }
    }
}
