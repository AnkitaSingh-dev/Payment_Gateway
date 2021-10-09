using Corno.Globals;
using Corno.Globals.Constants;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LINQtoCSV;

namespace Corno.Data.Base
{
    public class BaseModel : CornoModel, IBaseModel
    {
        #region -- Constructors --

        public BaseModel()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Reset();
        }

        #endregion

        #region -- Data Members --

        [DisplayName("Company")]
        public int? CompanyId { get; set; }
        [DisplayName("Serial No")]
        [DefaultValue(0)]
        public int? SerialNo { get; set; }

        public string Code { get; set; }
        [Key]
        public int Id { get; set; }
        public string Status { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Created Date")]
        public DateTime? CreatedDate { get; set; }
        [DisplayName("Modified By")]
        public string ModifiedBy { get; set; }
        [DisplayName("Modified Date")]
        public DateTime? ModifiedDate { get; set; }
        [DisplayName("Delete By")]
        public string DeletedBy { get; set; }
        [DisplayName("Deleted Date")]
        public DateTime? DeletedDate { get; set; }
        #endregion

        #region -- Methods --

        public override void Reset()
        {
            CompanyId = GlobalVariables.CompanyId;
            SerialNo = 0;

            Id = 0;
            Code = string.Empty;
            //Status = StatusConstants.Active;

            CreatedBy = GlobalVariables.UserId;
            CreatedDate = DateTime.Now;
            ModifiedBy = GlobalVariables.UserId;
            ModifiedDate = DateTime.Now;
        }

        #endregion
    }
}