using System.ComponentModel.DataAnnotations;

namespace BackEnd
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string ShortIntro { get; set; }
        [Required]
        public string LongIntro { get; set; }
        public string City { get; set; }

        /*id integer [primary key]
  spotify_token varchar
  picture binary
  long_intro varchar
         */
    }
}
