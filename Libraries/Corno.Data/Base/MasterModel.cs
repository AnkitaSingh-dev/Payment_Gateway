using System.ComponentModel.DataAnnotations;

namespace Corno.Data.Base
{
    public class MasterModel : BaseModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class MasterViewModel
    {
        public string Code { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}