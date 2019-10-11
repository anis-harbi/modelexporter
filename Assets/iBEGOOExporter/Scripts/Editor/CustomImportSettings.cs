#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class CustomImportSettings : AssetPostprocessor
{

    string humanoidpath = "models";

    void OnPreprocessModel()
    {
       
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


                if (filePath.Contains(humanoidpath) && fileExt.ToLower().Contains("fbx"))
                {
                    ModelImporter modelImporter = (ModelImporter)importer;
                    modelImporter.animationType = ModelImporterAnimationType.Human;
                    modelImporter.ExtractTextures(assetPath);
                }
                assetImporter.assetBundleName = fileName;
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



    }


}
#endif
