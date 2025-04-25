using System.ComponentModel.DataAnnotations;

namespace NguyenMinhTri_2122110517.Model
{
    public class Order
    {
        public int Id { get; set; } // bigint(20), auto_increment, not null
        public int User_Id { get; set; } // int(10), not null
        public string Name { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Email { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Phone { get; set; } // varchar(13), utf8mb4_unicode_ci, not null
        public string Address { get; set; } // varchar(1000), utf8mb4_unicode_ci, not null
        public string Note { get; set; } // tinytext, utf8mb4_unicode_ci, nullable
        public DateTime Created_At { get; set; } // timestamp, nullable
        public DateTime Updated_At { get; set; } // timestamp, nullable
        public int? Updated_By { get; set; } // int(10), nullable
        public int Status { get; set; } = 2; // tinyint(3), not null, default 2
        public User User { get; set; }
    }
}
