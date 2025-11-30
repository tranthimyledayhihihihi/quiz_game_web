using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.CoreEntities
{
    [Table("VaiTro")]
    public class VaiTro
    {
        [Key]
        public int VaiTroID { get; set; }

        [Required]
        [MaxLength(50)]
        public string TenVaiTro { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? MoTa { get; set; }

        // Navigation Properties
        public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

        // Chỉ giữ 1 collection N:N với VaiTro_Quyen
     
        public ICollection<VaiTro_Quyen> VaiTro_Quyens { get; set; } = new List<VaiTro_Quyen>();
    }
}
