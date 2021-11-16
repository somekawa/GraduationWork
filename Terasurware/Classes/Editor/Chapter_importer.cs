using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class Chapter_importer : AssetPostprocessor
{
    private static readonly string filePath = "Assets/ExcelData/Chapter.xls";
    private static readonly string[] sheetNames = {"0","1","2","3","4","5","6","7","8","9","10","11","12","100"};
    
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
                    var exportPath = "Assets/Resources/Chapter/Chapter" + sheetName + ".asset";

                    // check scriptable object
                    var data = (ChapterList)AssetDatabase.LoadAssetAtPath(exportPath, typeof(ChapterList));
                    if (data == null)
                    {
                        data = ScriptableObject.CreateInstance<ChapterList>();
                        AssetDatabase.CreateAsset((ScriptableObject)data, exportPath);
                        data.hideFlags = HideFlags.NotEditable;
                    }
                    data.param.Clear();

					// check sheet
                    var sheet = book.GetSheet("Chapter" + sheetName);
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
                        
                        var p = new ChapterList.Param();
			
					cell = row.GetCell(0); p.name1 = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(1); p.name2 = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.face = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.message = (cell == null ? "" : cell.StringCellValue);
                    cell = row.GetCell(4); p.eventNum = (int)(cell == null ? 0 : cell.NumericCellValue);

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
