using UnityEditor;
using TypeReferences.Editor;

namespace ChessEngine.Game.Editor
{
	[InitializeOnLoad]
	public static class InitializeChessTypeReferences
	{
		// Constructor(s).
		static InitializeChessTypeReferences()
		{
			// Register relevant chess assemblies.
			if (!ClassTypeReferenceEditorSettings.IncludeAssemblies.Contains("ChessEngine"))
				ClassTypeReferenceEditorSettings.IncludeAssemblies.Add("ChessEngine");
		}
	}
}