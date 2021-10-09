using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Corno.Services.Telerik.Interfaces;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Report = Telerik.Reporting.Report;
using ReportItemBase = Telerik.Reporting.ReportItemBase;
using TextBox = Telerik.Reporting.TextBox;

namespace Corno.Services.Telerik
{
    public class TelerikReportService : ITelerikReportService
    {
        #region -- Methods --
        public void MakefieldsInvisible(ReportItemBase.ItemCollection itemCollection, DataTable dataTable)
        {
            // Remove all unnecessary items
            foreach (var reportItem in itemCollection)
            {
                if (reportItem.GetType() == typeof(TextBox))
                {
                    if (reportItem.Name.Substring(0, 3) == "txt" || reportItem.Name.Substring(0, 3) == "lbl")
                    {
                        var fieldName = reportItem.Name.Remove(0, 3);
                        var contains = dataTable.Columns.Contains(fieldName);
                        if (false == contains)
                        {
                            reportItem.Visible = false;
                            ((TextBox) reportItem).Value = string.Empty;
                        }
                    }
                }

                if (reportItem.Items.Count > 0)
                    MakefieldsInvisible(reportItem.Items, dataTable);
            }
        }

        public ArrayList GetSelectedValues(string columnName, DataTable dataTable)
        {
            var selectedValues = new ArrayList();
            foreach (DataRow filterRow in dataTable.Rows)
            {
                if (string.Empty == filterRow[columnName].ToString() ||
                    "" == filterRow[columnName].ToString())
                    continue;

                if (false == selectedValues.Contains(filterRow[columnName]))
                {
                    selectedValues.Add(filterRow[columnName]);
                }
            }

            return selectedValues;
        }

        public Report GetTelerikReport(string reportName)
        {
            Type type;
            //if (type != null)
            //    return (Report) assembly.CreateInstance(type.FullName);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            foreach (var assembly in assemblies)
            {
                type = assembly.GetType(reportName);
                if (type != null)
                    return (Report) assembly.CreateInstance(type.FullName);
            }

            // Get Assembly name from its type name
            var nameStrings = reportName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var assemblyName = nameStrings[0];
            for (var index = 1; index < nameStrings.Length - 1; index++)
            {
                assemblyName += "." + nameStrings[index];
                try
                {
                    var assembly = Assembly.Load(assemblyName);
                    if (null == assembly)
                        continue;
                    type = assembly.GetType(reportName);
                    if (type != null)
                        return (Report) assembly.CreateInstance(type.FullName);
                }
                catch
                {
                    //Ignore
                }
            }

            return null;
        }

        public void SaveReport(string format, Report report, string fileName)
        {
            var reportProcessor = new ReportProcessor();
            var instanceReportSource = new InstanceReportSource { ReportDocument = report };
            var result = reportProcessor.RenderReport(format, instanceReportSource, null);

            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
                fs.Close();
            }
        }
        #endregion
    }
}