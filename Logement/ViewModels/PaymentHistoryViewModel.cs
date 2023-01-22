namespace Logement.ViewModels
{
    public class PaymentHistoryViewModel
    {
        public long Id { get; set; }

        public string TenantEmail { get; set; }
        //String because the 
        public string NunberOfMonthPaid { get; set; }

        public decimal AmountPaid { get; set; }

        public DateTime PaidDate { get; set; }
    }
}
