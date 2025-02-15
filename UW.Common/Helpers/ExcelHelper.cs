using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UW.Common
{
    public class ExcelHelper
    {
        public static string ExportDataSetToExcel(DataSet ds, string saveFile, string fontName, ExcelType excelType)
        {
            try
            {
                using (ds)
                {
                    IWorkbook workbook = null;
                    workbook = ((excelType != ExcelType.Excel2007) ? ((IWorkbook)new HSSFWorkbook()) : ((IWorkbook)new XSSFWorkbook()));
                    ICellStyle headerCellStyle = workbook.CreateCellStyle();
                    ICellStyle contentCellStyle = workbook.CreateCellStyle();
                    IFont headerFont = workbook.CreateFont();
                    IFont contentFont = workbook.CreateFont();
                    headerFont.FontHeightInPoints = 12.0;
                    headerFont.FontName = fontName;
                    contentFont.FontHeightInPoints = 10.0;
                    contentFont.FontName = fontName;
                    headerCellStyle.SetFont(headerFont);
                    contentCellStyle.SetFont(contentFont);
                    int dataTableCount = 0;
                    if (ds == null || ds.Tables.Count == 0)
                    {
                        return "没有查出数据，无法进行导出EXCEL处理";
                    }
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt == null || dt.Rows.Count == 0)
                        {
                            continue;
                        }
                        dataTableCount++;
                        ISheet sheet = workbook.CreateSheet(dt.TableName);
                        IRow headerRow = sheet.CreateRow(0);
                        foreach (DataColumn column in dt.Columns)
                        {
                            ICell cell = headerRow.CreateCell(column.Ordinal);
                            cell.SetCellValue(column.ColumnName);
                            cell.CellStyle = headerCellStyle;
                        }
                        int rowIndex = 1;
                        foreach (DataRow row in dt.Rows)
                        {
                            IRow contentRow = sheet.CreateRow(rowIndex);
                            foreach (DataColumn column in dt.Columns)
                            {
                                ICell cell = contentRow.CreateCell(column.Ordinal);
                                if (column.DataType.ToString().ToUpper() == "System.DateTime".ToUpper())
                                {
                                    if (row[column] != null && row[column].ToString() != "")
                                    {
                                        cell.SetCellValue(Convert.ToDateTime(row[column].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                    }
                                    else
                                    {
                                        cell.SetCellValue("");
                                    }
                                }
                                else if (column.DataType.ToString().ToUpper() == "System.DBNull".ToUpper())
                                {
                                    cell.SetCellValue("");
                                }
                                else if (column.DataType.ToString().ToUpper() == "System.Int32".ToUpper() || column.DataType.ToString().ToUpper() == "System.Int16".ToUpper() || column.DataType.ToString().ToUpper() == "System.Double".ToUpper())
                                {
                                    if (row[column] != null && row[column].ToString() != "")
                                    {
                                        cell.SetCellType(CellType.Numeric);
                                        cell.SetCellValue(Convert.ToDouble(row[column]));
                                    }
                                    else
                                    {
                                        cell.SetCellValue("");
                                    }
                                }
                                else
                                {
                                    cell.SetCellValue(row[column].ToString());
                                }
                                cell.CellStyle = contentCellStyle;
                            }
                            rowIndex++;
                        }
                    }
                    if (dataTableCount == 0)
                    {
                        return "没有查出数据，无法进行导出EXCEL处理";
                    }
                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.Write(stream);
                        using (FileStream fs = new FileStream(saveFile, FileMode.Create, FileAccess.Write))
                        {
                            byte[] buf = stream.ToArray();
                            fs.Write(buf, 0, buf.Length);
                            fs.Flush();
                            buf = null;
                        }
                        workbook = null;
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return "[ExportDataSetToExcel]Error," + ex.Message;
            }
        }

        public static string ExportDataTaleToExcel(DataTable dt, string saveFile, string fontName = "Arial", ExcelType excelType = ExcelType.Excel2003)
        {
            try
            {
                FileInfo fi = new FileInfo(saveFile);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                using (dt)
                {
                    IWorkbook workbook = null;
                    workbook = ((excelType != ExcelType.Excel2007) ? ((IWorkbook)new HSSFWorkbook()) : ((IWorkbook)new XSSFWorkbook()));
                    ICellStyle headerCellStyle = workbook.CreateCellStyle();
                    ICellStyle contentCellStyle = workbook.CreateCellStyle();
                    IFont headerFont = workbook.CreateFont();
                    IFont contentFont = workbook.CreateFont();
                    headerFont.FontHeightInPoints = 12.0;
                    headerFont.FontName = fontName;
                    contentFont.FontHeightInPoints = 10.0;
                    contentFont.FontName = fontName;
                    headerCellStyle.SetFont(headerFont);
                    contentCellStyle.SetFont(contentFont);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        return "没有查出数据，无法进行导出EXCEL处理";
                    }
                    dt.TableName = "Event";
                    ISheet sheet = workbook.CreateSheet(dt.TableName);
                    IRow headerRow = sheet.CreateRow(0);
                    foreach (DataColumn column in dt.Columns)
                    {
                        ICell cell = headerRow.CreateCell(column.Ordinal);
                        cell.SetCellValue(column.ColumnName);
                        cell.CellStyle = headerCellStyle;
                    }
                    int rowIndex = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        IRow contentRow = sheet.CreateRow(rowIndex);
                        foreach (DataColumn column in dt.Columns)
                        {
                            ICell cell = contentRow.CreateCell(column.Ordinal);
                            if (column.DataType.ToString().ToUpper() == "System.DateTime".ToUpper())
                            {
                                if (row[column] != null && row[column].ToString() != "")
                                {
                                    cell.SetCellValue(Convert.ToDateTime(row[column].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                else
                                {
                                    cell.SetCellValue("");
                                }
                            }
                            else if (column.DataType.ToString().ToUpper() == "System.DBNull".ToUpper())
                            {
                                cell.SetCellValue("");
                            }
                            else if (column.DataType.ToString().ToUpper() == "System.Int32".ToUpper() || column.DataType.ToString().ToUpper() == "System.Int16".ToUpper() || column.DataType.ToString().ToUpper() == "System.Double".ToUpper())
                            {
                                if (row[column] != null && row[column].ToString() != "")
                                {
                                    cell.SetCellType(CellType.Numeric);
                                    cell.SetCellValue(Convert.ToDouble(row[column]));
                                }
                                else
                                {
                                    cell.SetCellValue("");
                                }
                            }
                            else
                            {
                                cell.SetCellValue(row[column].ToString());
                            }
                            cell.CellStyle = contentCellStyle;
                        }
                        rowIndex++;
                    }
                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.Write(stream);
                        using (FileStream fs = new FileStream(saveFile, FileMode.Create, FileAccess.Write))
                        {
                            byte[] buf = stream.ToArray();
                            fs.Write(buf, 0, buf.Length);
                            fs.Flush();
                            buf = null;
                        }
                        workbook = null;
                    }
                }
                return "导出成功";
            }
            catch (Exception ex)
            {
                return "[ExportDataTaleToExcel]Error," + ex.Message;
            }
        }

        public static string ExportDataTaleToExcel(DataTable dt, ref byte[] streamBytes, string fontName, ExcelType excelType)
        {
            IWorkbook workbook = null;
            try
            {
                using (dt)
                {
                    workbook = ((excelType != ExcelType.Excel2007) ? ((IWorkbook)new HSSFWorkbook()) : ((IWorkbook)new XSSFWorkbook()));
                    ICellStyle headerCellStyle = workbook.CreateCellStyle();
                    ICellStyle contentCellStyle = workbook.CreateCellStyle();
                    IFont headerFont = workbook.CreateFont();
                    IFont contentFont = workbook.CreateFont();
                    headerFont.FontHeightInPoints = 12.0;
                    headerFont.FontName = fontName;
                    contentFont.FontHeightInPoints = 10.0;
                    contentFont.FontName = fontName;
                    headerCellStyle.SetFont(headerFont);
                    contentCellStyle.SetFont(contentFont);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        return "没有查出数据，无法进行导出EXCEL处理";
                    }
                    ISheet sheet = workbook.CreateSheet(dt.TableName);
                    IRow headerRow = sheet.CreateRow(0);
                    foreach (DataColumn column in dt.Columns)
                    {
                        ICell cell = headerRow.CreateCell(column.Ordinal);
                        cell.SetCellValue(column.ColumnName);
                        cell.CellStyle = headerCellStyle;
                    }
                    int rowIndex = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        IRow contentRow = sheet.CreateRow(rowIndex);
                        foreach (DataColumn column in dt.Columns)
                        {
                            ICell cell = contentRow.CreateCell(column.Ordinal);
                            if (column.DataType.ToString().ToUpper() == "System.DateTime".ToUpper())
                            {
                                if (row[column] != null && row[column].ToString() != "")
                                {
                                    cell.SetCellValue(Convert.ToDateTime(row[column].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                else
                                {
                                    cell.SetCellValue("");
                                }
                            }
                            else if (column.DataType.ToString().ToUpper() == "System.DBNull".ToUpper())
                            {
                                cell.SetCellValue("");
                            }
                            else if (column.DataType.ToString().ToUpper() == "System.Int32".ToUpper() || column.DataType.ToString().ToUpper() == "System.Int16".ToUpper() || column.DataType.ToString().ToUpper() == "System.Double".ToUpper())
                            {
                                if (row[column] != null && row[column].ToString() != "")
                                {
                                    cell.SetCellType(CellType.Numeric);
                                    cell.SetCellValue(Convert.ToDouble(row[column]));
                                }
                                else
                                {
                                    cell.SetCellValue("");
                                }
                            }
                            else
                            {
                                cell.SetCellValue(row[column].ToString());
                            }
                            cell.CellStyle = contentCellStyle;
                        }
                        rowIndex++;
                    }
                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.Write(stream);
                        stream.Flush();
                        stream.Position = 0L;
                        streamBytes = stream.ToArray();
                    }
                }
                return "导出成功";
            }
            catch (Exception ex)
            {
                return "[ExportDataTaleToExcel]Error," + ex.Message;
            }
            finally
            {
                workbook = null;
            }
        }

        public static DataTable ExportExcelToDataTable(string fileName, string sheetName, bool isFirstRowColumn)
        {
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                IWorkbook workbook = null;
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    if (fileName.IndexOf(".xlsx") > 0)
                    {
                        workbook = new XSSFWorkbook((Stream)fs);
                    }
                    else
                    {
                        if (fileName.IndexOf(".xls") <= 0)
                        {
                            throw new Exception("传入的Excel文件后缀名异常!");
                        }
                        workbook = new HSSFWorkbook(fs);
                    }
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                    if (sheet != null)
                    {
                        IRow firstRow = sheet.GetRow(0);
                        int cellCount = firstRow.LastCellNum;
                        if (isFirstRowColumn)
                        {
                            int num = 0;
                            for (int i = firstRow.FirstCellNum; i < cellCount; i++)
                            {
                                ICell cell = firstRow.GetCell(i);
                                if (cell != null)
                                {
                                    string cellValue = cell.StringCellValue;
                                    if (cellValue != null && num <= 22)
                                    {
                                        DataColumn column = new DataColumn(cellValue);
                                        data.Columns.Add(column);
                                        num++;
                                    }
                                }
                            }
                            startRow = sheet.FirstRowNum + 1;
                        }
                        else
                        {
                            startRow = sheet.FirstRowNum;
                        }
                        int rowCount = sheet.LastRowNum;
                        for (int i = startRow; i <= rowCount; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            if (row == null)
                            {
                                continue;
                            }
                            DataRow dataRow = data.NewRow();
                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (row.GetCell(j) != null && j <= 22)
                                {
                                    dataRow[j] = row.GetCell(j).ToString();
                                }
                            }
                            data.Rows.Add(dataRow);
                        }
                    }
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ExportExcelToDataTable Exception," + ex.Message);
            }
        }

        public static string GetCellText(ICell cell)
        {
            string retText = string.Empty;
            string strValue = string.Empty;
            switch (cell.CellType)
            {
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return ErrorEval.GetText(cell.ErrorCellValue);
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue.ToString("yyyy-MM-dd hh:mm:ss");
                    }
                    return cell.NumericCellValue.ToString();
                case CellType.String:
                    strValue = cell.StringCellValue;
                    if (!string.IsNullOrEmpty(strValue))
                    {
                        return strValue.ToString();
                    }
                    return string.Empty;
                case CellType.Formula:
                    switch (cell.CachedFormulaResultType)
                    {
                        case CellType.Boolean:
                            return cell.BooleanCellValue.ToString();
                        case CellType.Error:
                            return ErrorEval.GetText(cell.ErrorCellValue);
                        case CellType.Numeric:
                            if (DateUtil.IsCellDateFormatted(cell))
                            {
                                return cell.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                            }
                            return cell.NumericCellValue.ToString();
                        case CellType.String:
                            strValue = cell.StringCellValue;
                            if (!string.IsNullOrEmpty(strValue))
                            {
                                return strValue.ToString();
                            }
                            return string.Empty;
                        default:
                            return string.Empty;
                    }
                default:
                    return string.Empty;
            }
        }

        public static DataTable ExportExcelToDataTable(string fileName, string sheetName, int firstRowNum)
        {
            ISheet sheet = null;
            DataTable dt = new DataTable();
            int startRow = 0;
            try
            {
                IWorkbook workbook = null;
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    if (fileName.IndexOf(".xlsx") > 0)
                    {
                        workbook = new XSSFWorkbook((Stream)fs);
                    }
                    else
                    {
                        if (fileName.IndexOf(".xls") <= 0)
                        {
                            throw new Exception("传入的Excel文件后缀名异常!");
                        }
                        workbook = new HSSFWorkbook(fs);
                    }
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                    if (sheet != null)
                    {
                        IRow firstRow = sheet.GetRow(firstRowNum);
                        int cellCount = firstRow.LastCellNum;
                        List<int> lstColumnIndex = new List<int>();
                        for (int i = firstRow.FirstCellNum; i < cellCount; i++)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell == null)
                            {
                                continue;
                            }
                            string cellValue = cell.StringCellValue;
                            if (cellValue != null)
                            {
                                cellValue = cellValue.Replace("\r", "");
                                cellValue = cellValue.Replace("\n", "");
                                cellValue = cellValue.Replace("\t", "");
                                cellValue = cellValue.Replace(" ", "");
                                DataColumn column = new DataColumn(cellValue);
                                if (dt.Columns.IndexOf(cellValue) == -1)
                                {
                                    dt.Columns.Add(column);
                                    lstColumnIndex.Add(i);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + firstRowNum + 1;
                        int rowCount = sheet.LastRowNum;
                        for (int i = startRow; i <= rowCount; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            if (row == null)
                            {
                                continue;
                            }
                            DataRow dataRow = dt.NewRow();
                            int colIndex = 0;
                            string strValue = string.Empty;
                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (lstColumnIndex.IndexOf(j) >= 0)
                                {
                                    if (row.GetCell(j) == null)
                                    {
                                        dataRow[colIndex] = null;
                                        colIndex++;
                                    }
                                    else
                                    {
                                        ICell cell = row.GetCell(j);
                                        dataRow[colIndex] = GetCellText(cell);
                                        colIndex++;
                                    }
                                }
                            }
                            dt.Rows.Add(dataRow);
                        }
                    }
                    return dt;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ExportExcelToDataTable Exception," + ex.Message);
            }
        }

        public static DataSet ExportExcelToDataSet(string fileName, List<ExcelSheet> lstExcelSheet)
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = null;
                ISheet sheet = null;
                IWorkbook workbook = null;
                IRow row = null;
                ICell cell = null;
                int startRow = 0;
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    if (fileName.IndexOf(".xlsx") > 0)
                    {
                        workbook = new XSSFWorkbook((Stream)fs);
                    }
                    else
                    {
                        if (fileName.IndexOf(".xls") <= 0)
                        {
                            throw new Exception("传入的Excel文件后缀名异常!");
                        }
                        workbook = new HSSFWorkbook(fs);
                    }
                    foreach (ExcelSheet ExcelSheet in lstExcelSheet)
                    {
                        dt = new DataTable(ExcelSheet.DataTableName);
                        startRow = 0;
                        sheet = workbook.GetSheet(ExcelSheet.SheetName);
                        if (sheet == null)
                        {
                            throw new Exception("传入Excel的Sheet名称不存在!");
                        }
                        IRow firstRow = sheet.GetRow(ExcelSheet.FirstRowNum);
                        int cellCount = firstRow.LastCellNum;
                        int rowCount = sheet.LastRowNum;
                        List<int> lstColumnIndex = new List<int>();
                        for (int i = firstRow.FirstCellNum; i < cellCount; i++)
                        {
                            cell = firstRow.GetCell(i);
                            if (cell == null)
                            {
                                continue;
                            }
                            string cellValue = cell.StringCellValue;
                            if (cellValue == null)
                            {
                                continue;
                            }
                            cellValue = cellValue.Replace("\r", "");
                            cellValue = cellValue.Replace("\n", "");
                            cellValue = cellValue.Replace("\t", "");
                            cellValue = cellValue.Replace(" ", "");
                            cellValue = cellValue.ToLower();
                            if (ExcelSheet.LstColumnName.Count <= 0)
                            {
                                DataColumn column = new DataColumn(cellValue);
                                if (dt.Columns.IndexOf(cellValue) == -1)
                                {
                                    dt.Columns.Add(column);
                                    lstColumnIndex.Add(i);
                                }
                            }
                            else if (ExcelSheet.LstColumnName.IndexOf(cellValue) >= 0)
                            {
                                DataColumn column = new DataColumn(cellValue);
                                if (dt.Columns.IndexOf(cellValue) == -1)
                                {
                                    dt.Columns.Add(column);
                                    lstColumnIndex.Add(i);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + ExcelSheet.FirstRowNum + 1;
                        for (int i = startRow; i <= rowCount; i++)
                        {
                            row = sheet.GetRow(i);
                            if (row == null)
                            {
                                continue;
                            }
                            DataRow dataRow = dt.NewRow();
                            int colIndex = 0;
                            string strValue = string.Empty;
                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (lstColumnIndex.IndexOf(j) >= 0)
                                {
                                    if (row.GetCell(j) == null)
                                    {
                                        dataRow[colIndex] = null;
                                        colIndex++;
                                    }
                                    else
                                    {
                                        cell = row.GetCell(j);
                                        dataRow[colIndex] = GetCellText(cell);
                                        colIndex++;
                                    }
                                }
                            }
                            dt.Rows.Add(dataRow);
                        }
                        ds.Tables.Add(dt);
                    }
                    return ds;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
