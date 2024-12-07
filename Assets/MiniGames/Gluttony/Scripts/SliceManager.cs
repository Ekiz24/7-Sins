using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceManager : MonoBehaviour
{
    public LayerMask fruitLayer;
    private List<Vector3> slicePoints = new List<Vector3>();
    private Camera mainCamera;
    private LineRenderer lineRenderer;
    AudioManager audioManager;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        mainCamera = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.2f;       // 起始宽度
        lineRenderer.endWidth = 0.2f;         // 结束宽度
        lineRenderer.positionCount = 0;        // 初始化顶点数量
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Button Down");
            slicePoints.Clear();
            lineRenderer.positionCount = 0; // 清除当前线条
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // 确保 z 轴为 0

            // 检查鼠标移动距离
            if (slicePoints.Count == 0 || Vector3.Distance(slicePoints[slicePoints.Count - 1], mousePosition) > 0.01f)
            {
                slicePoints.Add(mousePosition); // 添加新点
                lineRenderer.positionCount = slicePoints.Count; // 更新位置计数
                lineRenderer.SetPosition(slicePoints.Count - 1, mousePosition); // 设置新点
                CheckSliceCollision(); // 检查切割碰撞
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse Button Up");
            StartCoroutine(ClearLineAfterDelay(5f)); // 5秒后清除线条
        }
    }

    private IEnumerator ClearLineAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        lineRenderer.positionCount = 0; // 清除线条
    }

    private void CheckSliceCollision()
    {
        if (slicePoints.Count < 2) return;

        for (int i = 0; i < slicePoints.Count - 1; i++)
        {
            Vector2 startPoint = slicePoints[i];
            Vector2 endPoint = slicePoints[i + 1];

            Debug.Log($"Checking slice from {startPoint} to {endPoint}");

            RaycastHit2D hit = Physics2D.Linecast(startPoint, endPoint, fruitLayer);
            if (hit.collider != null)
            {
                Debug.Log("Hit Fruit: " + hit.collider.name);
                audioManager.PlaySFX(audioManager.Slice);
                Fruit fruit = hit.collider.GetComponent<Fruit>();
                if (fruit != null)
                {
                    fruit.OnSliced();
                }
            }
            else
            {
                Debug.Log("No Hit");
            }
        }
        slicePoints.Clear(); // 清除切割路径
    }
}
