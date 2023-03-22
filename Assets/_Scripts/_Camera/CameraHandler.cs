using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MINIGAME
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        public LayerMask ignoreLayer;

        private Transform m_Transform;
        private Vector3 cameraTransformPosition;
        private Vector3 cameraFollowVelocity = Vector3.zero;

        public static CameraHandler cameraHandler;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;

        private float targetPos;
        private float defaultPos;
        private float lookAngle;
        private float pivotAngle;
        public float min_Pivot = -35;
        public float max_Pivot = 35;

        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;

        private void Awake()
        {
            cameraHandler = this;
            m_Transform = transform;
            defaultPos = cameraTransform.localPosition.z;
            ignoreLayer = ~(1 << 8 | 1 << 9| 1 << 10);
            targetTransform = FindObjectOfType<PlayerManager>().transform;
        }

        public void FollowTarget(float delta) {
            Vector3 targetPos = Vector3.SmoothDamp
                (m_Transform.position, targetTransform.position,ref cameraFollowVelocity, delta / followSpeed);
            m_Transform.position = targetPos;
            HandlerCameraCollisions(delta);
        }

        public void HandleCameraRotation(float delta, float mouseInputX, float mouseInputY)
        {
            lookAngle += (mouseInputX * lookSpeed) / delta;
            pivotAngle -= (mouseInputY* pivotSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, min_Pivot, max_Pivot);

            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            m_Transform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;

            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        private void HandlerCameraCollisions(float delta)
        {
            targetPos = defaultPos;
            RaycastHit hit;
            Vector3 dir = cameraTransform.position - cameraPivotTransform.position;
            dir.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius,
                dir, out hit, Mathf.Abs(targetPos), ignoreLayer))
            {
                float distance = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPos = -(distance - cameraCollisionOffset);
            }
            if(Mathf.Abs(targetPos) < minimumCollisionOffset)
            {
                targetPos = -minimumCollisionOffset;
            }
            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPos, delta/0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }
    }
}
