
//(c8

using UnityEngine;
using UnityEngine.EventSystems;

namespace ElectricWire
{
    public class AvatarControlRigidBody : MonoBehaviour
    {
        private float speed = 5f;
        private float runSpeed = 8f;
        private float jumpSpeed = 6f;
        private float lookSensitivity = 2f;
        private bool lockFlightViewRotationX = false;
        private bool crouched = false;

        private Rigidbody controller;
        private CapsuleCollider capsuleCollider;
        private Transform cameraHolder;

        private Vector3 cameraThird = new Vector3(0f, 2.5f, 0.5f);
        private Vector3 cameraFirst = new Vector3(0f, 1.9f, 0.4f);

        private float xRotation;
        private float yRotation;
        private Vector3 movement = Vector3.zero;
        private float cameraDistanceMin = 0f;
        private float cameraDistanceMax = 50f;

        private bool spaceIsPressed = false;

        private void Start()
        {
            controller = GetComponent<Rigidbody>();
            capsuleCollider = transform.GetChild(1).GetComponent<CapsuleCollider>();
            cameraHolder = transform.GetChild(0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (Cursor.lockState != CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

            // If UI selected do not move character
            if (EventSystem.current.currentSelectedGameObject != null)
                return;

            if (Input.GetKeyDown(KeyCode.G))
                controller.useGravity = !controller.useGravity;
            if (Input.GetKeyDown(KeyCode.Z))
                lockFlightViewRotationX = !lockFlightViewRotationX;

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                xRotation += Input.GetAxis("Mouse X") * lookSensitivity;
                xRotation = xRotation > 180 ? -180 : xRotation < -180 ? 180 : xRotation;
                transform.rotation = Quaternion.Euler(0, xRotation, 0);

                yRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
                yRotation = Mathf.Clamp(yRotation, -90, 90);
                cameraHolder.localRotation = Quaternion.Euler(yRotation, 0, 0);
            }

            //Scroll mouse
            if (Input.mouseScrollDelta[1] != 0 && !EventSystem.current.IsPointerOverGameObject() && ElectricManager.electricManager.CanTriggerComponent())
            {
                if ((Camera.main.transform.localPosition.z + Input.mouseScrollDelta[1]) <= -cameraDistanceMin &&
                    (Camera.main.transform.localPosition.z + Input.mouseScrollDelta[1]) >= -cameraDistanceMax)
                    Camera.main.transform.localPosition += new Vector3(0f, 0f, Input.mouseScrollDelta[1]);

                if (Camera.main.transform.localPosition.z == 0)
                    cameraHolder.localPosition = cameraFirst;
                else
                    cameraHolder.localPosition = cameraThird;
            }

            spaceIsPressed = Input.GetKey(KeyCode.Space);
        }

        private void FixedUpdate()
        {
            // Do not make player move on bounce
            if (!controller.useGravity)
            {
                controller.velocity = Vector3.zero;
                controller.angularVelocity = Vector3.zero;
            }

            // If UI selected do not move character
            if (EventSystem.current.currentSelectedGameObject != null)
                return;

            // Reset movement
            movement = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                if (controller.useGravity || lockFlightViewRotationX)
                    movement += transform.forward;
                else
                    movement += cameraHolder.forward;
            }

            if (Input.GetKey(KeyCode.S))
            {
                if (controller.useGravity || lockFlightViewRotationX)
                    movement -= transform.forward;
                else
                    movement -= cameraHolder.forward;
            }

            if (Input.GetKey(KeyCode.A))
                movement += transform.TransformDirection(Vector3.left);

            if (Input.GetKey(KeyCode.D))
                movement += transform.TransformDirection(Vector3.right);

            if (!controller.useGravity)
            {
                if (spaceIsPressed)
                    movement += Vector3.up * jumpSpeed;
                if (Input.GetKey(KeyCode.C))
                    movement -= Vector3.up * jumpSpeed;
            }

            movement = Vector3.Normalize(movement);
            movement *= Input.GetKey(KeyCode.LeftShift) ? runSpeed : speed;
            controller.MovePosition(transform.position + movement * Time.fixedDeltaTime);

            if (spaceIsPressed && IsGrounded() && controller.useGravity)
                controller.velocity = Vector3.up * jumpSpeed;

            if (Input.GetKey(KeyCode.C) && controller.useGravity)
            {
                crouched = true;
                transform.GetChild(1).localScale = new Vector3(1f, 0.5f, 1f);
            }

            if (!Input.GetKey(KeyCode.C) && crouched)
                transform.GetChild(1).localScale = new Vector3(1f, 1f, 1f);
        }

        private bool IsGrounded()
        {
            return Physics.CheckCapsule(capsuleCollider.bounds.center,
                                        new Vector3(capsuleCollider.bounds.center.x,
                                                    capsuleCollider.bounds.min.y,
                                                    capsuleCollider.bounds.center.z),
                                        0.1f);
        }
    }
}
