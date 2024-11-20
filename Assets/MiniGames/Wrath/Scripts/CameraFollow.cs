using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 主角的Transform
    public float smoothSpeed = 0.125f; // 平滑跟随的速度
    public Vector3 offset; // 相机与主角之间的偏移量

    public Vector2 minBounds; // 相机移动的最小边界
    public Vector2 maxBounds; // 相机移动的最大边界

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // 限制相机的移动范围
            float clampedX = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);
            float clampedY = Mathf.Clamp(smoothedPosition.y, minBounds.y, maxBounds.y);
            transform.position = new Vector3(clampedX, clampedY, smoothedPosition.z);
        }
    }
}
