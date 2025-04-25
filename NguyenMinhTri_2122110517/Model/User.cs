namespace NguyenMinhTri_2122110517.Model
{
    public class User
    {
        public int Id { get; set; } // bigint(20), auto_increment, not null
        public string Name { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Fullname { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Password { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Gender { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Phone { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Email { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Roles { get; set; } // enum('admin','customer'), utf8mb4_unicode_ci, not null
        public string Avatar { get; set; } // varchar(100), utf8mb4_unicode_ci, nullable
        public string Address { get; set; } // varchar(255), utf8mb4_unicode_ci, nullable
        public DateTime Created_At { get; set; } // timestamp, nullable
        public DateTime? Updated_At { get; set; } // timestamp, nullable
        public int Created_By { get; set; } // int(10), not null
        public int? Updated_By { get; set; } // int(10), nullable
        public int Status { get; set; } = 2; // tinyint(3), not null, default 2
    }
}
