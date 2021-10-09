using System.ComponentModel;
using Corno.Data.Base;

namespace Corno.Data.Common
{
    public class State : MasterModel
    {
        [DisplayName("Zone")]
        public int? ZoneId { get; set; }

        [DisplayName("Country")]
        public int? CountryId { get; set; }
    }
}
