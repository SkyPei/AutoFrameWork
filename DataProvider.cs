using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace AutoFrameWork
{
    internal class DataProvider
    {

        private static readonly object obj = new object();
        private string dataFile;
        private string sheetName;

        private IWorkbook wb;
        private ISheet sheet;

        internal DataProvider(string dataFile, string sheetName)
        {
            this.dataFile = dataFile;
            this.sheetName = sheetName;
        }

        internal static (IWorkbook, ISheet) GetWorkBookAndSheet(string excelFile, string sheetName)
        {
            bool isexist = false;
            isexist = File.Exists(excelFile);
            if (!isexist)
            {
                throw new FileNotFoundException($"Excel File {{{excelFile}}} is not found.");
            }

            IWorkbook wb = null;
            using (FileStream fs = File.Open(excelFile, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                string ext = Path.GetExtension(excelFile);
                if (string.Equals(".xlsx", ext, StringComparison.OrdinalIgnoreCase))
                {
                    wb = new XSSFWorkbook(fs);
                }
                else
                {
                    wb = new HSSFWorkbook(fs);
                }
            }

            ISheet sheet = string.IsNullOrEmpty(sheetName) ? wb.GetSheetAt(0) : wb.GetSheet(sheetName);

            if (sheet == null)
            {
                throw new System.Exception($"Cannot find excel sheet {{{sheetName}}}");
            }

            return (wb, sheet);

        }
        internal ProviderModel ExtractData(string bindingName, string scriptid)
        {
            lock (obj)
            {

                ProviderModel dt = null;
                (IWorkbook wb, ISheet sheet) excelFile = GetWorkBookAndSheet(dataFile, sheetName);
                wb = excelFile.wb;
                sheet = excelFile.sheet;

                //get columns
                int lastrow = sheet.LastRowNum;



                if (lastrow > 0)
                {
                    int lastcol = sheet.GetRow(0).LastCellNum;
                    dt = new ProviderModel();


                    for (int i = 0; i < lastcol; i++)
                    {
                        string name = sheet.GetRow(0).GetCell(i)?.ToString().Trim().ToLower();
                        if (!string.IsNullOrEmpty(name))
                        {

                            if (dt.ColumnMapping.Keys.Contains(name))
                            {
                                throw new Exception.ParseExcelException("Duplicate column name.");
                            }


                            dt.ColumnMapping.Add(name, i);


                        }

                    }



                    //get data

                    for (int x = 1; x <= lastrow; x++)
                    {
                        IRow row = sheet.GetRow(x);
                        bool isbreak = false;
                        if (row == null)
                        {
                            continue;
                        }

                        DataModel dr = new DataModel();
                        dr.RowNumber = x;

                        foreach (var y in dt.ColumnMapping.Keys)
                        {
                            string value = row.GetCell(dt.ColumnMapping[y])?.ToString();
                            if ("isenabled" == y)
                            {
                                if ("y" != value?.ToLower() && "yes" != value?.ToLower() && "true" != value?.ToLower() && scriptid == null)
                                {
                                    isbreak = true;
                                    break;
                                }


                            }
                            else if ("scriptname" == y)
                            {
                                if (string.IsNullOrWhiteSpace(value))
                                {
                                    isbreak = true;
                                    break;
                                }
                                else
                                {
                                    string[] definedscripts = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLower()).ToArray();
                                    var definedscript = definedscripts.FirstOrDefault(t => bindingName.Trim().ToLower() == t);
                                    if (definedscript == null)
                                    {
                                        isbreak = true;
                                        break;
                                    }
                                }
                            }
                            else if (scriptid != null && "script_id" == y)
                            {
                                if (!string.Equals(scriptid, value, StringComparison.OrdinalIgnoreCase))
                                {
                                    isbreak = true;
                                    break;
                                }
                                else
                                {
                                    dr.Dict.Add(y, value);
                                }
                            }
                            else
                            {
                                dr.Dict.Add(y, value);
                            }

                        }

                        if (isbreak)
                        {
                            continue;
                        }

                        dt.list.Add(dr);

                    }
                }
                else
                {
                    throw new Exception.ParseExcelException("No data found.");
                }

                return dt;
            }
        }


        internal void Update(ProviderModel model)
        {
            lock (obj)
            {
                var enabledlist = model.list.Where(t => t.IsEnabled).ToList();
                if (enabledlist.Count > 0)
                {



                    foreach (var item in enabledlist)
                    {
                        foreach (var key in item.Dict.isWriten)
                        {
                            ICell cell = sheet.GetRow(item.RowNumber).GetCell(model.ColumnMapping[key]);
                            if (cell == null)
                            {
                                cell = sheet.GetRow(item.RowNumber).CreateCell(model.ColumnMapping[key]);
                            }
                            cell.SetCellValue(item.Dict[key]);
                        }


                    }
                    string datapath = System.IO.Path.GetDirectoryName(dataFile);
                    if (string.IsNullOrEmpty(datapath))
                    {
                        datapath = ".";
                    }
                    string dataname = System.IO.Path.GetFileName(dataFile);
                    string datanew = $"{datapath}\\~{dataname}";
                    using (FileStream fs = System.IO.File.Create(datanew))
                    {
                        wb.Write(fs);

                    }
                    if (File.Exists(dataFile))
                    {
                        File.Delete(dataFile);
                        File.Move(datanew, dataFile);
                    }




                }
            }
        }
    }

    internal class DataModel
    {
        internal bool IsEnabled
        {
            get; set;
        } = true;

        internal int RowNumber
        {
            get; set;
        }
        internal AFWDictionary<string> Dict
        {
            get; set;
        } = new AFWDictionary<string>(StringComparer.OrdinalIgnoreCase);
    }

    internal class ProviderModel
    {

        internal Dictionary<string, int> ColumnMapping
        {
            get; set;
        } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        internal List<DataModel> list
        {
            get; set;
        } = new List<DataModel>();
    }





}