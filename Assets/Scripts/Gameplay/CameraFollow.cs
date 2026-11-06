using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow I;
    public Transform target;
    public Vector3 offset = new Vector3(0f, 3.4f, -7.2f);
    public float lookAhead = 9f;
    float shake, smoothX, fov = 62f;
    Camera cam;

    void Awake() { I = this; cam = GetComponent<Camera>(); }
    public void Shake(float m) { shake = Mathf.Max(shake, m); }

    void LateUpdate()
    {
        if (target == null) return;
        float dt = Time.deltaTime;
        var tp = target.position;
        smoothX = Mathf.Lerp(smoothX, tp.x, 1f - Mathf.Exp(-8f * dt));
        float mouse = Mathf.Clamp((Input.mousePosition.x / Mathf.Max(1, Screen.width) - 0.5f) * 2f, -1f, 1f) * SaveManager.MouseSens * 2.5f;
        var pos = new Vector3(smoothX * 0.65f + mouse * 0.3f, offset.y + tp.y * 0.4f, tp.z + offset.z);
        if (shake > 0f) { pos += Random.insideUnitSphere * shake; shake = Mathf.MoveTowards(shake, 0f, dt * 1.5f); }
        transform.position = pos;
        transform.LookAt(new Vector3(smoothX * 0.4f + mouse, 1.3f + tp.y * 0.2f, tp.z + lookAhead));
        bool boost = PowerUpManager.I != null && PowerUpManager.I.Active(PowerUpType.Speed);
        fov = Mathf.Lerp(fov, boost ? 74f : 62f, 1f - Mathf.Exp(-4f * dt));
        cam.fieldOfView = fov;
    }
}
