using System.Collections.Generic;

namespace Corno.Raychem.CustomerPortal.Areas.Admin.Models
{
    public sealed class AspNetRole
    {
        public AspNetRole()
        {
            AspNetUsers = new List<AspNetUser>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<AspNetUser> AspNetUsers { get; set; }
    }


}
