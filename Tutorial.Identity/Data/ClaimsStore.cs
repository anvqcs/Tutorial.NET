using System.ComponentModel.DataAnnotations;

namespace Tutorial.Identity.Data
{
    public class ClaimsStore
    {
        public int ClaimsStoreId { get; set; }
        [MaxLength(256)]
        public required string Type { get; set; }
        [MaxLength(256)]
        public required string Value {  get; set; }
    }
}
