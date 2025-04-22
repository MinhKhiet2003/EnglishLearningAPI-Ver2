using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishLearningAPI.Models
{
    [Table("payment")]
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        public int Id { get; set; }

        [Column("price")]
        public double Price { get; set; }

        [Column("create_at")]
        public DateTime CreateAt { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public Payment()
        {
        }

        public Payment(double price, DateTime createAt)
        {
            Price = price;
            CreateAt = createAt;
        }
    }
}
