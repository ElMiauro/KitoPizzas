using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Splines;
#endif
using UnityEngine;
using UnityEngine.Splines;

namespace SplineTools
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SplineMeshResolution : MonoBehaviour
    {
#if UNITY_EDITOR

        public SplineContainer splineContainer;

        public Mesh segmentMesh;    //The mesh to be manipulated
        public string meshName = "Spline Mesh"; //Name of the mesh. Doesn't really matter

        public VectorAxis forwardAxis = VectorAxis.Y;   //The axis along which the mesh extends (local)

        public bool uniformUVs = true;  //Whether UVs are uniformly spread out, or based on the spline points
        public float[] uvResolutions = { 1.0f }; //Variable to control UV resolution
        public VectorAxis uvAxis = VectorAxis.Y;    //Axis for the UV to be stretched.

        public Vector3 positionalAdjustment = Vector3.zero; //Positional adjustment (Useful for adding legs)
        public Quaternion rotationAdjustment = Quaternion.identity; // Rotation adjustment (for OBJ files)
        public Vector3 scaleAdjustment = Vector3.one; //Scale adjustment (for different file types)

        private MeshFilter meshFilter;

        public int[] resolution = { 10 };   //Here resolution is an array, size determined by Editor script


        //Subscribe to the AfterSplineWasModified event
        private void OnEnable()
        {
            EditorSplineUtility.AfterSplineWasModified += OnSplineModified;
        }

        //Unsubscribe from the AfterSplineWasModified event
        private void OnDisable()
        {
            EditorSplineUtility.AfterSplineWasModified -= OnSplineModified;
        }

        //This fuction is called to generate the spline mesh
        public void GenerateMeshAlongSpline()
        {
            if (splineContainer == null || segmentMesh == null)
            {
                Debug.LogError("SplineContainer or Mesh is not assigned.");
                return;
            }

            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }

            // Store the combined mesh data
            List<Vector3> combinedVertices = new List<Vector3>();
            List<Vector3> combinedNormals = new List<Vector3>();
            List<Vector2> combinedUVs = new List<Vector2>();
            List<int>[] combinedSubmeshTriangles = new List<int>[segmentMesh.subMeshCount];
            for (int i = 0; i < segmentMesh.subMeshCount; i++)
            {
                combinedSubmeshTriangles[i] = new List<int>();
            }
            int combinedVertexOffset = 0;

            int splineCounter = 0;

            // Normalize the segment mesh
            Mesh normalizedSegmentMesh = MeshUtility.NormalizeMesh(segmentMesh, rotationAdjustment, scaleAdjustment);

            foreach (Spline spline in splineContainer.Splines)
            {
                List<BezierKnot> knots = new List<BezierKnot>(spline.Knots); // Convert knots to list

                // Lists to store mesh data
                List<Vector3> vertices = new List<Vector3>();
                List<int>[] submeshTriangles = new List<int>[normalizedSegmentMesh.subMeshCount];
                List<Vector3> normals = new List<Vector3>();
                List<Vector2> uvs = new List<Vector2>();

                for (int i = 0; i < normalizedSegmentMesh.subMeshCount; i++)
                {
                    submeshTriangles[i] = new List<int>();
                }

                int segmentCount = knots.Count - 1; // Number of segments along the spline


                // Loop through each resolution of the spline
                for (int i = 0; i < resolution[splineCounter]; i++)
                {
                    float meshBoundsDistance = Mathf.Abs(GetRequiredAxis(normalizedSegmentMesh.bounds.size));

                    List<float> vertexRatios = new List<float>();
                    List<Vector3> vertexOffsets = new List<Vector3>();

                    // Calculate vertex ratios and offsets
                    foreach (Vector3 v in normalizedSegmentMesh.vertices)
                    {
                        float ratio = Mathf.Abs(GetRequiredAxis(v)) / meshBoundsDistance;
                        Vector3 offset = GetRequiredOffset(v);
                        vertexRatios.Add(ratio);
                        vertexOffsets.Add(offset);

                    }

                    int counter = 0;
                    foreach (Vector3 v in normalizedSegmentMesh.vertices)
                    {
                        float point = (i / (float)resolution[splineCounter]) + (vertexRatios[counter] * (1 / (float)resolution[splineCounter]));
                        Vector3 splinePosition = spline.EvaluatePosition(point);
                        Vector3 tangent = spline.EvaluateTangent(point);

                        Quaternion splineRotation = Quaternion.LookRotation(tangent, Vector3.up);


                        Vector3 transformedPosition = splinePosition + splineRotation * vertexOffsets[counter];
                        vertices.Add(transformedPosition + positionalAdjustment);
                        counter++;
                    }

                    // Add transformed normals
                    for (int j = 0; j < normalizedSegmentMesh.normals.Length; j++)
                    {
                        Vector3 normal = normalizedSegmentMesh.normals[j];
                        float point = (i / (float)resolution[splineCounter]) + (vertexRatios[j] * (1 / (float)resolution[splineCounter]));
                        Vector3 tangent = spline.EvaluateTangent(point);

                        Quaternion splineRotation = Quaternion.LookRotation(tangent, Vector3.up);
                        Vector3 transformedNormal = splineRotation * normal;

                        normals.Add(transformedNormal);
                    }

                    // Add triangles to each submesh
                    for (int submeshIndex = 0; submeshIndex < normalizedSegmentMesh.subMeshCount; submeshIndex++)
                    {
                        int[] submeshIndices = normalizedSegmentMesh.GetTriangles(submeshIndex);
                        for (int k = 0; k < submeshIndices.Length; k += 3)
                        {
                            combinedSubmeshTriangles[submeshIndex].Add(submeshIndices[k] + combinedVertexOffset);
                            combinedSubmeshTriangles[submeshIndex].Add(submeshIndices[k + 2] + combinedVertexOffset);
                            combinedSubmeshTriangles[submeshIndex].Add(submeshIndices[k + 1] + combinedVertexOffset);
                        }
                    }

                    // Add UVs with UV resolution
                    for (int j = 0; j < normalizedSegmentMesh.uv.Length; j++)
                    {
                        Vector2 uv = normalizedSegmentMesh.uv[j];
                        float point;
                        if (uniformUVs)
                        {
                            point = (i / (float)resolution[splineCounter]) + (vertexRatios[j] * (1 / (float)resolution[splineCounter]));
                        }
                        else
                        {
                            point = (i / (float)segmentCount) + (vertexRatios[j] * (1 / (float)segmentCount));
                        }

                        Vector2 splineUV = MakeUVs(uv, point, splineCounter); // Apply UV resolution
                        uvs.Add(splineUV);
                    }

                    combinedVertexOffset += normalizedSegmentMesh.vertexCount;


                }
                // Combine current spline mesh data into the combined lists
                combinedVertices.AddRange(vertices);
                combinedNormals.AddRange(normals);
                combinedUVs.AddRange(uvs);
                splineCounter++;
            }

            // Create the final combined mesh
            Mesh generatedMesh = new Mesh();
            generatedMesh.name = meshName;
            generatedMesh.vertices = combinedVertices.ToArray();
            generatedMesh.normals = combinedNormals.ToArray();
            generatedMesh.uv = combinedUVs.ToArray();
            generatedMesh.subMeshCount = segmentMesh.subMeshCount;
            for (int submeshIndex = 0; submeshIndex < segmentMesh.subMeshCount; submeshIndex++)
            {
                generatedMesh.SetTriangles(combinedSubmeshTriangles[submeshIndex].ToArray(), submeshIndex);
            }

            // Assign mesh to MeshFilter
            meshFilter.mesh = generatedMesh;

            // Recalculate bounds and normals
            generatedMesh.RecalculateBounds();
            generatedMesh.RecalculateNormals();
            generatedMesh.RecalculateTangents();

        }

        private Vector2 MakeUVs(Vector2 uv, float point, int splineCount)
        {
            switch (uvAxis)
            {
                case VectorAxis.X:
                    return new Vector2(point * uvResolutions[splineCount], uv.y);
                default:
                    return new Vector2(uv.x, point * uvResolutions[splineCount]);
            }
        }

        private Vector3 GetRequiredOffset(Vector3 v)
        {
            switch (forwardAxis)
            {
                case VectorAxis.X:
                    return new Vector3(v.y, v.z, 0f);
                case VectorAxis.Y:
                    return new Vector3(v.x, v.z, 0f);
                default:
                    return new Vector3(v.x, v.z, 0f);
            }
        }

        private float GetRequiredAxis(Vector3 vector)
        {
            switch (forwardAxis)
            {
                case VectorAxis.X:
                    return vector.x;
                case VectorAxis.Y:
                    return vector.y;
                default:
                    return vector.y;
            }
        }

        public void OnSplineModified(Spline spline)
        {
            if (spline == null || segmentMesh == null) return;
            if (spline == splineContainer.Spline)
            {
                GenerateMeshAlongSpline();
            }
        }
#endif
    }
}

