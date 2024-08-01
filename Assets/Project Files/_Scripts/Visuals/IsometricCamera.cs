using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;
    public Transform followObject;
    [SerializeField] private float smoothTime = 0.3f;


    // Update is called once per frame
    void Update()
    {
        if (followObject == null) return;


        Vector3 targetPos = followObject.position;

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }
}
