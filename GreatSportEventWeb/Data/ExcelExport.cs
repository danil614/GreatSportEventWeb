using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using OfficeOpenXml;

namespace GreatSportEventWeb.Data;

public static class ExcelExport
{
    public static string ExcelContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private static DataTable ListToDataTable<T>(List<T> data, string[] columnsToTake)
    {
        var properties = TypeDescriptor.GetProperties(typeof(T));
        var dataTable = new DataTable();

        foreach (var columnName in columnsToTake)
        {
            var property = properties.Find(columnName, true);

            if (property != null)
            {
                var displayName = GetDisplayName(property);
                dataTable.Columns.Add(displayName,
                    Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }
        }

        var values = new object?[columnsToTake.Length];
        foreach (var item in data)
        {
            for (var i = 0; i < columnsToTake.Length; i++)
            {
                var property = properties.Find(columnsToTake[i], true);
                values[i] = property?.GetValue(item);
            }

            dataTable.Rows.Add(values);
        }

        return dataTable;
    }

    private static string GetDisplayName(MemberDescriptor property)
    {
        var displayAttribute = property.Attributes.OfType<DisplayAttribute>().FirstOrDefault();
        return displayAttribute?.Name ?? property.Name;
    }

    private static byte[]? ExportExcel(DataTable dataTable, bool showIndex)
    {
        using var package = new ExcelPackage();
        var workSheet = package.Workbook.Worksheets.Add("Sheet 1");

        if (showIndex)
        {
            var dataColumn = dataTable.Columns.Add("#", typeof(int));
            dataColumn.SetOrdinal(0);
            var index = 1;
            foreach (DataRow item in dataTable.Rows)
            {
                item[0] = index;
                index++;
            }
        }

        // add the content into the Excel file 
        workSheet.Cells.LoadFromDataTable(dataTable, true);

        // autofit width of cells with small content 
        var columnIndex = 1;
        foreach (DataColumn unused in dataTable.Columns)
        {
            var columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex,
                workSheet.Dimension.End.Row, columnIndex];
            var maxLength = columnCells.DefaultIfEmpty().Max(cell =>
                cell?.Value == null ? 0 : (cell.Value.ToString() ?? string.Empty).Count());
            if (maxLength < 150) workSheet.Column(columnIndex).AutoFit();

            columnIndex++;
        }
        
        // format header - bold
        using (var excelRange = workSheet.Cells[1, 1, 1, dataTable.Columns.Count]) 
        {
            excelRange.Style.Font.Bold = true;
        } 

        return package.GetAsByteArray();
    }

    public static byte[]? ExportExcel<T>(List<T> data, string[] columnsToTake, bool showIndex = false)
    {
        var dataTable = ListToDataTable(data, columnsToTake);
        return ExportExcel(dataTable, showIndex);
    }
}