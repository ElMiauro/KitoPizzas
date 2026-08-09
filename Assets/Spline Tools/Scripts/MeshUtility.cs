using UnityEngine;

#if UNITY_EDITOR
namespace SplineTools
{
    public static class MeshUtility
    {
        //This script is used to add adjustments to an existing Mesh
        //Useful when dealing with multiple file types
        //.fbx files may require scaleAdjustment to be (100f, 100f, 100f)
        public static Mesh NormalizeMesh(Mesh mesh, Quaternion rotationAdjustment, Vector3 scaleAdjustment)
        {
            if (mesh == null)
            {
                Debug.LogError("Mesh is null.");
                return null;
            }

            Mesh normalizedMesh = Object.Instantiate(mesh);

            Vector3[] vertices = normalizedMesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                //Apply scale first
                vertices[i] = Vector3.Scale(vertices[i], scaleAdjustment);

                //Apply rotational offset
                vertices[i] = rotationAdjustment * vertices[i];
            }
            normalizedMesh.vertices = vertices;

            Vector3[] normals = normalizedMesh.normals;
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = rotationAdjustment * normals[i];
            }
            normalizedMesh.normals = normals;

            normalizedMesh.RecalculateBounds();
            normalizedMesh.RecalculateTangents();

            return normalizedMesh;
        }
    }
}
#endif