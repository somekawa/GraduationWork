using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class RecipeList_importer : AssetPostprocessor
{
    private static readonly string filePath = "Assets/ExcelData/RecipeList.xls";
    private static readonly string[] sheetNames = { "Recipe0","Recipe1","Recipe2","Recipe3", };
    
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets)
        {
            if (!filePath.Equals(asset))
                continue;

            using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}

                foreach (string sheetName in sheetNames)
                {
                    var exportPath = "Assets/Resources/RecipeList/" + sheetName + ".asset";
                    
                    // check scriptable object
                    var data = (RecipeList)AssetDatabase.LoadAssetAtPath(exportPath, typeof(RecipeList));
                    if (data == null)
                    {
                        data = ScriptableObject.CreateInstance<RecipeList>();
                        AssetDatabase.CreateAsset((ScriptableObject)data, exportPath);
                        data.hideFlags = HideFlags.NotEditable;
                    }
                    data.param.Clear();

					// check sheet
                    var sheet = book.GetSheet(sheetName);
                    if (sheet == null)
                    {
                        Debug.LogError("[QuestData] sheet not found:" + sheetName);
                        continue;
                    }

                	// add infomation
                    for (int i=1; i<= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        ICell cell = null;
                        
                        var p = new RecipeList.Param();
			
					cell = row.GetCell(0); p.ItemName = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(1); p.WantMateria1 = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.WantMateria2 = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.WantMateria3 = (cell == null ? "" : cell.StringCellValue);

                        data.param.Add(p);
                    }
                    
                    // save scriptable object
                    ScriptableObject obj = AssetDatabase.LoadAssetAtPath(exportPath, typeof(ScriptableObject)) as ScriptableObject;
                    EditorUtility.SetDirty(obj);
                }
            }

        }
    }
}
