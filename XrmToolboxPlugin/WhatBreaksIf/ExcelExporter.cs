using ClosedXML.Excel;
using System;
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

            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (var workbook = new XLWorkbook())
                {
                    foreach (var environment in environmentsWithFlows)
                    {
                        string flowManagementEnvironment = "https://make.powerautomate.com/";

                        var sheet = workbook.AddWorksheet(environment.properties.displayName);

                        sheet.Cell(1, 1).SetValue("Type");
                        sheet.Cell(1, 2).SetValue("Id");
                        sheet.Cell(1, 3).SetValue("Name");
                        sheet.Cell(1, 4).SetValue("DisplayName");
                        sheet.Cell(1, 5).SetValue("API");
                        sheet.Cell(1, 6).SetValue("Status");
                        sheet.Cell(1, 7).SetValue("CreatedOn");
                        sheet.Cell(1, 8).SetValue("CreatedBy");
                        sheet.Cell(1, 9).SetValue("ModifiedOn");
                        sheet.Cell(1, 10).SetValue("Url");

                        // Fill the table range with Flow data
                        for (int rowIndex = 2; rowIndex < environment.flows.Count + 1; rowIndex++)
                        {
                            var currentFlow = environment.flows[rowIndex - 1];
                            sheet.Cell(rowIndex, 1).SetValue("Flow");
                            sheet.Cell(rowIndex, 2).SetValue(currentFlow.id);
                            sheet.Cell(rowIndex, 3).SetValue(currentFlow.name);
                            sheet.Cell(rowIndex, 4).SetValue(currentFlow.properties.displayName);
                            sheet.Cell(rowIndex, 5).SetValue(currentFlow.properties.apiId.Replace("/providers/Microsoft.PowerApps/apis/", ""));
                            sheet.Cell(rowIndex, 6).SetValue(currentFlow.properties.state);
                            sheet.Cell(rowIndex, 7).SetValue($"{currentFlow.properties.createdTime.ToShortDateString()} {currentFlow.properties.createdTime.ToLongTimeString()}");
                            sheet.Cell(rowIndex, 8).SetValue(currentFlow.properties.creator.objectId);
                            sheet.Cell(rowIndex, 9).SetValue($"{currentFlow.properties.lastModifiedTime.ToShortDateString()} {currentFlow.properties.lastModifiedTime.ToLongTimeString()}");
                            sheet.Cell(rowIndex, 10).SetValue(currentFlow.id.Replace("/providers/Microsoft.ProcessSimple/", flowManagementEnvironment));
                        }

                        // Fill the table range with Connection References data
                        for (int rowIndex = 1; rowIndex <= environment.connectionReferences.Count; rowIndex++)
                        {
                            var currentConnectionReference = environment.connectionReferences[rowIndex - 1];
                            sheet.Cell(rowIndex + environment.flows.Count, 1).SetValue("Connection Reference");
                            sheet.Cell(rowIndex + environment.flows.Count, 2).SetValue(currentConnectionReference.id);
                            sheet.Cell(rowIndex + environment.flows.Count, 3).SetValue(currentConnectionReference.name);
                            sheet.Cell(rowIndex + environment.flows.Count, 4).SetValue(currentConnectionReference.properties.displayName);
                            sheet.Cell(rowIndex + environment.flows.Count, 5).SetValue(currentConnectionReference.properties.apiId.Substring(currentConnectionReference.properties.apiId.LastIndexOf('/') + 1));
                            sheet.Cell(rowIndex + environment.flows.Count, 6).SetValue(string.Join(",", currentConnectionReference.properties.statuses.Select(s => s.status)));
                            sheet.Cell(rowIndex + environment.flows.Count, 7).SetValue($"{currentConnectionReference.properties.createdTime.ToShortDateString()} {currentConnectionReference.properties.createdTime.ToLongTimeString()}");
                            sheet.Cell(rowIndex + environment.flows.Count, 8).SetValue(currentConnectionReference.properties.createdBy.email);
                            sheet.Cell(rowIndex + environment.flows.Count, 9).SetValue($"{currentConnectionReference.properties.lastModifiedTime.ToShortDateString()} {currentConnectionReference.properties.lastModifiedTime.ToLongTimeString()}");
                            sheet.Cell(rowIndex + environment.flows.Count, 10).SetValue($"https://make.powerapps.com/environments/{environment.name}/connections/{currentConnectionReference.properties.apiId.Substring(currentConnectionReference.properties.apiId.LastIndexOf('/') + 1)}/{currentConnectionReference.name}/details");
                        }

                        // create the table
                        var range = sheet.Range(1, 1, environment.flows.Count + environment.connectionReferences.Count, 10);
                        var table = range.CreateTable("DataTable");

                        table.Theme = XLTableTheme.TableStyleMedium2;
                        table.SetShowHeaderRow(true);
                        table.SetShowRowStripes(true);
                        table.SetShowAutoFilter(true);
                        sheet.Columns().AdjustToContents();
                    }

                    workbook.SaveAs(fs);
                }
            }
        }

    }

}