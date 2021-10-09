using System.Collections;
using System.Data;
using Telerik.Reporting;

namespace Corno.Services.Telerik.Interfaces
{
    public interface ITelerikReportService
    {
        #region -- Methods --

        void MakefieldsInvisible(ReportItemBase.ItemCollection itemCollection, DataTable dataTable);
        ArrayList GetSelectedValues(string columnName, DataTable dataTable);
        Report GetTelerikReport(string reportName);
        void SaveReport(string format, Report report, string fileName);

        #endregion
    }
}