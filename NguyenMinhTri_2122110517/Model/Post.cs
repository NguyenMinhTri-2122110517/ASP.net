namespace NguyenMinhTri_2122110517.Model
{
    public class Post
    {
        public int Id { get; set; } // bigint(20), auto_increment, not null
        public int Topic_Id { get; set; } // int(10), nullable
        public string Title { get; set; } // varchar(1000), utf8mb4_unicode_ci, not null
        public string Content { get; set; } // longtext, utf8mb4_unicode_ci, not null
        public string Description { get; set; } // mediumtext, utf8mb4_unicode_ci, nullable
        public string Image { get; set; } // varchar(1000), utf8mb4_unicode_ci, nullable
        public string Type { get; set; } = "post"; // enum('post','page'), utf8mb4_unicode_ci, not null, default 'post'
        public DateTime Created_At { get; set; } // datetime, nullable
        public DateTime Updated_At { get; set; } // datetime, nullable
        public int Created_By { get; set; } // int(10), not null
        public int Status { get; set; } = 2; // tinyint(3), not null, default 2
        public int? Updated_By { get; set; }
        public Topic Topic { get; set; }
    }
}
