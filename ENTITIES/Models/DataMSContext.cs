using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ENTITIES.Models
{
    public partial class DataMSContext : DbContext
    {
        public DataMSContext()
        {
        }

        public DataMSContext(DbContextOptions<DataMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountClient> AccountClient { get; set; }
        public virtual DbSet<Action> Action { get; set; }
        public virtual DbSet<AddressClient> AddressClient { get; set; }
        public virtual DbSet<AffiliateGroupProduct> AffiliateGroupProduct { get; set; }
        public virtual DbSet<AirPortCode> AirPortCode { get; set; }
        public virtual DbSet<Airlines> Airlines { get; set; }
        public virtual DbSet<AllCode> AllCode { get; set; }
        public virtual DbSet<AllotmentFund> AllotmentFund { get; set; }
        public virtual DbSet<AllotmentHistory> AllotmentHistory { get; set; }
        public virtual DbSet<AllotmentUse> AllotmentUse { get; set; }
        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<ArticleCategory> ArticleCategory { get; set; }
        public virtual DbSet<ArticleRelated> ArticleRelated { get; set; }
        public virtual DbSet<ArticleTag> ArticleTag { get; set; }
        public virtual DbSet<AttachFile> AttachFile { get; set; }
        public virtual DbSet<Baggage> Baggage { get; set; }
        public virtual DbSet<BankOnePay> BankOnePay { get; set; }
        public virtual DbSet<BankingAccount> BankingAccount { get; set; }
        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<Campaign> Campaign { get; set; }
        public virtual DbSet<CampaignAds> CampaignAds { get; set; }
        public virtual DbSet<Cashback> Cashback { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<ClientLinkAff> ClientLinkAff { get; set; }
        public virtual DbSet<Comments> Comments { get; set; }
        public virtual DbSet<ContactClient> ContactClient { get; set; }
        public virtual DbSet<Contract> Contract { get; set; }
        public virtual DbSet<ContractHistory> ContractHistory { get; set; }
        public virtual DbSet<ContractPay> ContractPay { get; set; }
        public virtual DbSet<ContractPayDetail> ContractPayDetail { get; set; }
        public virtual DbSet<DebtStatistic> DebtStatistic { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<DepositHistory> DepositHistory { get; set; }
        public virtual DbSet<District> District { get; set; }
        public virtual DbSet<FlightSegment> FlightSegment { get; set; }
        public virtual DbSet<FlyBookingDetail> FlyBookingDetail { get; set; }
        public virtual DbSet<FlyBookingExtraPackages> FlyBookingExtraPackages { get; set; }
        public virtual DbSet<FlyBookingPackagesOptional> FlyBookingPackagesOptional { get; set; }
        public virtual DbSet<GroupClassAirlines> GroupClassAirlines { get; set; }
        public virtual DbSet<GroupClassAirlinesDetail> GroupClassAirlinesDetail { get; set; }
        public virtual DbSet<GroupProduct> GroupProduct { get; set; }
        public virtual DbSet<Hotel> Hotel { get; set; }
        public virtual DbSet<HotelBankingAccount> HotelBankingAccount { get; set; }
        public virtual DbSet<HotelBooking> HotelBooking { get; set; }
        public virtual DbSet<HotelBookingCode> HotelBookingCode { get; set; }
        public virtual DbSet<HotelBookingRoomExtraPackages> HotelBookingRoomExtraPackages { get; set; }
        public virtual DbSet<HotelBookingRoomRates> HotelBookingRoomRates { get; set; }
        public virtual DbSet<HotelBookingRoomRatesOptional> HotelBookingRoomRatesOptional { get; set; }
        public virtual DbSet<HotelBookingRooms> HotelBookingRooms { get; set; }
        public virtual DbSet<HotelBookingRoomsOptional> HotelBookingRoomsOptional { get; set; }
        public virtual DbSet<HotelContact> HotelContact { get; set; }
        public virtual DbSet<HotelGuest> HotelGuest { get; set; }
        public virtual DbSet<HotelPosition> HotelPosition { get; set; }
        public virtual DbSet<HotelRoom> HotelRoom { get; set; }
        public virtual DbSet<HotelSupplier> HotelSupplier { get; set; }
        public virtual DbSet<HotelSurcharge> HotelSurcharge { get; set; }
        public virtual DbSet<ImageSize> ImageSize { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<InvoiceDetail> InvoiceDetail { get; set; }
        public virtual DbSet<InvoiceRequest> InvoiceRequest { get; set; }
        public virtual DbSet<InvoiceRequestDetail> InvoiceRequestDetail { get; set; }
        public virtual DbSet<InvoiceRequestHistory> InvoiceRequestHistory { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<Label> Label { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<MenuPermissions> MenuPermissions { get; set; }
        public virtual DbSet<Mfauser> Mfauser { get; set; }
        public virtual DbSet<National> National { get; set; }
        public virtual DbSet<Note> Note { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderBak> OrderBak { get; set; }
        public virtual DbSet<OrderBookClosing> OrderBookClosing { get; set; }
        public virtual DbSet<OtherBooking> OtherBooking { get; set; }
        public virtual DbSet<OtherBookingPackages> OtherBookingPackages { get; set; }
        public virtual DbSet<OtherBookingPackagesOptional> OtherBookingPackagesOptional { get; set; }
        public virtual DbSet<Passenger> Passenger { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<PaymentAccount> PaymentAccount { get; set; }
        public virtual DbSet<PaymentRequest> PaymentRequest { get; set; }
        public virtual DbSet<PaymentRequestDetail> PaymentRequestDetail { get; set; }
        public virtual DbSet<PaymentVoucher> PaymentVoucher { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<PlaygroundDetail> PlaygroundDetail { get; set; }
        public virtual DbSet<Policy> Policy { get; set; }
        public virtual DbSet<PolicyDetail> PolicyDetail { get; set; }
        public virtual DbSet<Position> Position { get; set; }
        public virtual DbSet<PriceDetail> PriceDetail { get; set; }
        public virtual DbSet<PriceLimitedSetting> PriceLimitedSetting { get; set; }
        public virtual DbSet<ProductFlyTicketService> ProductFlyTicketService { get; set; }
        public virtual DbSet<ProductRoomService> ProductRoomService { get; set; }
        public virtual DbSet<ProgramModification> ProgramModification { get; set; }
        public virtual DbSet<ProgramPackage> ProgramPackage { get; set; }
        public virtual DbSet<ProgramPackageDaily> ProgramPackageDaily { get; set; }
        public virtual DbSet<Programs> Programs { get; set; }
        public virtual DbSet<Province> Province { get; set; }
        public virtual DbSet<ProvinceHotel> ProvinceHotel { get; set; }
        public virtual DbSet<Request> Request { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RolePermission> RolePermission { get; set; }
        public virtual DbSet<RoomFacilities> RoomFacilities { get; set; }
        public virtual DbSet<RoomFun> RoomFun { get; set; }
        public virtual DbSet<RoomPackage> RoomPackage { get; set; }
        public virtual DbSet<RunningScheduleService> RunningScheduleService { get; set; }
        public virtual DbSet<ServiceDeclines> ServiceDeclines { get; set; }
        public virtual DbSet<ServicePiceRoom> ServicePiceRoom { get; set; }
        public virtual DbSet<SportWaterGuests> SportWaterGuests { get; set; }
        public virtual DbSet<SportWaterPackages> SportWaterPackages { get; set; }
        public virtual DbSet<Supplier> Supplier { get; set; }
        public virtual DbSet<SupplierContact> SupplierContact { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<TelegramDetail> TelegramDetail { get; set; }
        public virtual DbSet<Tour> Tour { get; set; }
        public virtual DbSet<TourDestination> TourDestination { get; set; }
        public virtual DbSet<TourGuests> TourGuests { get; set; }
        public virtual DbSet<TourPackages> TourPackages { get; set; }
        public virtual DbSet<TourPackagesOptional> TourPackagesOptional { get; set; }
        public virtual DbSet<TourPosition> TourPosition { get; set; }
        public virtual DbSet<TourProduct> TourProduct { get; set; }
        public virtual DbSet<TourProgramPackages> TourProgramPackages { get; set; }
        public virtual DbSet<Transactions> Transactions { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAgent> UserAgent { get; set; }
        public virtual DbSet<UserDepart> UserDepart { get; set; }
        public virtual DbSet<UserPosition> UserPosition { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<VinWonderBooking> VinWonderBooking { get; set; }
        public virtual DbSet<VinWonderBookingTicket> VinWonderBookingTicket { get; set; }
        public virtual DbSet<VinWonderBookingTicketCustomer> VinWonderBookingTicketCustomer { get; set; }
        public virtual DbSet<VinWonderBookingTicketDetail> VinWonderBookingTicketDetail { get; set; }
        public virtual DbSet<VinWonderPricePolicy> VinWonderPricePolicy { get; set; }
        public virtual DbSet<Voucher> Voucher { get; set; }
        public virtual DbSet<VoucherCampaign> VoucherCampaign { get; set; }
        public virtual DbSet<VoucherLogActivity> VoucherLogActivity { get; set; }
        public virtual DbSet<Ward> Ward { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=103.163.216.41;Initial Catalog=adavigo;Persist Security Info=True;User ID=us;Password=us@585668");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountClient>(entity =>
            {
                entity.Property(e => e.ForgotPasswordToken)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordBackup)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Action>(entity =>
            {
                entity.Property(e => e.ActionName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ControllerName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.Action)
                    .HasForeignKey(d => d.MenuId)
                    .HasConstraintName("FK_Action_Menu");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.Action)
                    .HasForeignKey(d => d.PermissionId)
                    .HasConstraintName("FK_Action_Permission");
            });

            modelBuilder.Entity<AddressClient>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.DistrictId).HasMaxLength(5);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("Đây là số điện thoại nhận hàng");

                entity.Property(e => e.ProvinceId).HasMaxLength(5);

                entity.Property(e => e.ReceiverName).HasMaxLength(255);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.WardId).HasMaxLength(5);
            });

            modelBuilder.Entity<AirPortCode>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.DistrictEn)
                    .HasColumnName("District_En")
                    .HasMaxLength(100);

                entity.Property(e => e.DistrictVi)
                    .HasColumnName("District_Vi")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Airlines>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK_Airlines_1");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AirLineGroup)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Logo).HasMaxLength(150);

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasColumnName("Name_En")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NameVi)
                    .IsRequired()
                    .HasColumnName("Name_Vi")
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<AllCode>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(300);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<AllotmentFund>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<AllotmentHistory>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.HasOne(d => d.AllotmentFund)
                    .WithMany(p => p.AllotmentHistory)
                    .HasForeignKey(d => d.AllotmentFundId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AllotmentHistory_AllotmentFund");
            });

            modelBuilder.Entity<AllotmentUse>(entity =>
            {
                entity.Property(e => e.AllomentFundId).HasComment("Thông tin số tiền của quỹ đã được phân bổ");

                entity.Property(e => e.AmountUse).HasComment("Số tiền đã sử dụng cho dịch vụ");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày tạo đơn hàng");

                entity.Property(e => e.DataId).HasComment("Là lưu trữ id dịch vụ");

                entity.HasOne(d => d.AllomentFund)
                    .WithMany(p => p.AllotmentUse)
                    .HasForeignKey(d => d.AllomentFundId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AllotmentUse_AllotmentFund");
            });

            modelBuilder.Entity<Article>(entity =>
            {
                entity.Property(e => e.Body)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.DownTime).HasColumnType("datetime");

                entity.Property(e => e.Image11).HasMaxLength(350);

                entity.Property(e => e.Image169)
                    .IsRequired()
                    .HasMaxLength(350);

                entity.Property(e => e.Image43).HasMaxLength(350);

                entity.Property(e => e.Lead)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.PublishDate).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.UpTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ArticleRelated>(entity =>
            {
                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticleRelated)
                    .HasForeignKey(d => d.ArticleId)
                    .HasConstraintName("FK_ArticleRelated_Article");
            });

            modelBuilder.Entity<ArticleTag>(entity =>
            {
                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticleTag)
                    .HasForeignKey(d => d.ArticleId)
                    .HasConstraintName("FK_ArticleTags_Article");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.ArticleTag)
                    .HasForeignKey(d => d.TagId)
                    .HasConstraintName("FK_ArticleTags_Tags");
            });

            modelBuilder.Entity<AttachFile>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Ext)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Path).HasMaxLength(400);
            });

            modelBuilder.Entity<Baggage>(entity =>
            {
                entity.Property(e => e.Airline)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Currency).HasMaxLength(50);

                entity.Property(e => e.EndPoint).HasMaxLength(250);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.StartPoint).HasMaxLength(250);

                entity.Property(e => e.StatusCode).HasMaxLength(50);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<BankOnePay>(entity =>
            {
                entity.Property(e => e.BankName)
                    .IsRequired()
                    .HasColumnName("bank_name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FullnameEn)
                    .HasColumnName("fullname_en")
                    .HasMaxLength(200);

                entity.Property(e => e.FullnameVi)
                    .HasColumnName("fullname_vi")
                    .HasMaxLength(200);

                entity.Property(e => e.Logo)
                    .IsRequired()
                    .HasColumnName("logo")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Type).HasColumnName("type");
            });

            modelBuilder.Entity<BankingAccount>(entity =>
            {
                entity.Property(e => e.AccountName).HasMaxLength(200);

                entity.Property(e => e.AccountNumber)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.BankId)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.Property(e => e.BrandCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BrandName).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.Property(e => e.CampaignCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.FromDate).HasColumnType("datetime");

                entity.Property(e => e.ToDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateLast).HasColumnType("datetime");
            });

            modelBuilder.Entity<CampaignAds>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CampaignName).HasMaxLength(300);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(400);

                entity.Property(e => e.StartDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Cashback>(entity =>
            {
                entity.Property(e => e.CashbackDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(500);
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.Avartar)
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.BusinessAddress).HasMaxLength(200);

                entity.Property(e => e.ClientCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ClientName).HasMaxLength(256);

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ExportBillAddress).HasMaxLength(200);

                entity.Property(e => e.IsReceiverInfoEmail).HasColumnName("isReceiverInfoEmail");

                entity.Property(e => e.JoinDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(400);

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReferralId)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.TaxNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ClientLinkAff>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ClientId).ValueGeneratedOnAdd();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.LinkAff)
                    .IsRequired()
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Comments>(entity =>
            {
                entity.Property(e => e.AttachFile)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.AttachFileName).HasMaxLength(255);

                entity.Property(e => e.Content).HasMaxLength(1000);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ContactClient>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.Property(e => e.ContractDate).HasColumnType("datetime");

                entity.Property(e => e.ContractNo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.DebtType).HasComment("1: 7 ngày, 2: 15 ngày");

                entity.Property(e => e.ExpireDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.ServiceType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalVerify).HasComment("Tổng số lần được duyệt của hợp đồng. Cộng dồn sau mỗi lần duyệt");

                entity.Property(e => e.UpdateLast).HasColumnType("datetime");

                entity.Property(e => e.UserIdVerify).HasComment("AccountClientID là user sẽ duyệt hđ này");

                entity.Property(e => e.VerifyDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày duyệt hợp đồng");
            });

            modelBuilder.Entity<ContractHistory>(entity =>
            {
                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ActionDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ContractPay>(entity =>
            {
                entity.HasKey(e => e.PayId);

                entity.HasIndex(e => e.ClientId);

                entity.Property(e => e.AttatchmentFile)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.BillNo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ExportDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày xuất hóa đơn");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.Note).HasMaxLength(300);

                entity.Property(e => e.PayType).HasComment("1: Tiền mặt , 2: Chuyển khoản");

                entity.Property(e => e.Type).HasComment("1:Thu tiền đơn hàng , 2: Thu tiền nạp quỹ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ContractPayDetail>(entity =>
            {
                entity.HasIndex(e => e.DataId);

                entity.HasIndex(e => e.PayId);

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ServiceCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<DebtStatistic>(entity =>
            {
                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Currency).HasMaxLength(50);

                entity.Property(e => e.DeclineReason).HasMaxLength(500);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.FromDate).HasColumnType("date");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.OrderIds)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ToDate).HasColumnType("date");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VerifyDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DepartmentCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DepartmentName).HasMaxLength(500);

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.FullParent)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsReport).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<DepositHistory>(entity =>
            {
                entity.Property(e => e.BankAccount).HasMaxLength(150);

                entity.Property(e => e.BankName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasComment("Thời gian giao dịch");

                entity.Property(e => e.ImageScreen)
                    .HasMaxLength(200)
                    .HasComment("Ảnh ủy nhiệm chi");

                entity.Property(e => e.NoteReject).HasMaxLength(300);

                entity.Property(e => e.PaymentType).HasComment("HÌnh thức thanh toán");

                entity.Property(e => e.Price).HasComment("Số tiền nạp");

                entity.Property(e => e.Status).HasComment("Trạng thái");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasComment("Tiêu đề nạp");

                entity.Property(e => e.TransNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("Mã giao dịch");

                entity.Property(e => e.TransType).HasComment("Loại giao dịch");

                entity.Property(e => e.UpdateLast).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasComment("User nạp trans. Lấy user login");

                entity.Property(e => e.VerifyDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.Property(e => e.DistrictId)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.Property(e => e.Location).HasMaxLength(30);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NameNonUnicode)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ProvinceId)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<FlightSegment>(entity =>
            {
                entity.Property(e => e.AllowanceBaggage).HasMaxLength(50);

                entity.Property(e => e.Class)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.EndPoint)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.EndTerminal).HasMaxLength(250);

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.FlightNumber)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.FlyBookingId).HasColumnName("FlyBookingID");

                entity.Property(e => e.HandBaggage).HasMaxLength(50);

                entity.Property(e => e.OperatingAirline)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Plane)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.StartPoint)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.StartTerminal).HasMaxLength(250);

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.StopPoint).HasMaxLength(250);
            });

            modelBuilder.Entity<FlyBookingDetail>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("IDX_FlyBookingDetail_OrderId");

                entity.Property(e => e.Adgcommission).HasColumnName("ADGCommission");

                entity.Property(e => e.Airline)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.BookingCode).HasMaxLength(250);

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.EndPoint).HasMaxLength(50);

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.Flight).HasMaxLength(250);

                entity.Property(e => e.GroupBookingId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.GroupClass).HasMaxLength(100);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.ServiceCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Session)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.StartPoint).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<FlyBookingExtraPackages>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BasePrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.GroupFlyBookingId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PackageCode)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PackageId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<FlyBookingPackagesOptional>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.PackageName).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<GroupClassAirlines>(entity =>
            {
                entity.Property(e => e.Airline)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ClassCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.DetailEn)
                    .IsRequired()
                    .HasColumnName("Detail_En")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DetailVi)
                    .IsRequired()
                    .HasColumnName("Detail_Vi")
                    .HasMaxLength(50);

                entity.Property(e => e.FareType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<GroupClassAirlinesDetail>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300);
            });

            modelBuilder.Entity<GroupProduct>(entity =>
            {
                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ImagePath)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.Path)
                    .HasMaxLength(400)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.Property(e => e.CheckinTime).HasColumnType("datetime");

                entity.Property(e => e.CheckoutTime).HasColumnType("datetime");

                entity.Property(e => e.City).HasMaxLength(200);

                entity.Property(e => e.Country).HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Email).HasMaxLength(150);

                entity.Property(e => e.Extends).HasColumnType("text");

                entity.Property(e => e.GroupName).HasMaxLength(200);

                entity.Property(e => e.HotelId)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.HotelType).HasMaxLength(200);

                entity.Property(e => e.ImageThumb).HasMaxLength(500);

                entity.Property(e => e.IsDisplayWebsite).HasDefaultValueSql("((0))");

                entity.Property(e => e.ListSupplierId)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.OtherSurcharge)
                    .HasMaxLength(500)
                    .HasComment("");

                entity.Property(e => e.ReviewRate).HasColumnType("decimal(6, 2)");

                entity.Property(e => e.ShortName).HasMaxLength(50);

                entity.Property(e => e.Star).HasColumnType("decimal(6, 2)");

                entity.Property(e => e.State).HasMaxLength(500);

                entity.Property(e => e.Street).HasMaxLength(1000);

                entity.Property(e => e.TaxCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Telephone)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.TypeOfRoom).HasMaxLength(200);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HotelBankingAccount>(entity =>
            {
                entity.Property(e => e.AccountName).HasMaxLength(200);

                entity.Property(e => e.AccountNumber)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.BankId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Branch).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HotelBooking>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("IDX_HotelBooking_OrderId");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.ArrivalDate).HasColumnType("datetime");

                entity.Property(e => e.BookingId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CheckinTime).HasColumnType("datetime");

                entity.Property(e => e.CheckoutTime).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DepartureDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(150);

                entity.Property(e => e.HotelName).HasMaxLength(250);

                entity.Property(e => e.ImageThumb).HasMaxLength(750);

                entity.Property(e => e.Note).HasMaxLength(1000);

                entity.Property(e => e.NumberOfAdult).HasColumnName("numberOfAdult");

                entity.Property(e => e.NumberOfChild).HasColumnName("numberOfChild");

                entity.Property(e => e.NumberOfInfant).HasColumnName("numberOfInfant");

                entity.Property(e => e.NumberOfPeople).HasColumnName("numberOfPeople");

                entity.Property(e => e.NumberOfRoom).HasColumnName("numberOfRoom");

                entity.Property(e => e.PropertyId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ReservationCode).HasMaxLength(150);

                entity.Property(e => e.ServiceCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Telephone).HasMaxLength(150);

                entity.Property(e => e.TotalAmount).HasColumnName("totalAmount");

                entity.Property(e => e.TotalPrice).HasColumnName("totalPrice");

                entity.Property(e => e.TotalProfit).HasColumnName("totalProfit");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HotelBookingCode>(entity =>
            {
                entity.Property(e => e.AttactFile)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.BookingCode)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HotelBookingRoomExtraPackages>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.HotelBookingRoomId).HasColumnName("HotelBookingRoomID");

                entity.Property(e => e.OperatorPrice).HasComment("Giá điều hành nhập 1 phòng/1 đêm");

                entity.Property(e => e.PackageCode).HasMaxLength(200);

                entity.Property(e => e.PackageId).HasMaxLength(50);

                entity.Property(e => e.SalePrice).HasComment("Giá sale nhập 1 phòng/1 đêm");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HotelBookingRoomRates>(entity =>
            {
                entity.Property(e => e.AllotmentId).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.OperatorPrice).HasComment("Giá điều hành nhập 1 phòng/1 đêm");

                entity.Property(e => e.PackagesInclude).HasMaxLength(500);

                entity.Property(e => e.RatePlanCode).HasMaxLength(50);

                entity.Property(e => e.RatePlanId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SalePrice).HasComment("Giá sale nhập 1 phòng/1 đêm");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.StayDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HotelBookingRoomRatesOptional>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.OperatorPrice).HasComment("Giá điều hành nhập 1 phòng/1 đêm");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HotelBookingRooms>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.IsRoomFund).HasComment("");

                entity.Property(e => e.NumberOfAdult).HasColumnName("numberOfAdult");

                entity.Property(e => e.NumberOfChild).HasColumnName("numberOfChild");

                entity.Property(e => e.NumberOfInfant).HasColumnName("numberOfInfant");

                entity.Property(e => e.PackageIncludes).HasMaxLength(250);

                entity.Property(e => e.RoomTypeCode).HasMaxLength(50);

                entity.Property(e => e.RoomTypeId)
                    .IsRequired()
                    .HasColumnName("RoomTypeID")
                    .HasMaxLength(50);

                entity.Property(e => e.RoomTypeName).HasMaxLength(200);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HotelBookingRoomsOptional>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PackageName).HasMaxLength(200);

                entity.Property(e => e.Price).HasComment("Giá đã trừ đi lợi nhuận");

                entity.Property(e => e.Profit).HasComment("Lợi nhuận");

                entity.Property(e => e.TotalAmount).HasComment("Tổng tiền dịch vụ khách phải trả");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HotelContact>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Position).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HotelGuest>(entity =>
            {
                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.HotelBookingRoomsId).HasColumnName("HotelBookingRoomsID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Note).HasMaxLength(250);
            });

            modelBuilder.Entity<HotelPosition>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PositionType).HasComment("1: B2B, 2: B2C");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HotelRoom>(entity =>
            {
                entity.Property(e => e.Avatar)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Extends).HasMaxLength(500);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDisplayWebsite).HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.RoomAvatar)
                    .HasMaxLength(8000)
                    .IsUnicode(false);

                entity.Property(e => e.RoomId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Thumbnails)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TypeOfRoom).HasMaxLength(200);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HotelSupplier>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<HotelSurcharge>(entity =>
            {
                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.FromDate)
                    .HasColumnType("datetime")
                    .HasComment("");

                entity.Property(e => e.Name).HasMaxLength(250);

                entity.Property(e => e.ToDate)
                    .HasColumnType("datetime")
                    .HasComment("");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ImageSize>(entity =>
            {
                entity.Property(e => e.PositionName)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.Property(e => e.AttactFile)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExportDate).HasColumnType("date");

                entity.Property(e => e.InvoiceCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.InvoiceFromId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.InvoiceNo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.InvoiceSignId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("date");

                entity.Property(e => e.VerifyDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<InvoiceDetail>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("date");
            });

            modelBuilder.Entity<InvoiceRequest>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.AttachFile)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyName).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeclineReason).HasMaxLength(500);

                entity.Property(e => e.InvoiceRequestNo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.PlanDate).HasColumnType("date");

                entity.Property(e => e.TaxNo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VerifyDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<InvoiceRequestDetail>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ProductName).HasMaxLength(500);

                entity.Property(e => e.Unit).HasMaxLength(200);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat).HasColumnName("VAT");
            });

            modelBuilder.Entity<InvoiceRequestHistory>(entity =>
            {
                entity.Property(e => e.Actioin).HasMaxLength(4000);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.Property(e => e.Type).HasComment("1: sync client ; 2 : sync order");
            });

            modelBuilder.Entity<Label>(entity =>
            {
                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.DescExpire).HasMaxLength(300);

                entity.Property(e => e.Domain)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Icon).HasMaxLength(500);

                entity.Property(e => e.PrefixOrderCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.StoreName).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.FullParent)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Icon).HasMaxLength(50);

                entity.Property(e => e.Link).HasMaxLength(200);

                entity.Property(e => e.MenuCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Title).HasMaxLength(250);
            });

            modelBuilder.Entity<Mfauser>(entity =>
            {
                entity.ToTable("MFAUser");

                entity.Property(e => e.BackupCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SecretKey)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsFixedLength();

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.UserCreatedYear)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<National>(entity =>
            {
                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameVn)
                    .HasColumnName("NameVN")
                    .HasMaxLength(200);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.Property(e => e.Comment).HasMaxLength(400);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.BankCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ClosingEndDate).HasColumnType("datetime");

                entity.Property(e => e.ColorCode).HasMaxLength(10);

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.DebtNote).HasMaxLength(500);

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngay ket thuc dich vu");

                entity.Property(e => e.ExpriryDate).HasColumnType("datetime");

                entity.Property(e => e.FinalizeDate).HasColumnType("date");

                entity.Property(e => e.IsLock).HasDefaultValueSql("((0))");

                entity.Property(e => e.Label).HasMaxLength(500);

                entity.Property(e => e.Note)
                    .HasMaxLength(300)
                    .HasComment("Chính là label so với wiframe");

                entity.Property(e => e.OperatorId)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.OrderNo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentNo).HasMaxLength(250);

                entity.Property(e => e.ProductService)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Refund).HasDefaultValueSql("((0))");

                entity.Property(e => e.SalerGroupId)
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.SmsContent).HasMaxLength(400);

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasComment("ngay bat dau khoi tao dich vu");

                entity.Property(e => e.UpdateLast).HasColumnType("datetime");

                entity.Property(e => e.UtmMedium)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UtmSource)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.VerifyDate).HasColumnType("datetime");

                entity.HasOne(d => d.ContactClient)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.ContactClientId)
                    .HasConstraintName("FK_Order_ContactClient");

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.ContractId)
                    .HasConstraintName("FK_Order_Contract");
            });

            modelBuilder.Entity<OrderBak>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Order_bak");

                entity.Property(e => e.BankCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ColorCode).HasMaxLength(10);

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(250);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.ExpriryDate).HasColumnType("datetime");

                entity.Property(e => e.Label).HasMaxLength(500);

                entity.Property(e => e.Note).HasMaxLength(300);

                entity.Property(e => e.OrderId).ValueGeneratedOnAdd();

                entity.Property(e => e.OrderNo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentNo).HasMaxLength(250);

                entity.Property(e => e.ProductService)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SalerGroupId)
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.SmsContent).HasMaxLength(400);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateLast).HasColumnType("datetime");
            });

            modelBuilder.Entity<OrderBookClosing>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FinalizeDate).HasColumnType("date");

                entity.Property(e => e.FromDate).HasColumnType("date");

                entity.Property(e => e.ToDate).HasColumnType("date");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<OtherBooking>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("IDX_OtherBooking_OrderId");

                entity.Property(e => e.ConfNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(1000);

                entity.Property(e => e.OperatorId).HasColumnName("OperatorID");

                entity.Property(e => e.RoomNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SerialNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ServiceCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<OtherBookingPackages>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BasePrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Commission)
                    .HasColumnType("decimal(18, 2)")
                    .HasComment("");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<OtherBookingPackagesOptional>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.PackageName).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Passenger>(entity =>
            {
                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.GroupBookingId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.MembershipCard)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Note).HasMaxLength(250);

                entity.Property(e => e.PersonType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Passenger)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Passenger_Order");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.BankName).HasMaxLength(50);

                entity.Property(e => e.BotPaymentScreenShot).HasMaxLength(250);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.DepositPaymentType).HasComment("loại thanh toán cho đối tượng nào. Đơn hàng hay nạp quỹ");

                entity.Property(e => e.ImageScreenShot).HasMaxLength(250);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.Property(e => e.TransferContent).HasMaxLength(250);
            });

            modelBuilder.Entity<PaymentAccount>(entity =>
            {
                entity.Property(e => e.AccountName)
                    .HasMaxLength(50)
                    .HasComment("Tên chủ tài khoản");

                entity.Property(e => e.AccountNumb)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("Số tài khoản");

                entity.Property(e => e.BankName)
                    .HasMaxLength(50)
                    .HasComment("Tên ngân hàng");

                entity.Property(e => e.Branch)
                    .HasMaxLength(50)
                    .HasComment("Chi nhánh");
            });

            modelBuilder.Entity<PaymentRequest>(entity =>
            {
                entity.HasIndex(e => e.SupplierId)
                    .HasName("IX_PaymentRequest_SupplierId_Status");

                entity.Property(e => e.AbandonmentReason).HasMaxLength(500);

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BankAccount)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BankName).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DeclineReason).HasMaxLength(500);

                entity.Property(e => e.Description).HasMaxLength(3000);

                entity.Property(e => e.IsPaymentBefore).HasDefaultValueSql("((0))");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.PaymentCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.Property(e => e.Type).HasComment("1: Thanh toán dịch vụ , 2: Thanh toán khác");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VerifyDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<PaymentRequestDetail>(entity =>
            {
                entity.HasIndex(e => e.RequestId)
                    .HasName("IDX_PaymentRequestDetail_RequestId");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ServiceCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<PaymentVoucher>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.AttachFiles)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.BankAccount)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.BankName).HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.PaymentCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RequestId)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasComment("Id Phiếu yêu cầu chi");

                entity.Property(e => e.Type).HasComment("1: Thanh toán dịch vụ , 2: Thanh toán khác");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<PlaygroundDetail>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.LocationName).HasMaxLength(250);

                entity.Property(e => e.NewsId).HasComment("Bài viết liên quan");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Policy>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EffectiveDate).HasColumnType("datetime");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsPrivate).HasDefaultValueSql("((0))");

                entity.Property(e => e.PolicyCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PolicyName).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<PolicyDetail>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.HotelDebtAmout).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.HotelDepositAmout).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductFlyTicketDebtAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductFlyTicketDepositAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TourDebtAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TourDepositAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TouringCarDebtAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TouringCarDepositAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VinWonderDebtAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VinWonderDepositAmount).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.Property(e => e.PositionName)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<PriceDetail>(entity =>
            {
                entity.Property(e => e.DayList)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FromDate).HasColumnType("datetime");

                entity.Property(e => e.MonthList)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ToDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<PriceLimitedSetting>(entity =>
            {
                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProductFlyTicketService>(entity =>
            {
                entity.Property(e => e.GroupProviderType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Campaign)
                    .WithMany(p => p.ProductFlyTicketService)
                    .HasForeignKey(d => d.CampaignId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductFlyTicketService_Campaign");
            });

            modelBuilder.Entity<ProductRoomService>(entity =>
            {
                entity.Property(e => e.AllotmentsId)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.PackageCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.RoomId).HasComment("");
            });

            modelBuilder.Entity<ProgramModification>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VerifyDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProgramPackage>(entity =>
            {
                entity.Property(e => e.ApplyDate).HasColumnType("date");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FromDate).HasColumnType("date");

                entity.Property(e => e.PackageCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PackageName).HasMaxLength(200);

                entity.Property(e => e.RoomType).HasMaxLength(50);

                entity.Property(e => e.ToDate).HasColumnType("date");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProgramPackageDaily>(entity =>
            {
                entity.Property(e => e.ApplyDate).HasColumnType("date");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FromDate).HasColumnType("date");

                entity.Property(e => e.PackageCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PackageName).HasMaxLength(200);

                entity.Property(e => e.RoomType).HasMaxLength(50);

                entity.Property(e => e.ToDate).HasColumnType("date");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Programs>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.ProgramCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProgramName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ServiceName).HasMaxLength(500);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.StayEndDate).HasColumnType("datetime");

                entity.Property(e => e.StayStartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VerifyDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NameNonUnicode)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ProvinceId)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<ProvinceHotel>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FromDate).HasColumnType("date");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.RequestNo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ToDate).HasColumnType("date");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VoucherName)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.RolePermission)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePermission_Menu");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.RolePermission)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePermission_Permission");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RolePermission)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePermission_Role");
            });

            modelBuilder.Entity<RoomFacilities>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<RoomFun>(entity =>
            {
                entity.Property(e => e.AllotmentId)
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.AllotmentName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.HotelId).IsUnicode(false);

                entity.Property(e => e.HotelName).HasMaxLength(250);

                entity.Property(e => e.UpdateLast).HasColumnType("datetime");
            });

            modelBuilder.Entity<RoomPackage>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.PackageId).IsUnicode(false);

                entity.Property(e => e.UpdateLast).HasColumnType("datetime");

                entity.HasOne(d => d.RoomFun)
                    .WithMany(p => p.RoomPackage)
                    .HasForeignKey(d => d.RoomFunId)
                    .HasConstraintName("FK_RoomPackage_RoomFun1");
            });

            modelBuilder.Entity<RunningScheduleService>(entity =>
            {
                entity.Property(e => e.LogDate).HasColumnType("datetime");

                entity.HasOne(d => d.Price)
                    .WithMany(p => p.RunningScheduleService)
                    .HasForeignKey(d => d.PriceId)
                    .HasConstraintName("FK_RunningScheduleService_Price");
            });

            modelBuilder.Entity<ServiceDeclines>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(1000);

                entity.Property(e => e.ServiceId)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("date");
            });

            modelBuilder.Entity<ServicePiceRoom>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.HotelId)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.RoomCode).HasMaxLength(100);

                entity.Property(e => e.RoomId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RoomName).HasMaxLength(250);

                entity.Property(e => e.UpdateLast).HasColumnType("datetime");

                entity.HasOne(d => d.RoomPackage)
                    .WithMany(p => p.ServicePiceRoom)
                    .HasForeignKey(d => d.RoomPackageId)
                    .HasConstraintName("FK_ServicePiceRoom_RoomPackage");
            });

            modelBuilder.Entity<SportWaterGuests>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.RoomNo)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(200);
            });

            modelBuilder.Entity<SportWaterPackages>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EffectDate).HasColumnType("datetime");

                entity.Property(e => e.ExpireDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Email)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(500);

                entity.Property(e => e.IsDisplayWebsite).HasDefaultValueSql("((0))");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ResidenceType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ServiceType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShortName).HasMaxLength(50);

                entity.Property(e => e.SupplierCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TaxCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<SupplierContact>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Position).HasMaxLength(500);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.TagName).HasMaxLength(100);
            });

            modelBuilder.Entity<TelegramDetail>(entity =>
            {
                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.GroupChatId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.GroupLog)
                    .IsRequired()
                    .HasMaxLength(80);

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(400)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Tour>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("IDX_Tour_OrderId");

                entity.Property(e => e.AdditionInfo).HasColumnType("text");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Image)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.IsDisplayWeb).HasDefaultValueSql("((0))");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.Schedule).HasColumnType("text");

                entity.Property(e => e.ServiceCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TourDestination>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Type).HasComment("Lưu theo Tour Type ");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TourGuests>(entity =>
            {
                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.Cccd)
                    .HasColumnName("CCCD")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FullName).HasMaxLength(200);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RoomNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TourPackages>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PackageCode).HasMaxLength(50);

                entity.Property(e => e.PackageName).HasMaxLength(200);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vat).HasColumnName("VAT");
            });

            modelBuilder.Entity<TourPackagesOptional>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.PackageName).HasMaxLength(200);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TourPosition>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PositionType).HasComment("1: B2C , 2: B2B");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TourProduct>(entity =>
            {
                entity.Property(e => e.AdditionInfo).HasColumnType("ntext");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateDeparture).HasMaxLength(500);

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.Exclude).HasColumnType("ntext");

                entity.Property(e => e.Image)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Include).HasColumnType("ntext");

                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDisplayWeb).HasDefaultValueSql("((0))");

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.Refund).HasColumnType("ntext");

                entity.Property(e => e.Schedule).HasColumnType("ntext");

                entity.Property(e => e.ServiceCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Surcharge).HasColumnType("ntext");

                entity.Property(e => e.TourName).HasMaxLength(200);

                entity.Property(e => e.Transportation)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<TourProgramPackages>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FromDate).HasColumnType("date");

                entity.Property(e => e.IsDaily).HasDefaultValueSql("((0))");

                entity.Property(e => e.ToDate).HasColumnType("date");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Transactions>(entity =>
            {
                entity.Property(e => e.BankReference)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.ContractNo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.TransactionNo)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.VerifyDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Avata).HasMaxLength(500);

                entity.Property(e => e.BirthDay).HasColumnType("datetime");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(500);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(2500);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ResetPassword)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserAgent>(entity =>
            {
                entity.HasKey(e => e.ClientId)
                    .HasName("PK_UserAgent_1");

                entity.Property(e => e.ClientId).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.MainFollow).HasComment("Quyền danh cho  0: Đối tác | 1: nhân viên của đối tác | 2: Saler phụ trách chính | 3: saler phụ trách cùng");

                entity.Property(e => e.UpdateLast).HasColumnType("datetime");

                entity.Property(e => e.VerifyDate).HasColumnType("datetime");

                entity.HasOne(d => d.Client)
                    .WithOne(p => p.UserAgent)
                    .HasForeignKey<UserAgent>(d => d.ClientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAgent_Client");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAgent)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAgent_User");
            });

            modelBuilder.Entity<UserDepart>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.JoinDate).HasColumnType("date");

                entity.Property(e => e.LeaveDate).HasColumnType("date");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserPosition>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRole_Role");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRole_User");
            });

            modelBuilder.Entity<VinWonderBooking>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasName("IDX_VinWonderBooking_OrderId");

                entity.Property(e => e.AdavigoBookingId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.ServiceCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.SiteCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SiteName).HasMaxLength(100);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<VinWonderBookingTicket>(entity =>
            {
                entity.Property(e => e.Adt).HasColumnName("adt");

                entity.Property(e => e.Child).HasColumnName("child");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateUsed).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Old).HasColumnName("old");

                entity.Property(e => e.RateCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<VinWonderBookingTicketCustomer>(entity =>
            {
                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(200);

                entity.Property(e => e.Genre).HasMaxLength(20);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.OtherDetail).HasColumnType("text");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<VinWonderBookingTicketDetail>(entity =>
            {
                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateFrom).HasColumnType("datetime");

                entity.Property(e => e.DateTo).HasColumnType("datetime");

                entity.Property(e => e.GateCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.GateName).HasMaxLength(50);

                entity.Property(e => e.GroupName).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.ServiceKey)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShortName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TypeCode).HasMaxLength(20);

                entity.Property(e => e.TypeName).HasMaxLength(50);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Vatpercent).HasColumnName("VATPercent");

                entity.Property(e => e.WeekDays)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VinWonderPricePolicy>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.ServiceCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ServiceId).HasComment("Khóa chính của các loại vé theo tên đặt bên Vinwonder");

                entity.Property(e => e.SiteName).HasMaxLength(200);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Campaign)
                    .WithMany(p => p.VinWonderPricePolicy)
                    .HasForeignKey(d => d.CampaignId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VinWonderPricePolicy_Campaign");
            });

            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.Property(e => e.CampaignId).HasColumnName("campaign_id");

                entity.Property(e => e.Cdate)
                    .HasColumnName("cdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnName("code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.EDate)
                    .HasColumnName("eDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.GroupUserPriority)
                    .HasColumnName("group_user_priority")
                    .IsUnicode(false)
                    .HasComment("Trường này để lưu nhóm những user được áp dụng trên voucher này");

                entity.Property(e => e.IsLimitVoucher).HasColumnName("is_limit_voucher");

                entity.Property(e => e.IsMaxPriceProduct).HasColumnName("is_max_price_product");

                entity.Property(e => e.IsPublic)
                    .HasColumnName("is_public")
                    .HasComment("Nêu set true thì hiểu voucher này được public cho các user thanh toán đơn hàng");

                entity.Property(e => e.LimitTotalDiscount).HasColumnName("limit_total_discount");

                entity.Property(e => e.LimitUse).HasColumnName("limitUse");

                entity.Property(e => e.MinTotalAmount).HasColumnName("min_total_amount");

                entity.Property(e => e.PriceSales)
                    .HasColumnName("price_sales")
                    .HasColumnType("money");

                entity.Property(e => e.ProjectType).HasColumnName("project_type");

                entity.Property(e => e.RuleType)
                    .HasColumnName("rule_type")
                    .HasComment("Trường này dùng để phân biệt voucher triển khai này chạy theo rule nào. Ví dụ: rule giảm giá với 1 số tiền vnđ trên toàn bộ đơn hàng. Giảm giá 20% phí first pound đầu tiên của nhãn hàng amazon. 1: triển khai rule giảm giá cho toàn bộ đơn hàng. 2 là rule áp dụng cho 20% phí first pound đầu tiên.");

                entity.Property(e => e.StoreApply)
                    .HasColumnName("store_apply")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Udate)
                    .HasColumnName("udate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Unit)
                    .HasColumnName("unit")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VoucherCampaign>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CampaignVoucher)
                    .HasColumnName("campaign_voucher")
                    .HasMaxLength(400);

                entity.Property(e => e.EndDate)
                    .HasColumnName("end_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.StartDate)
                    .HasColumnName("start_date")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<VoucherLogActivity>(entity =>
            {
                entity.ToTable("voucherLogActivity");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CartId).HasColumnName("cart_id");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("create_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.PriceSaleVnd).HasColumnName("price_sale_vnd");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasComment("Trang thai giao dịch voucher. 1: khoa. 0: dang ap dung");

                entity.Property(e => e.StoreId).HasColumnName("store_id");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.VoucherId).HasColumnName("voucher_id");

                entity.HasOne(d => d.Voucher)
                    .WithMany(p => p.VoucherLogActivity)
                    .HasForeignKey(d => d.VoucherId)
                    .HasConstraintName("FK_voucherLogActivity_Voucher");
            });

            modelBuilder.Entity<Ward>(entity =>
            {
                entity.Property(e => e.DistrictId)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.Property(e => e.Location).HasMaxLength(30);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.NameNonUnicode)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.WardId)
                    .IsRequired()
                    .HasMaxLength(5);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
