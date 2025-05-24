using UnityEngine;

namespace Managers
{
    public class CameraController: MonoBehaviour
    {
        public Transform target;
        public float smoothSpeed = 5.0f;
        public Vector3 offset = new Vector3(0.0f, 0.0f, -10.0f);

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 targetPosition = target.position + offset;
            if (smoothSpeed > 0)
            {
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
                transform.position = smoothedPosition;
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }
}