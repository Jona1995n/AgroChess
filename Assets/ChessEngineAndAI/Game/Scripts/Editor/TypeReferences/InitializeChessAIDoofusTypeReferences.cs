using UnityEditor;
using TypeReferences.Editor;

namespace ChessEngine.Game.Editor
{
	[InitializeOnLoad]
	public static class InitializeChessAIDoofusTypeReferences
	{
		// Constructor(s).
		static InitializeChessAIDoofusTypeReferences()
		{
			// Register relevant chess AI assemblies.
			if (!ClassTypeReferenceEditorSettings.IncludeAssemblies.Contains("ChessEngine.AI.Doofus"))
				ClassTypeReferenceEditorSettings.IncludeAssemblies.Add("ChessEngine.AI.Doofus");
		}
	}
}