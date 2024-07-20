using UnityEngine;
using UnityEditor;

namespace ChessEngine.Game.Editor
{
    /// <summary>
    /// A custom inspector for VisualChessTableTiles.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [CustomEditor(typeof(VisualChessTableTile)), CanEditMultipleObjects]
    public class VisualChessTableTileEditor : UnityEditor.Editor
    {
		#region Unity Callback(s)
		// Unity callback(s).
		public override void OnInspectorGUI()
        {
            // Draw the default inspector.
            DrawDefaultInspector();

            // Draw custom inspector elements for VisualChessTableTile.
            VisualChessTableTile visualTile = (VisualChessTableTile)target;

            // Section: Edit mode only UI.
            if (!Application.isPlaying)
            {
                // Button that finds all Renderer(s) in the 'gameObject' and generates a bounding box that encapsulates them.
                if (GUILayout.Button("Compute Tile Bounds"))
                {
                    // Ask for confirmation.
                    if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to compute tile bounds? This will overwrite 'tileLength', 'tileWidth', and 'tileHeight'.\n\nUndo is supported.", "Yes", "No"))
                    {
                        // Generate the bounding box for the tile and use it to populate 'tileLength', 'tileWidth', and 'tileHeight'. Support undoing.
                        UnityEditor.Undo.RecordObject(visualTile, "Compute Tile Bounds");
                        Bounds boundingBox = GenerateBoundingBoxForMeshVertices(visualTile.gameObject);

                        // Populate 'tileLength', 'tileWidth', and 'tileHeight' with the bounding box dimensions.
                        Vector3 dimensions = boundingBox.size;
                        visualTile.tileLength = dimensions.z;
                        visualTile.tileWidth = dimensions.x;
                        visualTile.tileHeight = dimensions.y;

                        // Mark the object as dirty.
                        EditorUtility.SetDirty(visualTile);

                        // Name undo group.
                        UnityEditor.Undo.SetCurrentGroupName("Compute Tile Bounds");
                    }
                }
            }
        }
		#endregion

		#region Private Method(s)
		/// <summary>Generates a bounding box that encapsulates all vertices found in meshes belonging to pGameObject.</summary>
		/// <param name="pGameObject"></param>
		/// <returns>a bounding box that encapsulates all vertices found in the meshes belonging to pGameObject.</returns>
		Bounds GenerateBoundingBoxForMeshVertices(GameObject pGameObject)
		{
			MeshFilter[] meshFilters = pGameObject.GetComponentsInChildren<MeshFilter>();
			if (meshFilters.Length == 0)
				return new Bounds(Vector3.zero, Vector3.zero);
			
			Bounds combinedBounds = new Bounds(meshFilters[0].transform.position, Vector3.zero);

			foreach (MeshFilter meshFilter in meshFilters)
			{
				Mesh mesh = meshFilter.sharedMesh;
				Vector3[] vertices = mesh.vertices;

				for (int i = 0; i < vertices.Length; ++i)
				{
					vertices[i] = meshFilter.transform.TransformPoint(vertices[i]);
				}

				combinedBounds.Encapsulate(new Bounds(vertices[0], Vector3.zero));

				for (int i = 1; i < vertices.Length; ++i)
				{
					combinedBounds.Encapsulate(vertices[i]);
				}
			}

			return combinedBounds;
		}
		#endregion
    }
}
