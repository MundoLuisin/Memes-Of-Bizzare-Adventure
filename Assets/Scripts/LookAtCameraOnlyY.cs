using UnityEngine;

public class LookAtCameraOnlyY : MonoBehaviour
{
    void Update()
    {
        if (Camera.main != null)
        {
            Vector3 lookDirection = Camera.main.transform.position - transform.position;
            lookDirection.y = 0; // Ignora el eje Y
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}
