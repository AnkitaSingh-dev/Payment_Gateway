using Corno.Data.Base;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Corno.Data.Login
{
    //[NotMapped]
    public class ApplicationRole : IdentityRole, ICornoModel
    {
        #region -- Constructors --

        #endregion

        #region -- Properties --

        public string Description { get; set; }

        #endregion

        #region -- Methods --

        public void Reset()
        {
            Description = string.Empty;
        }

        #endregion
    }
}