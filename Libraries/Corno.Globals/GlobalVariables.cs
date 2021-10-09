using Microsoft.Practices.Unity;

namespace Corno.Globals
{
    /// <summary>
    ///     Contains global variables for project.
    /// </summary>
    public static class GlobalVariables
    {
        public static int CompanyId = 1;

        public static int FinancialYearId = 1;

        /// <summary>
        ///     Financial Year Id
        /// </summary>
        public static int DepatmentId = 0;

        /// <summary>
        ///     user Id;
        /// </summary>
        public static string UserId = null;

        /// <summary>
        ///     Global variable that is constant.
        /// </summary>
        public static string UserName = string.Empty;

        /// <summary>
        ///     Employee
        /// </summary>
        public static int EmployeeId = 0;

        public static string ConnectionString = string.Empty;
        public static string ConnectionStringExamServer = string.Empty;

        /// <summary>
        ///     Global variable that is constant.
        /// </summary>
        public static IUnityContainer Container = null;
    }
}