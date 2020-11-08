using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3rdPersonCamera : MonoBehaviour
{
    public float cameraWallOffset;
    public Vector3 zoom;

    private GameObject m_camTarget;
    private Transform m_camTargetTransform;
    private Transform m_camTransform;
    private Camera m_cam;

    // 카메라가 캐릭터에 뒤에 있어야 할 최소 값ㄴ
    private float m_zoomBackLimit;

    // Y component : 카메라가 캐릭터에 얼만큼 위에 있어야 하는지
    // Z component : 카메라가 캐릭터에 뒤로 얼만큼 뒤에 있어야 하는지
    private Vector3 m_forwardOffset;

    private void InitPlayer3rdPersonCamera()
    {
        this.m_camTarget = GameObject.Find("Player");
        this.m_camTargetTransform = this.m_camTarget.transform;
        this.m_camTransform = GetComponent<Transform>();
        this.m_cam = GetComponent<Camera>();

        float maxTargetScale = Mathf.Max(this.m_camTargetTransform.localScale.x, this.m_camTargetTransform.localScale.y);
        maxTargetScale = Mathf.Max(maxTargetScale, this.m_camTargetTransform.localScale.z);

        this.m_zoomBackLimit = maxTargetScale;

        this.m_forwardOffset = new Vector3(0f, 0f, 1f);
    }

    void Start()
    {
        InitPlayer3rdPersonCamera();
    }

    private Vector3[] GetCameraNearPlaneWorldPositions()
    {
        Vector3[] nearPlanePositions = new Vector3[4];

        Vector3 camForward = this.m_camTransform.forward;
        Vector3 camUp = this.m_camTransform.up;
        Vector3 camRight = this.m_camTransform.right;

        float nearDist = this.m_cam.nearClipPlane;
        float fovRadian = Mathf.Deg2Rad * this.m_cam.fieldOfView;
        float nearPlaneHalfHeight = Mathf.Tan(fovRadian / 2.0f) * nearDist;
        float nearPlaneHalfWidth = nearPlaneHalfHeight * this.m_cam.aspect;

        Vector3 nearPlaneCenter = this.m_camTransform.position + camForward * nearDist;

        // left-upper
        nearPlanePositions[0] = nearPlaneCenter + camUp * nearPlaneHalfHeight - camRight * nearPlaneHalfWidth;

        // right-upper
        nearPlanePositions[1] = nearPlanePositions[0] + camRight * nearPlaneHalfWidth * 2.0f;

        // left-lower
        nearPlanePositions[2] = nearPlanePositions[0] - camUp * nearPlaneHalfHeight * 2.0f;

        // right-lower
        nearPlanePositions[3] = nearPlanePositions[2] + camRight * nearPlaneHalfWidth * 2.0f;

        return nearPlanePositions;
    }

    // https://www.gamasutra.com/blogs/EricUndersander/20131001/201382/Accurate_Collision_Zoom_for_Cameras.php
    private Vector3 HandleCollisionZoom(out bool positionChanged)
    {
        Vector3 camPos = this.m_camTransform.position;
        Vector3 targetPos = this.m_camTargetTransform.position;

        float camToTargetDist = Vector3.Magnitude(targetPos - camPos);
        float rayCastLength = camToTargetDist - m_zoomBackLimit;
        if (rayCastLength < 0.0f)
        {
            positionChanged = false;
            return camPos;
        }

        Vector3 camOut = Vector3.Normalize(targetPos - camPos);
        Vector3 nearestCamPos = targetPos - camOut * m_zoomBackLimit;
        float minHitFraction = 1.0f;

        Vector3[] nearPlanePoints = GetCameraNearPlaneWorldPositions();

        for (int i = 0; i < 4; i++)
        {
            Vector3 corner = nearPlanePoints[i];
            Vector3 offsetToCorner = corner - camPos;
            Vector3 rayStart = nearestCamPos + offsetToCorner;
            Vector3 rayEnd = corner;

            float rayMaxdistance = Vector3.Magnitude(rayEnd - rayStart);

            // a result between 0 and 1 indicates a hit along the ray segment
            RaycastHit hitInfo;
            if (Physics.Raycast(rayStart, (rayEnd - rayStart) / rayMaxdistance, out hitInfo, rayMaxdistance))
            {
                minHitFraction = Mathf.Min(minHitFraction, hitInfo.distance / rayMaxdistance);
            }
        }

        if (minHitFraction < 1.0f)
        {
            positionChanged = true;
            return nearestCamPos - camOut * (rayCastLength * minHitFraction + cameraWallOffset);
        }
        else
        {
            positionChanged = false;
            return camPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Target의 position과 rotation으로 넣고
        this.m_camTransform.position = this.m_camTargetTransform.position;
        this.m_camTransform.rotation = this.m_camTargetTransform.rotation;

        // 현재 rotation에 따라, zoom을 정해진 위치에서 하게 끔 하기 위해,
        // 기존 zoom을 회전시키고
        Vector3 rotatedZoom = this.m_camTransform.localRotation * zoom;
        Vector3 rotatedForward = this.m_camTransform.localRotation * this.m_forwardOffset;

        // 그것으로 카메라 position을 조정한다.
        this.m_camTransform.position += rotatedZoom;

        Vector3 targetPosition = this.m_camTargetTransform.position + rotatedForward;

        // 그리고 camera가 target 바라보도록 한다.
        this.m_camTransform.rotation = Quaternion.LookRotation(targetPosition - this.m_camTransform.position, Vector3.up);

        // 그리고 현재 그 카메라가 부딪히는지를 보고 collision을 해결한다.
        bool isPositionChange = false;
        this.m_camTransform.position = HandleCollisionZoom(out isPositionChange);

        // 만약 Collision을 처리하면서 position이 바뀌었다면, rotation도 다시 설정해준다.
        if (isPositionChange == true)
        {
            this.m_camTransform.rotation = Quaternion.LookRotation((targetPosition - this.m_camTransform.position), Vector3.up);
        }
    }
}
