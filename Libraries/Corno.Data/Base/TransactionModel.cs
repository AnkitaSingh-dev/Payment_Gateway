namespace Corno.Data.Base
{
    public class TransactionModel : BaseModel, ITransactionModel
    {
        public TransactionModel()
        {
            FinancialYearId = 1;
        }

        public int? FinancialYearId { get; set; }
    }
}