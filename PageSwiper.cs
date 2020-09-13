using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PageSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private Vector3 panelPosition;
    [SerializeField]
    private float changePagePercent = 0.2f;
    [SerializeField]
    private float easing = 0.5f;
    [SerializeField]
    private Boundary posXClamp;
    [SerializeField]
    private int slideCount;

    private void Start()
    {
        panelPosition = transform.position;
        InitBoundary();
    }

    private void InitBoundary()
    {
        posXClamp.min = -(Screen.width * (slideCount - 1));
        posXClamp.max = 0;
    }

    public void OnDrag(PointerEventData data)
    {
        float difference = data.position.x - data.pressPosition.x;
        Vector3 newPosition = panelPosition - new Vector3(difference, 0, 0);
        newPosition.x = Mathf.Clamp(newPosition.x, posXClamp.min, posXClamp.max);
        transform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData data)
    {
        Debug.Log($"Press: {data.pressPosition.x} and Current: {data.position.x}");
        float percentage = (data.position.x - data.pressPosition.x) / Screen.width;
        float absPercentage = percentage > 0 ? percentage : -percentage;

        if (absPercentage >= changePagePercent)
        {
            Vector3 newLocation = CalculateNewPosition(percentage);

            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
        }
        else
        {
            StartCoroutine(SmoothMove(transform.position, panelPosition, easing));
        }
    }

    private Vector3 CalculateNewPosition(float percentage)
    {
        Vector3 newPosition = panelPosition;

        if (percentage > 0)
        {
            newPosition += new Vector3(-Screen.width, 0, 0);
        }
        else if (percentage < 0)
        {
            newPosition += new Vector3(Screen.width, 0, 0);
        }

        newPosition.x = Mathf.Clamp(newPosition.x, posXClamp.min, posXClamp.max);
        return newPosition;
    }

    private IEnumerator SmoothMove(Vector3 startPos, Vector3 endPos, float seconds)
    {
        float t = 0f;
        while (t <= 1.0f)
        {
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
            
            yield return null;
        }

        panelPosition = endPos;
    }
}

public struct Boundary
{
    public float min;
    public float max;
}
