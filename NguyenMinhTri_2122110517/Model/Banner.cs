namespace NguyenMinhTri_2122110517.Model
{
    public class Banner
    {
        public int Id { get; set; } // bigint(20), auto_increment, not null
        public string Name { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Image { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Link { get; set; } // varchar(1000), utf8mb4_unicode_ci, not null
        public int Sort_Order { get; set; } = 0; // int(10), not null, default 0
        public string Position { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Description { get; set; } // varchar(1000), utf8mb4_unicode_ci, nullable
        public int Created_By { get; set; } // int(10), not null
        public int? Updated_By { get; set; } // int(10), nullable
        public DateTime Created_At { get; set; } // timestamp, nullable
        public DateTime Updated_At { get; set; } // timestamp, nullable
        public int Status { get; set; } = 2; // tinyint(3), not null, default 2
    }
}
