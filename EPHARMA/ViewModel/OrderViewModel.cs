using EPHARMA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPHARMA.ViewModel
{
    public class OrderViewModel
    {
      public  Order Order { get; set; } = new Order();
        public OrderPrescription Prescription { get; set; } = new OrderPrescription();
        public List<OrderMedicine> OrderMedicine { get; set; } = new List<OrderMedicine>();
    }
}
