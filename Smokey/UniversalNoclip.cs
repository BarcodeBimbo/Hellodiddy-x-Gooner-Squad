using UnityEngine;

namespace Smokey
{
    public static class UniversalNoclip
    {
        public static bool IsFlying = false;

        private static float normalSpeed = 10f;
        private static float fastSpeed = 30f;
        private static float slowSpeed = 5f;

        public static void Update()
        {
            if (!IsFlying)
                return;

            Transform target = null;
            if (Features.LocalPlayer != null && Features.LocalPlayer.transform != null)
            {
                target = Features.LocalPlayer.transform;
            }
            else if (Camera.main != null)
            {
                target = Camera.main.transform;
            }

            if (target == null)
                return;

            Vector3 direction = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
                direction += target.forward;
            if (Input.GetKey(KeyCode.S))
                direction -= target.forward;
            if (Input.GetKey(KeyCode.A))
                direction -= target.right;
            if (Input.GetKey(KeyCode.D))
                direction += target.right;
            if (Input.GetKey(KeyCode.Space))
                direction += target.up;
            if (Input.GetKey(KeyCode.LeftControl))
                direction -= target.up;

            float speed = normalSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
                speed = fastSpeed;
            if (Input.GetKey(KeyCode.LeftAlt))
                speed = slowSpeed;

            target.position += direction.normalized * speed * Time.deltaTime;
        }
    }
}
