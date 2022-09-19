using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPHARMA.Models.ViewModels
{
    public class OrderViewModel
    {
      public  List<Order> OrderList { get; set; } = new List<Order>();
        public OrderPrescription Prescription { get; set; } = new OrderPrescription();
        public OrderMedicine OrderMedicine { get; set; } = new OrderMedicine();
    }
}
