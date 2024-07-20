using UnityEditor;
using TypeReferences.Editor;

namespace ChessEngine.Game.Editor
{
	[InitializeOnLoad]
	public static class InitializeChessAIRiddleTypeReferences
	{
		// Constructor(s).
		static InitializeChessAIRiddleTypeReferences()
		{
			// Register relevant chess AI assemblies.
			if (!ClassTypeReferenceEditorSettings.IncludeAssemblies.Contains("ChessEngine.AI.Riddle"))
				ClassTypeReferenceEditorSettings.IncludeAssemblies.Add("ChessEngine.AI.Riddle");
		}
	}
}