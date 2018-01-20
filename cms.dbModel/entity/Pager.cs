using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    public class Pager
    {
        [Required]
        public int page { get; set; }
        [Required]
        public int size { get; set; }
        [Required]
        public int page_count { get; set; }
        [Required]
        public int items_count { get; set; }
    }
}