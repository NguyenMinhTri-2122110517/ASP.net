namespace NguyenMinhTri_2122110517.Model
{
    public class Config
    {
        public int Id { get; set; } // bigint(20), auto_increment, not null
        public string Site_Name { get; set; } // varchar(1000), utf8mb4_unicode_ci, not null
        public string Email { get; set; } // varchar(1000), utf8mb4_unicode_ci, not null
        public string Phone { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Address { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Hotline { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Zalo { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Facebook { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public int Status { get; set; } = 0; // tinyint(4), not null, default 0
    }
}
