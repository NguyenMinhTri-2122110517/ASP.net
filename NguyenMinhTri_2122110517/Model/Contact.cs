namespace NguyenMinhTri_2122110517.Model
{
    public class Contact
    {
        public int Id { get; set; } // bigint(20), auto_increment, not null
        public int User_Id { get; set; } // int(10), nullable
        public string Name { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Email { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Phone { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Title { get; set; } // varchar(255), utf8mb4_unicode_ci, not null
        public string Content { get; set; } // mediumtext, utf8mb4_unicode_ci, not null
        public int Replay_Id { get; set; } = 0; // int(10), not null, default 0
        public DateTime Created_At { get; set; } // datetime, not null, default CURRENT_TIMESTAMP
        public DateTime Updated_At { get; set; } // timestamp, nullable
        public int? Updated_By { get; set; } // int(10), nullable
        public int Status { get; set; } = 2; // tinyint(3), not null, default 2
        public User User { get; set; }
    }
}
