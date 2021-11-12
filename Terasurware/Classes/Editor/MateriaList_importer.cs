using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class MateriaList_importer : AssetPostprocessor
{
    private static readonly string filePath = "Assets/ExcelData/MateriaList.xls";
    private static readonly string[] sheetNames = { "M_Field0","M_Field1","M_Field2","M_Field3","M_Field4","M_Field5", };
    
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
                    var exportPath = "Assets/Resources/MateriaList/" + sheetName + ".asset";
                    
                    // check scriptable object
                    var data = (MateriaList)AssetDatabase.LoadAssetAtPath(exportPath, typeof(MateriaList));
                    if (data == null)
                    {
                        data = ScriptableObject.CreateInstance<MateriaList>();
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
                        
                        var p = new MateriaList.Param();
			
					cell = row.GetCell(0); p.MateriaName = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(1); p.Price_Buy = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.Price_Sell = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.Explanation = (cell == null ? "" : cell.StringCellValue);

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
