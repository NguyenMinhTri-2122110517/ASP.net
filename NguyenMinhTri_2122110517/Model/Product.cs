namespace NguyenMinhTri_2122110517.Model
{
    public class Product
    {
        public int Id { get; set; } // bigint(20), auto_increment, not null
        public int Category_Id { get; set; } // int(10), not null
        public int Brand_Id { get; set; } // int(10), not null
        public string Name { get; set; } // varchar(1000), utf8mb4_unicode_ci, not null
        public string Slug { get; set; } // varchar(1000), utf8mb4_unicode_ci, not null
        public string Description { get; set; } // varchar(100), utf8mb4_unicode_ci, nullable
        public double Price { get; set; } // double, not null
        public string Image { get; set; }
        public DateTime Created_At { get; set; } // timestamp, nullable
        public DateTime? Updated_At { get; set; } // timestamp, nullable
        public int Created_By { get; set; } // int(10), not null
        public int? Updated_By { get; set; } // int(10), nullable
        public int Status { get; set; } = 2; // tinyint(3), not null, default 2
        public string Content { get; set; } // longtext, utf8mb4_unicode_ci, not null       
        public Category Category { get; set; }
        public Brand Brand { get; set; }
    }
}
 