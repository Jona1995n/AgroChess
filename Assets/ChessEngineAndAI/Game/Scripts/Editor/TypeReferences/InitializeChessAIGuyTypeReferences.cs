using UnityEditor;
using TypeReferences.Editor;

namespace ChessEngine.Game.Editor
{
	[InitializeOnLoad]
	public static class InitializeChessAIGuyTypeReferences
	{
		// Constructor(s).
		static InitializeChessAIGuyTypeReferences()
		{
			// Register relevant chess AI assemblies.
			if (!ClassTypeReferenceEditorSettings.IncludeAssemblies.Contains("ChessEngine.AI.Guy"))
				ClassTypeReferenceEditorSettings.IncludeAssemblies.Add("ChessEngine.AI.Guy");
		}
	}
}