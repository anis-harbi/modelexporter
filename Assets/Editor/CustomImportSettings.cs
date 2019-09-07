#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Set the scale of all the imported models to  "globalScaleModifier"
// and dont generate materials for the imported objects

public class CustomImportSettings : AssetPostprocessor
{
    //float globalScaleModifier = 0.0028f;

    void OnPreprocessModel()
    {
        ModelImporter importer = (ModelImporter)assetImporter;

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



        //importer.globalScale = globalScaleModifier;
        //importer.importMaterials = false;
        importer.animationType = ModelImporterAnimationType.Human;
        importer.ExtractTextures(assetPath);
        
        assetImporter.assetBundleName = fileName;
        //AssetBundleCreator.BuildBundles();
    }
}
#endif
