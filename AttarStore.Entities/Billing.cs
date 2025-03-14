using System.ComponentModel.DataAnnotations;

namespace AttarStore.Entities
{
    public class Billing
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime BillDate { get; set; }

        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [StringLength(20)]
        public string PaymentStatus { get; set; }
    }

}
