using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.Social_RankingModels;
using QUIZ_GAME_WEB.Models.SocialRankingModels;

namespace QUIZ_GAME_WEB.Data
{
    public class QuizGameContext : DbContext
    {
        public QuizGameContext(DbContextOptions<QuizGameContext> options) : base(options) { }

        // === 1. KHAI BÁO CÁC DbSet (Entities) ===
        // ... (CORE ENTITIES và QUIZ ENTITIES giữ nguyên)

        // CORE ENTITIES
        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<NguoiDung> NguoiDungs { get; set; } = null!;
        public DbSet<VaiTro> VaiTros { get; set; } = null!;
        public DbSet<Quyen> Quyens { get; set; } = null!;
        public DbSet<VaiTroQuyen> VaiTroQuyens { get; set; } = null!;
        public DbSet<CaiDatNguoiDung> CaiDatNguoiDungs { get; set; } = null!;
        public DbSet<PhienDangNhap> PhienDangNhaps { get; set; } = null!;
        public DbSet<ClientKey> ClientKeys { get; set; } = null!;
        public DbSet<SystemSetting> SystemSettings { get; set; } = null!;

        // QUIZ ENTITIES
        public DbSet<CauHoi> CauHois { get; set; } = null!;
        public DbSet<ChuDe> ChuDes { get; set; } = null!;
        public DbSet<DoKho> DoKhos { get; set; } = null!;
        public DbSet<TroGiup> TroGiups { get; set; } = null!;
        public DbSet<QuizTuyChinh> QuizTuyChinhs { get; set; } = null!;
        public DbSet<QuizNgay> QuizNgays { get; set; } = null!;
        public DbSet<QuizChiaSe> QuizChiaSes { get; set; } = null!;

        // RESULTS & SOCIAL ENTITIES
        public DbSet<KetQua> KetQuas { get; set; } = null!;
        public DbSet<CauSai> CauSais { get; set; } = null!;
        public DbSet<ChuoiNgay> ChuoiNgays { get; set; } = null!;
        public DbSet<ThanhTuu> ThanhTuus { get; set; } = null!;
        public DbSet<ThongKeNguoiDung> ThongKeNguoiDungs { get; set; } = null!;
        public DbSet<ThuongNgay> ThuongNgays { get; set; } = null!;
        public DbSet<BXH> BXHs { get; set; } = null!;
        public DbSet<NguoiDungOnline> NguoiDungOnlines { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!; // 👈 ĐÃ BỔ SUNG

        // === 2. CẤU HÌNH MỐI QUAN HỆ & TÊN BẢNG ===
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tắt tự động thêm "s" ở cuối tên bảng (Pluralization) và ánh xạ tên bảng
            modelBuilder.Entity<Admin>().ToTable("Admin");
            modelBuilder.Entity<NguoiDung>().ToTable("NguoiDung");
            modelBuilder.Entity<VaiTro>().ToTable("VaiTro");
            modelBuilder.Entity<Quyen>().ToTable("Quyen");
            modelBuilder.Entity<VaiTroQuyen>().ToTable("VaiTro_Quyen");
            modelBuilder.Entity<CauHoi>().ToTable("CauHoi");
            modelBuilder.Entity<CaiDatNguoiDung>().ToTable("CaiDatNguoiDung");

            modelBuilder.Entity<Comment>().ToTable("Comment"); // 👈 ĐÃ BỔ SUNG

            // ... (Áp dụng ToTable() cho tất cả các Entities khác theo tên SQL số ít)

            // 1. Cấu hình Khóa Phức Hợp (VaiTro_Quyen)
            modelBuilder.Entity<VaiTroQuyen>()
                .HasKey(vq => new { vq.VaiTroID, vq.QuyenID });

            // 2. Mối quan hệ 1:1 (NguoiDung và Admin)
            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User) // Admin có 1 User
                .WithOne(u => u.AdminInfo) // User có 1 AdminInfo
                .HasForeignKey<Admin>(a => a.UserID);

            // 3. Mối quan hệ 1:1 (NguoiDung và CaiDatNguoiDung)
            modelBuilder.Entity<CaiDatNguoiDung>()
                .HasOne(c => c.NguoiDung)
                .WithOne(u => u.CaiDat)
                .HasForeignKey<CaiDatNguoiDung>(c => c.UserID);

            // 4. Cấu hình Index Unique (TenDangNhap, Email)
            modelBuilder.Entity<NguoiDung>(entity =>
            {
                entity.HasIndex(n => n.TenDangNhap).IsUnique();
                entity.HasIndex(n => n.Email).IsUnique();
            });

            // 5. Cấu hình Quan hệ N:1 cho các FK có thể không theo quy ước
            modelBuilder.Entity<QuizChiaSe>()
                .HasOne(q => q.UserGui)
                .WithMany(u => u.QuizChiaSeGui) // Giả sử đã định nghĩa ICollection này trong NguoiDung
                .HasForeignKey(q => q.UserGuiID);

            // 6. Cấu hình Quan hệ N:1 cho Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany() // Giả định không cần truy cập ngược từ NguoiDung đến Comment
                .HasForeignKey(c => c.UserID);

            // Tùy chọn: Gọi phương thức SeedData
            // modelBuilder.Seed();
        }
    }
}