using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using FlowOwnershipAudit.Model;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using static FlowOwnershipAudit.FlowOwnershipAuditControl;
using Connection = FlowOwnershipAudit.Model.Connection;

namespace FlowOwnershipAudit
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

        internal void ExportToExcel(string filename, bool allData, bool multiSheet)
        {
            // get all environments that have components﻿
            var environmentsWithFlows = targetEnvironments.Where(env => env.Key.flows.Any() || env.Key.connections.Any()).Select(x => x.Key).ToList();

            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (var workbook = new XLWorkbook())
                {
                    List<Flow> flows = new List<Flow>();
                    List<Connection> connections = new List<Connection>();
                    List<ConnectionReference> connectionReferences = new List<ConnectionReference>();

                    string worksheetName = string.Empty;
                    IXLWorksheet sheet = null;

                    if (!multiSheet)
                    {
                        worksheetName = "Data";
                        sheet = workbook.AddWorksheet(worksheetName);
                    }

                    foreach (var environment in environmentsWithFlows)
                    {
                        // Fill the table range with Flow data
                        if (multiSheet)
                        {
                            flows = new List<Flow>();
                            connections = new List<Connection>();
                            // take only the first 31 characters of the environment name for the worksheet name
                            worksheetName = (environment.properties.displayName.Length > 30) ? environment.properties.displayName.Substring(0, 31) : environment.properties.displayName;
                            sheet = workbook.AddWorksheet(worksheetName);
                        }

                        // Fill the table range with Flow data
                        // Fill the table range with Connection data
                        // Till the table range with Connection Reference data
                        if (allData)
                        {
                            flows.AddRange(environment.flows.ToList());
                            connections.AddRange(environment.connections.ToList());
                            connectionReferences.AddRange(environment.flows.Where(x => x.properties != null && x.properties.connectionReferences != null).SelectMany(flow => flow.properties.connectionReferences).Distinct().ToList());
                        }
                        else
                        {
                            flows.AddRange(environment.flows.Where(x => x.isOwnedByX).ToList());
                            connections.AddRange(environment.connections.Where(x => x.isOwnedByX).ToList());
                        }
                        
                        
                        if (allData)
                        {
                            connections.AddRange(environment.connections.ToList());
                        }
                        else
                        {
                            connections.AddRange(environment.connections.Where(x => x.isOwnedByX).ToList());
                        }

                        if (multiSheet)
                        {
                            populateWorkSheet(sheet, flows, connections);
                        }

                        /*currentTargetEnvironment.Key.flows
                            .Where(x=>x.properties != null && x.properties.connectionReferences != null)
                            .SelectMany(flow => flow.properties.connectionReferences)
                            .Select(connectionReference => connectionReference.id)
                            .Distinct()
                            .Count()
                            .ToString()*/
                    }

                    if (!multiSheet)
                    {
                        populateWorkSheet(sheet, flows, connections);
                    }

                    workbook.SaveAs(fs);
                }
            }
        }

        internal void populateWorkSheet(IXLWorksheet sheet, List<Flow> flows, List<Connection> connections)
        {
            string flowManagementEnvironment = "https://make.powerautomate.com/";

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

            for (int rowIndex = 2; rowIndex < flows.Count + 1; rowIndex++)
            {
                var currentFlow = flows[rowIndex - 1];
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


            for (int rowIndex = 1; rowIndex <= connections.Count; rowIndex++)
            {
                var currentConnection = connections[rowIndex - 1];
                sheet.Cell(rowIndex + flows.Count, 1).SetValue("Connection");
                sheet.Cell(rowIndex + flows.Count, 2).SetValue(currentConnection.id);
                sheet.Cell(rowIndex + flows.Count, 3).SetValue(currentConnection.name);
                sheet.Cell(rowIndex + flows.Count, 4).SetValue(currentConnection.properties.displayName);
                sheet.Cell(rowIndex + flows.Count, 5).SetValue(currentConnection.properties.apiId.Substring(currentConnection.properties.apiId.LastIndexOf('/') + 1));
                sheet.Cell(rowIndex + flows.Count, 6).SetValue(string.Join(",", currentConnection.properties.statuses.Select(s => s.status)));
                sheet.Cell(rowIndex + flows.Count, 7).SetValue($"{currentConnection.properties.createdTime.ToShortDateString()} {currentConnection.properties.createdTime.ToLongTimeString()}");
                sheet.Cell(rowIndex + flows.Count, 8).SetValue(currentConnection.properties.createdBy.email);
                sheet.Cell(rowIndex + flows.Count, 9).SetValue($"{currentConnection.properties.lastModifiedTime.ToShortDateString()} {currentConnection.properties.lastModifiedTime.ToLongTimeString()}");
                //TODO
                sheet.Cell(rowIndex + flows.Count, 10).SetValue($"https://make.powerapps.com/environments/{currentConnection.properties.environment.name}/connections/{currentConnection.properties.apiId.Substring(currentConnection.properties.apiId.LastIndexOf('/') + 1)}/{currentConnection.name}/details");
            }

            // create the table
            var range = sheet.Range(1, 1, flows.Count + connections.Count, 10);
            var table = range.CreateTable("DataTable");

            table.Theme = XLTableTheme.TableStyleMedium2;
            table.SetShowHeaderRow(true);
            table.SetShowRowStripes(true);
            table.SetShowAutoFilter(true);
            sheet.Columns().AdjustToContents();
        }

    }

}