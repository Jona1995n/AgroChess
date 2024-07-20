using UnityEditor;
using TypeReferences.Editor;

namespace ChessEngine.Game.Editor
{
	[InitializeOnLoad]
	public static class InitializeChessAITypeReferences
	{
		// Constructor(s).
		static InitializeChessAITypeReferences()
		{
			// Register relevant chess AI assemblies.
			if (!ClassTypeReferenceEditorSettings.IncludeAssemblies.Contains("ChessEngine.AI"))
				ClassTypeReferenceEditorSettings.IncludeAssemblies.Add("ChessEngine.AI");
		}
	}
}