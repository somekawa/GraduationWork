using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class BookList_importer : AssetPostprocessor
{
    private static readonly string filePath = "Assets/ExcelData/BookList.xls";
    private static readonly string[] sheetNames = { "BookList0","BookList1","BookList2","BookList3","BookList4", };
    
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
                    var exportPath = "Assets/Resources/BookList/" + sheetName + ".asset";
                    
                    // check scriptable object
                    var data = (BookList)AssetDatabase.LoadAssetAtPath(exportPath, typeof(BookList));
                    if (data == null)
                    {
                        data = ScriptableObject.CreateInstance<BookList>();
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
                        
                        var p = new BookList.Param();
			
					cell = row.GetCell(0); p.BookName = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(1); p.Word = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.AddNum = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.Price = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.Info = (cell == null ? "" : cell.StringCellValue);

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
