using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class CookList_importer : AssetPostprocessor
{
    private static readonly string filePath = "Assets/ExcelData/CookList.xls";
    private static readonly string[] sheetNames = { "Cook0", };
    
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
                    var exportPath = "Assets/Resources/" + sheetName + ".asset";
                    
                    // check scriptable object
                    var data = (Cook0)AssetDatabase.LoadAssetAtPath(exportPath, typeof(Cook0));
                    if (data == null)
                    {
                        data = ScriptableObject.CreateInstance<Cook0>();
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
                        
                        var p = new Cook0.Param();

                        cell = row.GetCell(0); p.num = (int)(cell == null ? 0 : cell.NumericCellValue);
					    cell = row.GetCell(1); p.name = (cell == null ? "" : cell.StringCellValue);
					    cell = row.GetCell(2); p.info = (cell == null ? "" : cell.StringCellValue);
					    cell = row.GetCell(3); p.statusUp = (cell == null ? "" : cell.StringCellValue);
					    cell = row.GetCell(4); p.needFood = (cell == null ? "" : cell.StringCellValue);
					    cell = row.GetCell(5); p.needNum = (cell == null ? "" : cell.StringCellValue);
					    cell = row.GetCell(6); p.needMoney = (int)(cell == null ? 0 : cell.NumericCellValue);
					    cell = row.GetCell(7); p.eventNum = (int)(cell == null ? 0 : cell.NumericCellValue);

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
