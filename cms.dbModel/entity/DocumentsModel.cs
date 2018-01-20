using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    public class DocumentsModel
    {
        public Guid id { get; set; }
        [Required]
        public string Title { get; set; }
        public string FilePath { get; set; }
        public DateTime DateCreate { get; set; }
        public Guid idPage { get; set; }
        public int Permit { get; set; }
    }
}
