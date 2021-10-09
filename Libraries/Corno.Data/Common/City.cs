using System.ComponentModel;
using Corno.Data.Base;

namespace Corno.Data.Common
{
    public class City : MasterModel
    {
        [DisplayName("Zone")]
        public int? ZoneId { get; set; }
        [DisplayName("State Name")]
        public int? StateId { get; set; }
        [DisplayName("Country Name")]
        public int? CountryId { get; set; }
    }
}
