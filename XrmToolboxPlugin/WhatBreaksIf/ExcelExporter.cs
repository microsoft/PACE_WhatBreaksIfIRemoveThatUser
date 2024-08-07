using OfficeOpenXml;
using OfficeOpenXml.Table;
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
            var environmentsWithFlows = targetEnvironments.Where(env => env.Key.flows.Any() || env.Key.connectionReferences.Any()).Select(x => x.Key).ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (var package = new ExcelPackage(fs))
                {
                    foreach (var environment in environmentsWithFlows)
                    {
                        string flowManagementEnvironment = "https://make.powerautomate.com/";

                        /*switch (environment.location)
                        {
                            case "europe":
                                flowManagementEnvironment = "https://emea.flow.microsoft.com/manage/";
                                break;
                            default:
                                flowManagementEnvironment = "https://flow.microsoft.com/manage/";
                                break;
                        }*/
                        var sheet = package.Workbook.Worksheets.Add(environment.properties.displayName);

                        // headers
                        sheet.Cells[1,1].Value = "Type";
                        sheet.Cells[1, 2].Value = "Id";
                        sheet.Cells[1, 3].Value = "Name";
                        sheet.Cells[1, 4].Value = "DisplayName";
                        sheet.Cells[1, 5].Value = "API";
                        sheet.Cells[1, 6].Value = "Status";
                        sheet.Cells[1, 7].Value = "CreatedOn";
                        sheet.Cells[1, 8].Value = "CreatedBy";
                        sheet.Cells[1, 9].Value = "ModifiedOn";
                        sheet.Cells[1, 10].Value = "Url";

                        // Fill the table range with Flow data
                        for (int rowIndex = 2; rowIndex < environment.flows.Count + 1; rowIndex++)
                        {
                            var currentFlow = environment.flows[rowIndex - 1];
                            sheet.Cells[rowIndex, 1].Value = "Flow";
                            sheet.Cells[rowIndex, 2].Value = currentFlow.id;
                            sheet.Cells[rowIndex, 3].Value = currentFlow.name;
                            sheet.Cells[rowIndex, 4].Value = currentFlow.properties.displayName;
                            sheet.Cells[rowIndex, 5].Value = currentFlow.properties.apiId.Replace("/providers/Microsoft.PowerApps/apis/", "");
                            sheet.Cells[rowIndex, 6].Value = currentFlow.properties.state;
                            sheet.Cells[rowIndex, 7].Value = $"{currentFlow.properties.createdTime.ToShortDateString()} {currentFlow.properties.createdTime.ToLongTimeString()}";
                            sheet.Cells[rowIndex, 8].Value = currentFlow.properties.creator.objectId;
                            sheet.Cells[rowIndex, 9].Value = $"{currentFlow.properties.lastModifiedTime.ToShortDateString()} {currentFlow.properties.lastModifiedTime.ToLongTimeString()}";
                            sheet.Cells[rowIndex, 10].Value = currentFlow.id.Replace("/providers/Microsoft.ProcessSimple/", flowManagementEnvironment);
                        }

                        // Fill the table range with Connection References data
                        for (int rowIndex = 1; rowIndex <= environment.connectionReferences.Count; rowIndex++)
                        {
                            var currentConnectionReference = environment.connectionReferences[rowIndex - 1];
                            sheet.Cells[rowIndex + environment.flows.Count, 1].Value = "Connection Reference";
                            sheet.Cells[rowIndex + environment.flows.Count, 2].Value = currentConnectionReference.id;
                            sheet.Cells[rowIndex + environment.flows.Count, 3].Value = currentConnectionReference.name;
                            sheet.Cells[rowIndex + environment.flows.Count, 4].Value = currentConnectionReference.properties.displayName;
                            sheet.Cells[rowIndex + environment.flows.Count, 5].Value = currentConnectionReference.properties.apiId.Substring(currentConnectionReference.properties.apiId.LastIndexOf('/') + 1);
                            sheet.Cells[rowIndex + environment.flows.Count, 6].Value = string.Join(",", currentConnectionReference.properties.statuses.Select(s => s.status));
                            sheet.Cells[rowIndex + environment.flows.Count, 7].Value = $"{currentConnectionReference.properties.createdTime.ToShortDateString()} {currentConnectionReference.properties.createdTime.ToLongTimeString()}";
                            sheet.Cells[rowIndex + environment.flows.Count, 8].Value = currentConnectionReference.properties.createdBy.email;
                            sheet.Cells[rowIndex + environment.flows.Count, 9].Value = $"{currentConnectionReference.properties.lastModifiedTime.ToShortDateString()} {currentConnectionReference.properties.lastModifiedTime.ToLongTimeString()}";
                            sheet.Cells[rowIndex + environment.flows.Count, 10].Value = $"https://make.powerapps.com/environments/{environment.name}/connections/{currentConnectionReference.properties.apiId.Substring(currentConnectionReference.properties.apiId.LastIndexOf('/') + 1)}/{currentConnectionReference.name}/ details";
                        }

                        //sheet.Cells[1, 7, environment.flows.Count + environment.connectionReferences.Count, 7].Style.Numberformat.Format = "dd/mm/yyyy hh:mm:ss";
                        //sheet.Cells[1, 7, environment.flows.Count + environment.connectionReferences.Count, 4].Style.Numberformat.Format = "dd/mm/yyyy hh:mm:ss";

                        // Table range including header row
                        var range = sheet.Cells[1, 1, environment.flows.Count + environment.connectionReferences.Count, 10];

                        // create the table
                        var table = sheet.Tables.Add(range, "DataTable");

                        // configure the table
                        table.ShowHeader = true;
                        table.ShowFirstColumn = true;
                        table.TableStyle = TableStyles.Medium2;
                        range.AutoFitColumns();

                    }

                    package.Save();
                }
            }
        }

    }

}