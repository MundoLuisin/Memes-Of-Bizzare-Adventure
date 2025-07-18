using UnityEngine;
using System.Collections.Generic;

public class ObstructionHologram : MonoBehaviour
{
    public Transform player;
    public LayerMask obstructionMask;
    public Material holographicMat;

    private Dictionary<Renderer, Material[]> originalMats = new();
    private List<Renderer> currentObstructions = new();
    private List<GameObject> disabledObjects = new();

    void Update()
    {
        ClearOldObstructions();

        Vector3 dirToPlayer = player.position - transform.position;
        Ray ray = new Ray(transform.position, dirToPlayer);
        RaycastHit[] hits = Physics.RaycastAll(ray, dirToPlayer.magnitude, obstructionMask);

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                if (!originalMats.ContainsKey(rend))
                {
                    if (GameData.Instance.buildingsDisappear)
                    {
                        GameObject go = rend.gameObject;
                        if (go.activeSelf)
                        {
                            disabledObjects.Add(go);
                            go.SetActive(false);
                        }
                    }
                    else
                    {
                        originalMats[rend] = rend.materials;
                        Material[] holoArray = new Material[rend.materials.Length];
                        for (int i = 0; i < holoArray.Length; i++)
                            holoArray[i] = holographicMat;
                        rend.materials = holoArray;
                        currentObstructions.Add(rend);
                    }
                }
            }
        }
    }

    void ClearOldObstructions()
    {
        if (GameData.Instance.buildingsDisappear)
        {
            foreach (GameObject obj in disabledObjects)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
            disabledObjects.Clear();
        }
        else
        {
            foreach (Renderer r in currentObstructions)
            {
                if (originalMats.ContainsKey(r))
                    r.materials = originalMats[r];
            }
            currentObstructions.Clear();
            originalMats.Clear();
        }
    }
}
