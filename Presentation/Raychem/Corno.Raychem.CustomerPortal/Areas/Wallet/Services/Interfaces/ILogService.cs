namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Services.Interfaces
{
    public interface ILogService
    {
        #region -- Methods --

        void GenerateLog(string tboType, string url, string requestName, string request, string response, string username = "");

        #endregion
    }
}