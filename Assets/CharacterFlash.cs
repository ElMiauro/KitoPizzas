using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterFlash : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private int flashCount = 5;
    [SerializeField] private bool useOpacityMethod = true;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private List<Material> originalMaterials = new List<Material>();
    private List<Material> flashMaterials = new List<Material>();

    private void Awake()
    {
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        if (useOpacityMethod)
        {
            SetupMaterials();
        }
    }

    private void SetupMaterials()
    {
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            foreach (Material material in renderer.materials)
            {
                originalMaterials.Add(new Material(material));
                Material flashMaterial = new Material(material);
                flashMaterial.color = new Color(flashMaterial.color.r, flashMaterial.color.g, flashMaterial.color.b, 0f);
                flashMaterials.Add(flashMaterial);
            }
        }
    }

    public void StartFlashing(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine(duration));
    }

    private IEnumerator FlashCoroutine(float duration)
    {
        float endTime = Time.time + duration;
        bool isVisible = true;

        while (Time.time < endTime)
        {
            if (useOpacityMethod)
            {
                SetMaterials(isVisible ? flashMaterials : originalMaterials);
            }
            else
            {
                SetRenderersEnabled(!isVisible);
            }

            isVisible = !isVisible;
            yield return new WaitForSeconds(flashDuration);
        }

        // Ensure the character is visible at the end
        if (useOpacityMethod)
        {
            SetMaterials(originalMaterials);
        }
        else
        {
            SetRenderersEnabled(true);
        }
    }

    private void SetMaterials(List<Material> materials)
    {
        int materialIndex = 0;
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                newMaterials[i] = materials[materialIndex];
                materialIndex++;
            }
            renderer.materials = newMaterials;
        }
    }

    private void SetRenderersEnabled(bool enabled)
    {
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            renderer.enabled = enabled;
        }
    }
}