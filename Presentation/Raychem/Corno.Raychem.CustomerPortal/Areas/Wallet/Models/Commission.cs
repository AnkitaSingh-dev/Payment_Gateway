 using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Base;
using Corno.Data.Common;

namespace Corno.Raychem.CustomerPortal.Areas.Wallet.Models
{
    [Table("Commission")]
    public class Commission : BaseModel
    {
        #region -- Constructors --
        //public Commission()
        //{
        //    MiscMasters = new List<MiscMaster>();
        //}
        #endregion

        [DisplayName("Service")]
        public int? ServiceId { get; set; }
        [DisplayName("User Type")]
        public int? UserTypeId { get; set; }
        [DisplayName("Operator")]
        public int? OperatorId { get; set; }
        [DisplayName("Incomming Commission")]
        public double? IncomingCommission { get; set; }
        [DisplayName("Outgoing Commission")]
        public double? OutgoingCommission { get; set; }
        [DisplayName("Convinience Fee")]
        public double? ConvinienceFee { get; set; }
        [DisplayName("TDS")]
        public double? Tds { get; set; }
        [DisplayName("GST")]
        public double? Gst { get; set; }

        public double? TransactionAmount { get; set; }
        public double? GrossComission { get; set; }
        public double? BasicComission { get; set; }
        public double? GstAmount { get; set; }
        public double? TdsAmount { get; set; }
        public double? PayableAmount { get; set; }

        //[NotMapped]
        //public IEnumerable<MiscMaster> MiscMasters { get; set; }
        //[NotMapped]
        //public string UserTypeName { get; set; }
        //[NotMapped]
        //public string ServiceName { get; set; }
        //[NotMapped]
        //public string OperatorName { get; set; }
    }
}