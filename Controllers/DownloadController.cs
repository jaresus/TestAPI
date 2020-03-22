using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        [HttpGet]
        public async Task<FileStreamResult> Download()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(mem, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "zestawienie" };

                    sheets.Append(sheet);

                    Row headerRow = new Row();
                    List<String> columns = new List<string>();

                    foreach (var r in new List<string> { "aaa", "bbb", "ccc", "ddd", "zzz" })
                    {
                        columns.Add(r);

                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(r);
                        headerRow.AppendChild(cell);
                    }
                    sheetData.AppendChild(headerRow);

                    foreach (var t in new List<string> { "qq", "ww", "ee" })
                    {
                        Row newRow = new Row();
                        foreach (String col in columns)
                        {
                            Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(t);
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }

                    
                    mem.Seek(0, SeekOrigin.Begin);

                    return new FileStreamResult(document.WorkbookPart.GetStream(), "application/ms-excel");
                }
            }
        }
    }
}