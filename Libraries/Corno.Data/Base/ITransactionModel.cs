namespace Corno.Data.Base
{
    public interface ITransactionModel : IBaseModel
    {
        int? FinancialYearId { get; set; }
    }
}