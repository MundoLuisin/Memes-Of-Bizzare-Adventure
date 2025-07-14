using UnityEngine;

public class Target : MonoBehaviour
{
    void Update()
    {
        Quaternion currentRotation = transform.rotation;
        Vector3 euler = currentRotation.eulerAngles;
        euler.y += 20f * Time.deltaTime; 
        transform.rotation = Quaternion.Euler(euler);
    }

}
