using UnityEditor;

namespace Script_Templates.State
{
    public static class StateScriptGenerator
    {
        [MenuItem("Assets/Create/Versus Fighting/Player State")]
        public static void CreatePlayerState()
        {
            string templatePath = "Assets/Scripts/Script Templates/State/StateScriptTemplate.txt";
            string defaultName = "NewPlayerState.cs";

            string path = EditorUtility.SaveFilePanelInProject(
                "Create Player State",
                defaultName,
                "cs",
                "Enter a file name for the new Player State script."
            );

            if (!string.IsNullOrEmpty(path))
            {
                string templateContent = System.IO.File.ReadAllText(templatePath);
                string className = System.IO.Path.GetFileNameWithoutExtension(path);
                templateContent = templateContent.Replace("#SCRIPTNAME#", className);

                System.IO.File.WriteAllText(path, templateContent);
                

                AssetDatabase.Refresh();

                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                Selection.activeObject = asset;
            }
        }
    }
}