#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class CustomImportSettings : AssetPostprocessor
{

    string humanoidpath = "models";





    void OnPreprocessAsset()
    {
        PlayerSettings.stripUnusedMeshComponents = false;
        AssetImporter importer = (AssetImporter)assetImporter;
        if (importer.assetPath.Contains("iBEGOOExporter/Resources"))
        {
            try
            {
                //end of directory path, start of file name
                int fileNamePos = importer.assetPath.LastIndexOf("/", System.StringComparison.CurrentCulture);

                //end of file name, start of file extension
                int fileExtPos = importer.assetPath.LastIndexOf(".", System.StringComparison.CurrentCulture);
                //parent directory with trailing slash
                string filePath = importer.assetPath.Substring(0, fileNamePos + 1);

                //isolated file name
                string fileName = importer.assetPath.Substring(fileNamePos + 1, fileExtPos - filePath.Length);
                //extension with "."
                string fileExt = importer.assetPath.Substring(fileExtPos);


                if (filePath.ToLower().Contains(humanoidpath) && fileExt.ToLower().Contains("fbx"))
                {
                    ModelImporter modelImporter = (ModelImporter)importer;
                    modelImporter.animationType = ModelImporterAnimationType.Human;
                    modelImporter.ExtractTextures(assetPath);
                }
                if ((filePath.ToLower().Contains("props") || filePath.ToLower().Contains("sets") || filePath.ToLower().Contains("models")) && fileExt.ToLower().Contains("prefab")) 
                {  
                    assetImporter.assetBundleName = fileName;
                }

            }
            catch
            {
                //
                int folderNamePos = importer.assetPath.Substring(0, importer.assetPath.Length - 2).LastIndexOf("/", System.StringComparison.CurrentCulture);
                string folderName = importer.assetPath.Substring(folderNamePos + 1, importer.assetPath.Length - 1 - folderNamePos);

                //assetImporter.assetBundleName = folderName;
            }


            //assetImporter.assetBundleName = fileName;
        }


        AddAlwaysIncludedShader("Standard");




    }

    public static void AddAlwaysIncludedShader(string shaderName)
    {
        var shader = Shader.Find(shaderName);
        if (shader == null)
            return;

        var graphicsSettingsObj = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
        var serializedObject = new SerializedObject(graphicsSettingsObj);
        var arrayProp = serializedObject.FindProperty("m_AlwaysIncludedShaders");
        bool hasShader = false;
        for (int i = 0; i < arrayProp.arraySize; ++i)
        {
            var arrayElem = arrayProp.GetArrayElementAtIndex(i);
            if (shader == arrayElem.objectReferenceValue)
            {
                hasShader = true;
                break;
            }
        }

        if (!hasShader)
        {
            int arrayIndex = arrayProp.arraySize;
            arrayProp.InsertArrayElementAtIndex(arrayIndex);
            var arrayElem = arrayProp.GetArrayElementAtIndex(arrayIndex);
            arrayElem.objectReferenceValue = shader;

            serializedObject.ApplyModifiedProperties();

            AssetDatabase.SaveAssets();
        }
    }


}
#endif
