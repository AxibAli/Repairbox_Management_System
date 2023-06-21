using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.DAL.Entities
{
    public class Resource : Base
    {
        [Required]
        [MaxLength(255)]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        [ForeignKey("Permission")]
        public int PermissionId { get; set; }
    }
}
