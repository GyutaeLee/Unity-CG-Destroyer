using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;

    private Transform m_transform;
    private float m_yaw;
    private float m_pitch;
    private float m_roll;
    private CharacterController m_characterController;

    private AnimationCurve m_mouseSensitivityCurve;

    private void InitPlayerController()
    {
        this.m_transform = GetComponent<Transform>();
        this.m_characterController = GetComponent<CharacterController>();
        this.m_mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));
    }

    void Start()
    {
        InitPlayerController();
    }

    Vector3 GetInputTranslationDirection()
    {
        Vector3 direction = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }
        return direction;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 translation = Vector3.zero;

        // Hide and lock cursor when right mouse button pressed
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Unlock and show cursor when right mouse button released
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Translation
        translation = GetInputTranslationDirection() * Time.deltaTime * this.moveSpeed;

        // Speed up movement when shift key held
        if(Input.GetKey(KeyCode.LeftShift))
        {
            translation *= 10.0f;
        }

        translation = Quaternion.Euler(m_pitch, m_yaw, this.m_transform.eulerAngles.z) * translation;


        if (this.m_characterController.isGrounded == false)
        {
            this.m_characterController.Move(Physics.gravity * Time.deltaTime);
        }

        this.m_characterController.Move(translation);

        // Rotation
        if (Input.GetMouseButton(1))
        {
            Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * -1);

            float mouseSensitivityFactor = this.m_mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

            this.m_yaw += mouseMovement.x * mouseSensitivityFactor;
            this.m_pitch += mouseMovement.y * mouseSensitivityFactor;
        }

        // Framerate-independent interpolation
        // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
        const float kPositionLerpTime = 0.2f;
        const float kRotationLerpTime = 0.01f;
        float positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / kPositionLerpTime) * Time.deltaTime);
        float rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / kRotationLerpTime) * Time.deltaTime);
        float lerpYaw = Mathf.Lerp(this.m_transform.eulerAngles.y, this.m_yaw, rotationLerpPct);
        float lerpPitch = Mathf.Lerp(this.m_transform.eulerAngles.x, this.m_pitch, rotationLerpPct);
        float lerpRoll = this.m_transform.eulerAngles.z;
        this.m_transform.eulerAngles = new Vector3(lerpPitch, lerpYaw, lerpRoll);
    }
}
