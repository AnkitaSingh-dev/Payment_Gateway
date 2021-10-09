
namespace Corno.Services.Http.Interfaces
{
    public interface IHttpService
    {
        #region -- Methods --
        string Get(string uri);
        object JsonPost(string requestData, string url);

        #endregion
    }
}