using ClosedXML.Excel;

namespace dominikz.Infrastructure.Excel;

public abstract class ExcelEditor : IDisposable
{
    private readonly string _filepath;
    private readonly XLWorkbook _workBook;
    private readonly IXLWorksheet _workSheet;

    protected ExcelEditor(string filepath, string sheetName)
    {
        _filepath = filepath;
        _workBook = new XLWorkbook(filepath);
        var sheet = _workBook.Worksheets.FirstOrDefault(x => x.Name.Equals(sheetName, StringComparison.OrdinalIgnoreCase));
        sheet ??= _workBook.Worksheets.FirstOrDefault();
        if (sheet == null)
        {
            _workBook.Worksheets.Add(sheetName);
            sheet = _workBook.Worksheets.First();
        }

        _workSheet = sheet;
    }

    protected void UpdateCell(int rowIdx, int columnIdx, string value, Action<IXLCell> formatter)
    {
        var cell = GetCellRef(rowIdx, columnIdx);
        cell.Value = value;
        formatter(cell);
    }

    protected void UpdateCell(int rowIdx, int columnIdx, string value)
        => UpdateCell(rowIdx, columnIdx, value, _ => { });

    protected IXLCell GetCellRef(int rowIdx, int columnIdx)
        => _workSheet.Cell(rowIdx, columnIdx);

    protected IXLCells GetCellRef(string range)
        => _workSheet.Cells(range);

    protected IXLCell Merge(string range)
        => _workSheet.Range(range).Merge().FirstCell();

    protected void Save()
        => _workBook.SaveAs(_filepath);

    public void Dispose()
    {
        _workBook.Dispose();
    }
}