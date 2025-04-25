using System.ComponentModel.DataAnnotations;

namespace NguyenMinhTri_2122110517.Model
{
    public class OrderDetail
    {
        public int Id { get; set; } // bigint(20), auto_increment, not null
        public int Order_Id { get; set; } // int(10), not null
        public int Product_Id { get; set; } // int(10), not null
        public double Price { get; set; } // double, not null
        public int Qty { get; set; } // int(10), not null
        public double Discount { get; set; } // double, not null
        public double Amount { get; set; } // double, not null
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
