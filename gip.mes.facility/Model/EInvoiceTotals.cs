namespace gip.mes.facility
{
    public class EInvoiceTotals
    {

        public decimal? LineTotalAmount { get; set; }
        public decimal? ChargeTotalAmount { get; set; }
        public decimal? AllowanceTotalAmount { get; set; }
        public decimal? TaxBasisAmount { get; set; }
        public decimal? TaxTotalAmount { get; set; }
        public decimal? GrandTotalAmount { get; set; }
        public decimal? TotalPrepaidAmount { get; set; }
        public decimal? DuePayableAmount { get; set; }
        public decimal? RoundingAmount { get; set; }
    }
}
