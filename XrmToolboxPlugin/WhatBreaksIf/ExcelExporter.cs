using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static WhatBreaksIf.WhatBreaksIfControl;

namespace WhatBreaksIf﻿
{
    internal class ExcelExporter : IDisposable
    {
        private readonly EnvironmentCollection targetEnvironments;

        public ExcelExporter(EnvironmentCollection targetEnvironments)
        {
            this.targetEnvironments = targetEnvironments;
        }

        public void Dispose()
        {
            // todo - check if we need to clean up anything
        }

        internal void ExportToExcel(string filename)
        {
            // get all environments that have either flow or connection references﻿
            var environmentsWithFlows = targetEnvironments.Where(env => env.Key.flows.Any()).Select(x => x.Key).ToList();

            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();

                // create a sheet for each environment﻿
                foreach (var environment in environmentsWithFlows)
                {
                    // create a sheet for that environment﻿
                    ISheet excelSheet = workbook.CreateSheet(environment.properties.displayName);


                    List<string> columns = new List<string>() { "Type", "Id", "Name", "URL" };

                    // create the header row﻿
                    IRow row = excelSheet.CreateRow(0);

                    for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++)
                    {
                        var columnName = columns[columnIndex];
                        columns.Add(columnName);
                        row.CreateCell(columnIndex).SetCellValue(columnName);
                    }

                    // create the rows﻿

                    for (int rowIndex = 1; rowIndex < environment.flows.Count; rowIndex++)
                    {
                        row = excelSheet.CreateRow(rowIndex);

                        var currentFlow = environment.flows[rowIndex - 1];

                        // type﻿
                        row.CreateCell(0).SetCellValue("Flow");

                        // Id﻿
                        row.CreateCell(0).SetCellValue(currentFlow.id);

                        // Name﻿
                        row.CreateCell(0).SetCellValue(currentFlow.name);

                        // URL﻿
                        row.CreateCell(0).SetCellValue("not implemented yet :(");
                    }
                }

                workbook.Write(fs);
            }
        }

    }

}