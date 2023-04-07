using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MINIGAME
{
    public class CameraHandler : MonoBehaviour
    {
        InputHandler inputHandler;
        PlayerManager playerManager;

        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        public LayerMask ignoreLayer;
        public LayerMask environmentLayer;

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
        public float lockedPivotPosition = 2.25f;
        public float unLockedPivotPosition = 1.65f;


        public Transform currentLockOnTarget;

        List<CharacterManager> availableTargets = new List<CharacterManager>();
        public Transform nearestLockOnTarget;
        public Transform leftLockTarget;
        public Transform rightLockTarget;

        public float maximumLockOnDistance = 30;

        private void Awake()
        {
            cameraHandler = this;
            m_Transform = transform;
            defaultPos = cameraTransform.localPosition.z;
            ignoreLayer = ~(1 << 8 | 1 << 9| 1 << 10);
            targetTransform = FindObjectOfType<PlayerManager>().transform;
            inputHandler = FindObjectOfType<InputHandler>();
            playerManager = FindObjectOfType<PlayerManager>();
        }

        private void Start()
        {
            environmentLayer = LayerMask.NameToLayer("Environment");
        }

        public void FollowTarget(float delta) {
            Vector3 targetPos = Vector3.SmoothDamp
                (m_Transform.position, targetTransform.position,ref cameraFollowVelocity, delta / followSpeed);
            m_Transform.position = targetPos;
            HandlerCameraCollisions(delta);
        }

        public void HandleCameraRotation(float delta, float mouseInputX, float mouseInputY)
        {
            if (!inputHandler.lockOnFlag && currentLockOnTarget == null)
            {
                lookAngle += (mouseInputX * lookSpeed) / delta;
                pivotAngle -= (mouseInputY * pivotSpeed) / delta;
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
            else
            {
                float velocity = 0;
                Vector3 dir = currentLockOnTarget.position - transform.position;
                dir.Normalize();
                dir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = targetRotation;

                dir = currentLockOnTarget.position - cameraPivotTransform.position;
                dir.Normalize();

                targetRotation = Quaternion.LookRotation(dir);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                eulerAngle.y = 0;
                cameraPivotTransform.localEulerAngles = eulerAngle;
            }
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

        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);

            for(int i = 0; i <colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();
                if(character != null)
                {
                    Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(targetTransform.position , character.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);
                    RaycastHit hit;

                    if(character.transform.root != targetTransform.transform.root
                        && viewableAngle > -50 && viewableAngle < 50
                        && distanceFromTarget <= maximumLockOnDistance)
                    {
                        if(Physics.Linecast(playerManager.lockOnTransform.position, character.transform.position, out hit))
                        {
                            Debug.DrawLine(playerManager.lockOnTransform.position, character.transform.position);

                            if(hit.transform.gameObject.layer == environmentLayer)
                            {

                            }
                            else
                            {
                                availableTargets.Add(character);
                            }
                        }
                    }
                }
            }

            for(int j = 0; j < availableTargets.Count; j++)
            {
                float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[j].transform.position);
                if(distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[j].lockOnTransform;
                }

                if (inputHandler.lockOnFlag)
                {
                    Vector3 relativeEnenmyPosition = currentLockOnTarget.InverseTransformPoint(availableTargets[j].transform.position);
                    var distanceFromLeftTarget = currentLockOnTarget.transform.position.x - availableTargets[j].transform.position.x;
                    var distanceFromRightTarget = currentLockOnTarget.transform.position.x + availableTargets[j].transform.position.x;

                    if(relativeEnenmyPosition.x > 0.00  && distanceFromLeftTarget < shortestDistanceOfLeftTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockTarget = availableTargets[j].lockOnTransform;
                    }
                    if (relativeEnenmyPosition.x < 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockTarget = availableTargets[j].lockOnTransform;
                    }
                }
            }
        }
        public void ClearLockOnTargets()
        {
            availableTargets.Clear();
            nearestLockOnTarget = null;
            currentLockOnTarget = null;
        }

        public void SetCameraHeight()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
            Vector3 newUnlockedPosition = new Vector3(0, unLockedPivotPosition);

            if(currentLockOnTarget != null)
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }
        }
    }
}
