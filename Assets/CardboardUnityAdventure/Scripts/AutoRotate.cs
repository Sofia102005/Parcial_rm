using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public float speed = 30f;

    void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}