using UnityEditor;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
namespace SplineTools
{

    [CustomEditor(typeof(SplineMesh))]
    public class SplineMeshEditor : Editor
    {
        private bool autoGenerateMesh = true;

        public override void OnInspectorGUI()
        {
            SplineMesh splineMesh = (SplineMesh)target;

            // Auto Generate Mesh Button with toggle functionality
            GUIStyle toggleButtonStyle = new GUIStyle(GUI.skin.button);
            toggleButtonStyle.fontStyle = FontStyle.Bold;
            toggleButtonStyle.fixedHeight = 20;
            toggleButtonStyle.fixedWidth = 270;
            toggleButtonStyle.alignment = TextAnchor.MiddleCenter;

            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = autoGenerateMesh ? Color.cyan : Color.gray; // Green for enabled, gray for disabled

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Auto Generate Mesh On Spline Modified?", toggleButtonStyle))
            {
                autoGenerateMesh = !autoGenerateMesh;

                if (autoGenerateMesh)
                {
                    EditorSplineUtility.AfterSplineWasModified += splineMesh.OnSplineModified;
                    splineMesh.GenerateMeshAlongSpline();
                }
                else
                {
                    EditorSplineUtility.AfterSplineWasModified -= splineMesh.OnSplineModified;
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // Reset GUI background color
            GUI.backgroundColor = originalColor;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Spline Mesh Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Spline Container Field with "Self" button
            EditorGUILayout.BeginHorizontal();
            splineMesh.splineContainer = (SplineContainer)EditorGUILayout.ObjectField("Spline Container", splineMesh.splineContainer, typeof(SplineContainer), true);

            if (GUILayout.Button("Self", GUILayout.Width(50)))
            {
                splineMesh.splineContainer = splineMesh.GetComponent<SplineContainer>();
                if (splineMesh.splineContainer == null)
                {
                    Debug.LogWarning("No SplineContainer found on this GameObject.");
                }
            }
            EditorGUILayout.EndHorizontal();
            // Segment Mesh Field
            splineMesh.segmentMesh = (Mesh)EditorGUILayout.ObjectField("Segment Mesh", splineMesh.segmentMesh, typeof(Mesh), false);

            //Mesh Name
            splineMesh.meshName = EditorGUILayout.TextField("Mesh Name", splineMesh.meshName);

            // Forward Axis Enum
            splineMesh.forwardAxis = (VectorAxis)EditorGUILayout.EnumPopup("Forward Axis", splineMesh.forwardAxis);

            EditorGUILayout.Space();

            //UV Resolution
            splineMesh.uniformUVs = EditorGUILayout.Toggle("Uniform UVs", splineMesh.uniformUVs);

            SerializedProperty UVresolutions = serializedObject.FindProperty("uvResolutions");
            if (splineMesh.splineContainer != null)
            {
                UVresolutions.arraySize = splineMesh.splineContainer.Splines.Count;
            }
            EditorGUILayout.PropertyField(UVresolutions);
            splineMesh.uvAxis = (VectorAxis)EditorGUILayout.EnumPopup("UV Axis", splineMesh.uvAxis);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Offsets", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            splineMesh.positionalAdjustment = EditorGUILayout.Vector3Field("Offset", splineMesh.positionalAdjustment);
            splineMesh.rotationAdjustment = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", splineMesh.rotationAdjustment.eulerAngles));
            splineMesh.scaleAdjustment = EditorGUILayout.Vector3Field("Scale", splineMesh.scaleAdjustment);

            EditorGUILayout.Space();

            // Generate Mesh Button with custom color
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fixedHeight = 30;
            buttonStyle.fixedWidth = 150;
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            GUI.backgroundColor = Color.green; // Change this to the desired color

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Generate", buttonStyle))
            {
                splineMesh.GenerateMeshAlongSpline();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // Reset GUI background color
            GUI.backgroundColor = originalColor;

            // Apply any changes to the serialized object
            serializedObject.ApplyModifiedProperties();
        }

    }

}
#endif
