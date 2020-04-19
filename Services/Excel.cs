using System;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace OpenXMLExcel.SLExcelUtility
{
    public class SLExcelWriter
    {
        /// <summary>
        /// zamienia numer na literowe oznaczenie kolumny
        /// </summary>
        private string ColumnLetter(int intCol)
        {
            var intFirstLetter = (intCol / 676) + 64;
            var intSecondLetter = (intCol % 676 / 26) + 64;
            var intThirdLetter = (intCol % 26) + 65;

            var firstLetter = (intFirstLetter > 64)   ? (char)intFirstLetter : ' ';
            var secondLetter = (intSecondLetter > 64) ? (char)intSecondLetter : ' ';
            var thirdLetter = (char)intThirdLetter;

            return string.Concat(firstLetter, secondLetter,
                thirdLetter).Trim();
        }

        /// <summary>
        /// tworzy komórkę excelową
        /// </summary>
        /// <param name="header">nagłówek</param>
        /// <param name="index">index</param>
        /// <param name="text">zawartość komórki</param>
        /// <returns></returns>
        private Cell CreateTextCell(string header, UInt32 index, string text)
        {
            var cell = new Cell
            {
                DataType = CellValues.InlineString,
                CellReference = header + index
            };

            var istring = new InlineString();
            var t = new Text { Text = text };
            istring.AppendChild(t);
            cell.AppendChild(istring);
            return cell;
        }
        /// <summary>
        /// generuje strumień - pilk excel'a
        /// </summary>
        /// <param name="data">dane przygotowne do konwersji na plik excela</param>
        public byte[] GenerateExcel(SLExcelData data)
        {
            var stream = new MemoryStream();
            var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

            var workbookpart = document.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();
            var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();

            worksheetPart.Worksheet = new Worksheet(sheetData);

            var sheets = document.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            var sheet = new Sheet()
            {
                Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = data.SheetName??"Lista ocen"
            };
            sheets.AppendChild(sheet);

            // Add header
            UInt32 rowIndex = 0;
            var row = new Row { RowIndex = ++rowIndex };
            sheetData.AppendChild(row);
            var cellIndex = 0;

            foreach (var header in data.Headers)
            {
                row.AppendChild(
                    CreateTextCell(ColumnLetter(cellIndex++), rowIndex, header??string.Empty)
                );
            }
            if (data.Headers.Count > 0)
            {
                // Add the column configuration if available
                if (data.ColumnConfigurations != null)
                {
                    var columns = (Columns)data.ColumnConfigurations.Clone();
                    worksheetPart.Worksheet.InsertAfter(columns, worksheetPart.Worksheet.SheetFormatProperties);
                }
            }

            // Add sheet data
            foreach (var rowData in data.DataRows)
            {
                cellIndex = 0;
                row = new Row { RowIndex = ++rowIndex };
                sheetData.AppendChild(row);
                foreach (var callData in rowData)
                {
                    var cell = CreateTextCell(ColumnLetter(cellIndex++), rowIndex, callData??string.Empty);
                    row.AppendChild(cell);
                }
            }

            workbookpart.Workbook.Save();
            document.Close();

            return stream.ToArray();
        }
    }

    public class SLExcelStatus
    {
        public string Message { get; set; }
        public bool Success
        {
            get { return string.IsNullOrWhiteSpace(Message); }
        }
    }

    public class SLExcelData
    {
        public SLExcelStatus Status { get; set; }
        public Columns ColumnConfigurations { get; set; }
        public List<string> Headers { get; set; }
        public List<List<string>> DataRows { get; set; }
        public string SheetName { get; set; }

        public SLExcelData()
        {
            Status = new SLExcelStatus();
            Headers = new List<string>();
            DataRows = new List<List<string>>();
        }
    }
}