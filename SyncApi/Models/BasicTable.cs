using System.ComponentModel.DataAnnotations;

namespace SyncApi.Models
{
    public class BasicTable
    {
        [Key]
        public int IdServer { get; set; }

        public bool IsActive { get; set; }

        public BasicTable()
        {

        }
    }
}
