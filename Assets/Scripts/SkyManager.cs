using UnityEngine;
public class SkyManager : MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    private void Update()
    {
        float currentRotation = RenderSettings.skybox.GetFloat("_Rotation");
        currentRotation += rotationSpeed * Time.deltaTime;
        RenderSettings.skybox.SetFloat("_Rotation", currentRotation);
    }
}
