using Microsoft.EntityFrameworkCore;
using SiteReservationSystem.Web.Models;

namespace SiteReservationSystem.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets - Users & Authentication
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Admin> Admins { get; set; }

        // DbSets - Sites
        public DbSet<Site> Sites { get; set; }
        public DbSet<SiteType> SiteTypes { get; set; }
        public DbSet<SiteTypePricing> SiteTypePricings { get; set; }
        public DbSet<SitePhoto> SitePhotos { get; set; }

        // DbSets - Reservations
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationStatus> ReservationStatuses { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<ReservationFee> ReservationFees { get; set; }

        // DbSets - Billing & Payments
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User relationships - One-to-One
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Site relationships
            modelBuilder.Entity<Site>()
                .HasOne(s => s.SiteType)
                .WithMany(st => st.Sites)
                .HasForeignKey(s => s.SiteTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SitePhoto>()
                .HasOne(sp => sp.Site)
                .WithMany(s => s.SitePhotos)
                .HasForeignKey(sp => sp.SiteID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SiteTypePricing>()
                .HasOne(stp => stp.SiteType)
                .WithMany(st => st.SiteTypePricings)
                .HasForeignKey(stp => stp.SiteTypeID)
                .OnDelete(DeleteBehavior.Cascade);

            // Reservation relationships
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Reservations)
                .HasForeignKey(r => r.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Site)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.SiteID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.ReservationStatus)
                .WithMany(rs => rs.Reservations)
                .HasForeignKey(r => r.ReservationStatusID)
                .OnDelete(DeleteBehavior.Restrict);

            // ReservationFee relationships
            modelBuilder.Entity<ReservationFee>()
                .HasOne(rf => rf.Reservation)
                .WithMany(r => r.ReservationFees)
                .HasForeignKey(rf => rf.ReservationID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReservationFee>()
                .HasOne(rf => rf.Fee)
                .WithMany(f => f.ReservationFees)
                .HasForeignKey(rf => rf.FeeID)
                .OnDelete(DeleteBehavior.Restrict);

            // Invoice relationships
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Reservation)
                .WithOne(r => r.Invoice)
                .HasForeignKey<Invoice>(i => i.ReservationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment relationships
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.PaymentMethod)
                .WithMany(pm => pm.Payments)
                .HasForeignKey(p => p.PaymentMethodID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.ProcessedByEmployee)
                .WithMany(e => e.ProcessedPayments)
                .HasForeignKey(p => p.ProcessedByEmployeeID)
                .OnDelete(DeleteBehavior.SetNull);

            // Unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Site>()
                .HasIndex(s => s.SiteNumber)
                .IsUnique();

            // Seed Data - Reservation Statuses
            modelBuilder.Entity<ReservationStatus>().HasData(
                new ReservationStatus { ReservationStatusID = 1, StatusName = "Upcoming",    Description = "Reservation is scheduled for future dates" },
                new ReservationStatus { ReservationStatusID = 2, StatusName = "In Progress", Description = "Guest is currently checked in" },
                new ReservationStatus { ReservationStatusID = 3, StatusName = "Completed",   Description = "Reservation has been completed" },
                new ReservationStatus { ReservationStatusID = 4, StatusName = "Cancelled",   Description = "Reservation was cancelled" }
            );

            // Seed Data - Payment Methods
            modelBuilder.Entity<PaymentMethod>().HasData(
                new PaymentMethod { PaymentMethodID = 1, MethodName = "Credit Card", RequiresOnlineProcessing = true,  IsActive = true },
                new PaymentMethod { PaymentMethodID = 2, MethodName = "Debit Card",  RequiresOnlineProcessing = true,  IsActive = true },
                new PaymentMethod { PaymentMethodID = 3, MethodName = "Cash",        RequiresOnlineProcessing = false, IsActive = true },
                new PaymentMethod { PaymentMethodID = 4, MethodName = "Check",       RequiresOnlineProcessing = false, IsActive = true }
            );

            // Seed Data - Common Fees
            modelBuilder.Entity<Fee>().HasData(
                new Fee { FeeID = 1, FeeName = "Late Checkout",    DefaultAmount = 25.00m, Description = "Fee for checking out after designated time", IsActive = true },
                new Fee { FeeID = 2, FeeName = "Early Check-in",   DefaultAmount = 15.00m, Description = "Fee for checking in before standard time",   IsActive = true },
                new Fee { FeeID = 3, FeeName = "Cancellation Fee", DefaultAmount = 10.00m, Description = "Standard cancellation fee",                   IsActive = true }
            );

            // ============================================================================
            // SEED DATA - Team Admins
            // ============================================================================

            modelBuilder.Entity<User>().HasData(
                new User { UserID = 1, Email = "ashlyn.arave@example.com",        PasswordHash = "hashed_password_1", Role = UserRole.Admin, IsActive = true, DateCreated = new DateTime(2025, 1, 1) },
                new User { UserID = 2, Email = "kelise.bridge@example.com",       PasswordHash = "hashed_password_2", Role = UserRole.Admin, IsActive = true, DateCreated = new DateTime(2025, 1, 1) },
                new User { UserID = 3, Email = "zachary.chamberlain@example.com", PasswordHash = "hashed_password_3", Role = UserRole.Admin, IsActive = true, DateCreated = new DateTime(2025, 1, 1) },
                new User { UserID = 4, Email = "tyler.fleischel@example.com",     PasswordHash = "hashed_password_4", Role = UserRole.Admin, IsActive = true, DateCreated = new DateTime(2025, 1, 1) },
                new User { UserID = 5, Email = "nicole.gaddis@example.com",       PasswordHash = "hashed_password_5", Role = UserRole.Admin, IsActive = true, DateCreated = new DateTime(2025, 1, 1) },
                new User { UserID = 6, Email = "luke.peterson@example.com",       PasswordHash = "hashed_password_6", Role = UserRole.Admin, IsActive = true, DateCreated = new DateTime(2025, 1, 1) }
            );

            modelBuilder.Entity<Admin>().HasData(
                new Admin { AdminID = 1, UserID = 1, FirstName = "Ashlyn",  LastName = "Arave",       DateCreated = new DateTime(2025, 1, 1) },
                new Admin { AdminID = 2, UserID = 2, FirstName = "Kelise",  LastName = "Bridge",      DateCreated = new DateTime(2025, 1, 1) },
                new Admin { AdminID = 3, UserID = 3, FirstName = "Zachary", LastName = "Chamberlain", DateCreated = new DateTime(2025, 1, 1) },
                new Admin { AdminID = 4, UserID = 4, FirstName = "Tyler",   LastName = "Fleischel",   DateCreated = new DateTime(2025, 1, 1) },
                new Admin { AdminID = 5, UserID = 5, FirstName = "Nicole",  LastName = "Gaddis",      DateCreated = new DateTime(2025, 1, 1) },
                new Admin { AdminID = 6, UserID = 6, FirstName = "Luke",    LastName = "Peterson",    DateCreated = new DateTime(2025, 1, 1) }
            );

            // ============================================================================
            // SEED DATA - Employees
            // ============================================================================

            modelBuilder.Entity<User>().HasData(
                new User { UserID = 13, Email = "jane.doe@rvpark.com",    PasswordHash = "hashed_password_13", Role = UserRole.Employee, IsActive = true,  DateCreated = new DateTime(2025, 1, 15) },
                new User { UserID = 14, Email = "bob.smith@rvpark.com",   PasswordHash = "hashed_password_14", Role = UserRole.Employee, IsActive = true,  DateCreated = new DateTime(2025, 1, 15) },
                new User { UserID = 15, Email = "carol.white@rvpark.com", PasswordHash = "hashed_password_15", Role = UserRole.Employee, IsActive = true,  DateCreated = new DateTime(2025, 2, 1) },
                new User { UserID = 16, Email = "dan.locked@rvpark.com",  PasswordHash = "hashed_password_16", Role = UserRole.Employee, IsActive = false, DateCreated = new DateTime(2025, 1, 1) }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    EmployeeID = 1, UserID = 13, FirstName = "Jane", LastName = "Doe",
                    AccessPermissions = AccessPermissions.ManageSites | AccessPermissions.ManageReservations | AccessPermissions.ViewReports | AccessPermissions.ManageFees,
                    IsLockedOut = false, DateHired = new DateTime(2025, 1, 15), IsActive = true
                },
                new Employee
                {
                    EmployeeID = 2, UserID = 14, FirstName = "Bob", LastName = "Smith",
                    AccessPermissions = AccessPermissions.ManageReservations | AccessPermissions.ViewReports,
                    IsLockedOut = false, DateHired = new DateTime(2025, 1, 15), IsActive = true
                },
                new Employee
                {
                    EmployeeID = 3, UserID = 15, FirstName = "Carol", LastName = "White",
                    AccessPermissions = AccessPermissions.ViewReports,
                    IsLockedOut = false, DateHired = new DateTime(2025, 2, 1), IsActive = true
                },
                new Employee
                {
                    EmployeeID = 4, UserID = 16, FirstName = "Dan", LastName = "Locked",
                    AccessPermissions = AccessPermissions.ManageReservations,
                    IsLockedOut = true, DateHired = new DateTime(2025, 1, 1), IsActive = false
                }
            );

            // ============================================================================
            // SEED DATA - Site Types & Pricing
            // ============================================================================

            modelBuilder.Entity<SiteType>().HasData(
                new SiteType { SiteTypeID = 1, TypeName = "Trailers (1-45)",           Description = "Standard trailer sites for vehicles 1-45 feet", IsActive = true },
                new SiteType { SiteTypeID = 2, TypeName = "Walk-in Trailers (11B, 12B)", Description = "Walk-in trailer sites with linens provided",   IsActive = true },
                new SiteType { SiteTypeID = 3, TypeName = "Dry Storage",               Description = "Dry storage sites for RV parking",              IsActive = true },
                new SiteType { SiteTypeID = 4, TypeName = "Tent Site",                 Description = "Tent camping site near Dog Park",                IsActive = true }
            );

            modelBuilder.Entity<SiteTypePricing>().HasData(
                new SiteTypePricing { PricingID = 1, SiteTypeID = 1, StartDate = new DateTime(2024, 10, 1), EndDate = null, BasePrice = 25.00m, Description = "Current rate for standard trailers" },
                new SiteTypePricing { PricingID = 2, SiteTypeID = 2, StartDate = new DateTime(2024, 10, 1), EndDate = null, BasePrice = 30.00m, Description = "Current rate for walk-in trailers" },
                new SiteTypePricing { PricingID = 3, SiteTypeID = 3, StartDate = new DateTime(2024, 10, 1), EndDate = null, BasePrice = 5.00m,  Description = "Current daily rate for dry storage (also $30/week, $36/month)" },
                new SiteTypePricing { PricingID = 4, SiteTypeID = 4, StartDate = new DateTime(2024, 10, 1), EndDate = null, BasePrice = 17.00m, Description = "Current rate for tent site" }
            );

            // ============================================================================
            // SEED DATA - Sites
            // ============================================================================

            modelBuilder.Entity<Site>().HasData(
                new Site { SiteID = 1,  SiteNumber = "2",      SiteTypeID = 1, MaxLengthFeet = 40, IsActive = true },
                new Site { SiteID = 2,  SiteNumber = "3",      SiteTypeID = 1, MaxLengthFeet = 40, IsActive = true },
                new Site { SiteID = 3,  SiteNumber = "4",      SiteTypeID = 1, MaxLengthFeet = 40, IsActive = true },
                new Site { SiteID = 4,  SiteNumber = "5",      SiteTypeID = 1, MaxLengthFeet = 40, IsActive = true },
                new Site { SiteID = 5,  SiteNumber = "6",      SiteTypeID = 1, MaxLengthFeet = 40, IsActive = true },
                new Site { SiteID = 6,  SiteNumber = "7",      SiteTypeID = 1, MaxLengthFeet = 40, IsActive = true },
                new Site { SiteID = 7,  SiteNumber = "8",      SiteTypeID = 1, MaxLengthFeet = 40, IsActive = true },
                new Site { SiteID = 8,  SiteNumber = "9",      SiteTypeID = 1, MaxLengthFeet = 40, IsActive = true },
                new Site { SiteID = 9,  SiteNumber = "10",     SiteTypeID = 1, MaxLengthFeet = 40, IsActive = true },
                new Site { SiteID = 10, SiteNumber = "11",     SiteTypeID = 1, MaxLengthFeet = 40, IsActive = true },
                new Site { SiteID = 11, SiteNumber = "12",     SiteTypeID = 1, MaxLengthFeet = 40, IsActive = true },
                new Site { SiteID = 12, SiteNumber = "13",     SiteTypeID = 1, MaxLengthFeet = 40, IsActive = true },
                new Site { SiteID = 13, SiteNumber = "14",     SiteTypeID = 1, MaxLengthFeet = 40, IsActive = true },
                new Site { SiteID = 14, SiteNumber = "17",     SiteTypeID = 1, MaxLengthFeet = 45, IsActive = true },
                new Site { SiteID = 15, SiteNumber = "18",     SiteTypeID = 1, MaxLengthFeet = 45, IsActive = true },
                new Site { SiteID = 16, SiteNumber = "20",     SiteTypeID = 1, MaxLengthFeet = 45, IsActive = true },
                new Site { SiteID = 17, SiteNumber = "22",     SiteTypeID = 1, MaxLengthFeet = 45, IsActive = true },
                new Site { SiteID = 18, SiteNumber = "23",     SiteTypeID = 1, MaxLengthFeet = 45, IsActive = true },
                new Site { SiteID = 19, SiteNumber = "24",     SiteTypeID = 1, MaxLengthFeet = 45, IsActive = true },
                new Site { SiteID = 20, SiteNumber = "25",     SiteTypeID = 1, MaxLengthFeet = 45, IsActive = true },
                new Site { SiteID = 21, SiteNumber = "26",     SiteTypeID = 1, MaxLengthFeet = 45, IsActive = true },
                new Site { SiteID = 22, SiteNumber = "27",     SiteTypeID = 1, MaxLengthFeet = 45, IsActive = true },
                new Site { SiteID = 23, SiteNumber = "28",     SiteTypeID = 1, MaxLengthFeet = 45, IsActive = true },
                new Site { SiteID = 24, SiteNumber = "29",     SiteTypeID = 1, MaxLengthFeet = 45, IsActive = true },
                new Site { SiteID = 25, SiteNumber = "30",     SiteTypeID = 1, MaxLengthFeet = 45, IsActive = true },
                new Site { SiteID = 26, SiteNumber = "31",     SiteTypeID = 1, MaxLengthFeet = 45, IsActive = true },
                new Site { SiteID = 27, SiteNumber = "32",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                new Site { SiteID = 28, SiteNumber = "33",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                new Site { SiteID = 29, SiteNumber = "34",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                new Site { SiteID = 30, SiteNumber = "35",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                new Site { SiteID = 31, SiteNumber = "36",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                new Site { SiteID = 32, SiteNumber = "37",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                new Site { SiteID = 33, SiteNumber = "38",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                new Site { SiteID = 34, SiteNumber = "39",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                new Site { SiteID = 35, SiteNumber = "40",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                new Site { SiteID = 36, SiteNumber = "41",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                new Site { SiteID = 37, SiteNumber = "42",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                new Site { SiteID = 38, SiteNumber = "43",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                new Site { SiteID = 39, SiteNumber = "44",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                new Site { SiteID = 40, SiteNumber = "45",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                new Site { SiteID = 41, SiteNumber = "1",      SiteTypeID = 1, MaxLengthFeet = 55, Notes = "Exception - 55 feet", IsActive = true },
                new Site { SiteID = 42, SiteNumber = "19",     SiteTypeID = 1, MaxLengthFeet = 55, Notes = "Exception - 55 feet", IsActive = true },
                new Site { SiteID = 43, SiteNumber = "21",     SiteTypeID = 1, MaxLengthFeet = 55, Notes = "Exception - 55 feet", IsActive = true },
                new Site { SiteID = 44, SiteNumber = "11B",    SiteTypeID = 2, MaxLengthFeet = 30, Notes = "Walk-in trailer with linens", IsActive = true },
                new Site { SiteID = 45, SiteNumber = "12B",    SiteTypeID = 2, MaxLengthFeet = 30, Notes = "Walk-in trailer with linens", IsActive = true },
                new Site { SiteID = 46, SiteNumber = "A",      SiteTypeID = 3, MaxLengthFeet = 65, Notes = "Dry storage site", IsActive = true },
                new Site { SiteID = 47, SiteNumber = "B",      SiteTypeID = 3, MaxLengthFeet = 65, Notes = "Dry storage site", IsActive = true },
                new Site { SiteID = 48, SiteNumber = "C",      SiteTypeID = 3, MaxLengthFeet = 65, Notes = "Dry storage site", IsActive = true },
                new Site { SiteID = 49, SiteNumber = "D",      SiteTypeID = 3, MaxLengthFeet = 65, Notes = "Dry storage site", IsActive = true },
                new Site { SiteID = 50, SiteNumber = "TENT-1", SiteTypeID = 4, MaxLengthFeet = 0,  Notes = "Tent site near Dog Park", IsActive = true }
            );

            // ============================================================================
            // SEED DATA - Site Photos
            // ============================================================================

            modelBuilder.Entity<SitePhoto>().HasData(
                new SitePhoto { PhotoID = 1,  SiteID = 1,  PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0694.jpg?w=4032&ssl=1",            Caption = "Site 2 - Overview",          DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 2,  SiteID = 1,  PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/09/IMG_1864.jpg?w=4032&ssl=1",            Caption = "Site 2 - Hookup View",       DisplayOrder = 2, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 3,  SiteID = 2,  PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/09/IMG_1843.jpg?w=4032&ssl=1",            Caption = "Site 3",                     DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 4,  SiteID = 3,  PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/09/IMG_1844.jpg?w=4032&ssl=1",            Caption = "Site 4",                     DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 5,  SiteID = 4,  PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/10/IMG_2113.jpg?w=4032&ssl=1",            Caption = "Site 5",                     DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 6,  SiteID = 5,  PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0401.jpg?w=4032&ssl=1",            Caption = "Site 6",                     DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 7,  SiteID = 6,  PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0710.jpg?w=4032&ssl=1",            Caption = "Site 7",                     DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 8,  SiteID = 7,  PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/09/IMG_1855-1.jpg?w=4032&ssl=1",          Caption = "Site 8",                     DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 9,  SiteID = 8,  PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1345.jpg?w=4032&ssl=1",            Caption = "Site 9",                     DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 10, SiteID = 9,  PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1054.jpg?w=4032&ssl=1",            Caption = "Site 10",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 11, SiteID = 10, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1039.jpg?w=4032&ssl=1",            Caption = "Site 11",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 12, SiteID = 11, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/09/IMG_1860.jpg?w=4032&ssl=1",            Caption = "Site 12",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 13, SiteID = 12, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1141.jpg?w=4032&ssl=1",            Caption = "Site 13",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 14, SiteID = 13, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0419.jpg?w=4032&ssl=1",            Caption = "Site 14",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 15, SiteID = 14, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0954.jpg?w=4032&ssl=1",            Caption = "Site 17",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 16, SiteID = 15, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0512.jpg?w=4032&ssl=1",            Caption = "Site 18",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 17, SiteID = 16, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0596.jpg?w=4032&ssl=1",            Caption = "Site 20",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 18, SiteID = 17, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0659.jpg?w=4032&ssl=1",            Caption = "Site 22",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 19, SiteID = 18, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0408.jpg?w=4032&ssl=1",            Caption = "Site 23",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 20, SiteID = 19, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1321.jpg?w=4032&ssl=1",            Caption = "Site 24",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 21, SiteID = 20, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0409.jpg?w=4032&ssl=1",            Caption = "Site 25",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 22, SiteID = 21, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0403.jpg?w=4032&ssl=1",            Caption = "Site 26",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 23, SiteID = 22, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1223-1.jpg?w=4032&ssl=1",          Caption = "Site 27",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 24, SiteID = 23, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0407.jpg?w=4032&ssl=1",            Caption = "Site 28",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 25, SiteID = 24, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1164.jpg?w=4032&ssl=1",            Caption = "Site 29",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 26, SiteID = 25, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/04/IMG_20190422_100028.jpg?w=4032&ssl=1", Caption = "Site 30",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 27, SiteID = 26, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1065.jpg?w=4032&ssl=1",            Caption = "Site 31",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 28, SiteID = 27, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1226.jpg?w=4032&ssl=1",            Caption = "Site 32",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 29, SiteID = 28, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0982.jpg?w=4032&ssl=1",            Caption = "Site 33",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 30, SiteID = 29, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0988.jpg?w=4032&ssl=1",            Caption = "Site 34",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 31, SiteID = 30, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1173.jpg?w=4032&ssl=1",            Caption = "Site 35",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 32, SiteID = 31, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1269.jpg?w=4032&ssl=1",            Caption = "Site 36",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 33, SiteID = 32, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0640.jpg?w=4032&ssl=1",            Caption = "Site 37",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 34, SiteID = 33, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1199.jpg?w=4032&ssl=1",            Caption = "Site 38",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 35, SiteID = 34, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1377.jpg?w=4032&ssl=1",            Caption = "Site 39",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 36, SiteID = 35, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0617.jpg?w=4032&ssl=1",            Caption = "Site 40",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 37, SiteID = 36, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1004.jpg?w=4032&ssl=1",            Caption = "Site 41",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 38, SiteID = 37, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1315.jpg?w=4032&ssl=1",            Caption = "Site 42",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 39, SiteID = 38, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0390.jpg?w=4032&ssl=1",            Caption = "Site 43",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 40, SiteID = 39, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0612.jpg?w=4032&ssl=1",            Caption = "Site 44",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 41, SiteID = 40, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_20190430_100259.jpg?w=4032&ssl=1", Caption = "Site 45",                    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 42, SiteID = 41, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1304.jpg?w=4032&ssl=1",            Caption = "Site 1 (55ft exception)",    DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 43, SiteID = 42, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_20190430_100506.jpg?w=4032&ssl=1", Caption = "Site 19 (55ft exception)",   DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 44, SiteID = 43, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_1019.jpg?w=4032&ssl=1",            Caption = "Site 21 (55ft exception)",   DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 45, SiteID = 44, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1309.jpg?w=4032&ssl=1",            Caption = "Site 11B - Walk-in Trailer", DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 46, SiteID = 45, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0439.jpg?w=4032&ssl=1",            Caption = "Site 12B - Walk-in Trailer", DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 47, SiteID = 46, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0603.jpg?w=4032&ssl=1",            Caption = "Dry Storage A",              DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 48, SiteID = 47, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0444.jpg?w=4032&ssl=1",            Caption = "Dry Storage B",              DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 49, SiteID = 48, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0976.jpg?w=4032&ssl=1",            Caption = "Dry Storage C",              DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 50, SiteID = 49, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0413.jpg?w=4032&ssl=1",            Caption = "Dry Storage D",              DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) },
                new SitePhoto { PhotoID = 51, SiteID = 50, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0515.jpg?w=4032&ssl=1",            Caption = "Tent Site - Near Dog Park",  DisplayOrder = 1, DateUploaded = new DateTime(2025, 1, 1) }
            );

            // ============================================================================
            // SEED DATA - Guest Customers
            // ============================================================================

            modelBuilder.Entity<User>().HasData(
                new User { UserID = 7,  Email = "john.smith@military.com",     PasswordHash = "hashed_password_7",  Role = UserRole.Customer, IsActive = true, DateCreated = new DateTime(2025, 1, 1) },
                new User { UserID = 8,  Email = "sarah.johnson@military.com",  PasswordHash = "hashed_password_8",  Role = UserRole.Customer, IsActive = true, DateCreated = new DateTime(2024, 12, 1) },
                new User { UserID = 9,  Email = "mike.williams@military.com",  PasswordHash = "hashed_password_9",  Role = UserRole.Customer, IsActive = true, DateCreated = new DateTime(2025, 2, 1) },
                new User { UserID = 10, Email = "emily.davis@military.com",    PasswordHash = "hashed_password_10", Role = UserRole.Customer, IsActive = true, DateCreated = new DateTime(2025, 2, 15) },
                new User { UserID = 11, Email = "david.martinez@military.com", PasswordHash = "hashed_password_11", Role = UserRole.Customer, IsActive = true, DateCreated = new DateTime(2024, 11, 1) },
                new User { UserID = 12, Email = "lisa.anderson@military.com",  PasswordHash = "hashed_password_12", Role = UserRole.Customer, IsActive = true, DateCreated = new DateTime(2025, 2, 19) }
            );

            // DoDStatus note: Mike Williams = PCS_ORDERS, which exempts him from the
            // 14-day peak season limit (see Reservation 3 - 14-night stay during peak).
            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerID = 1, UserID = 7,  FirstName = "John",  LastName = "Smith",    PhoneNumber = "555-0101", MilitaryAffiliation = MilitaryAffiliation.AIR_FORCE,    DoDStatus = DoDStatus.ACTIVE_DUTY, DateCreated = new DateTime(2025, 1, 1) },
                new Customer { CustomerID = 2, UserID = 8,  FirstName = "Sarah", LastName = "Johnson",  PhoneNumber = "555-0102", MilitaryAffiliation = MilitaryAffiliation.ARMY,         DoDStatus = DoDStatus.RETIRED,     DateCreated = new DateTime(2024, 12, 1) },
                new Customer { CustomerID = 3, UserID = 9,  FirstName = "Mike",  LastName = "Williams", PhoneNumber = "555-0103", MilitaryAffiliation = MilitaryAffiliation.NAVY,         DoDStatus = DoDStatus.PCS_ORDERS,  DateCreated = new DateTime(2025, 2, 1) },
                new Customer { CustomerID = 4, UserID = 10, FirstName = "Emily", LastName = "Davis",    PhoneNumber = "555-0104", MilitaryAffiliation = MilitaryAffiliation.MARINES,      DoDStatus = DoDStatus.ACTIVE_DUTY, DateCreated = new DateTime(2025, 2, 15) },
                new Customer { CustomerID = 5, UserID = 11, FirstName = "David", LastName = "Martinez", PhoneNumber = "555-0105", MilitaryAffiliation = MilitaryAffiliation.COAST_GUARD,  DoDStatus = DoDStatus.RESERVIST,   DateCreated = new DateTime(2024, 11, 1) },
                new Customer { CustomerID = 6, UserID = 12, FirstName = "Lisa",  LastName = "Anderson", PhoneNumber = "555-0106", MilitaryAffiliation = MilitaryAffiliation.DOD_CIVILIAN, DoDStatus = DoDStatus.ACTIVE_DUTY, DateCreated = new DateTime(2025, 2, 19) }
            );

            // ============================================================================
            // SEED DATA - Reservations (Anchor: 2025-03-01)
            // ============================================================================

            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    ReservationID = 1, CustomerID = 1, SiteID = 14, ReservationStatusID = 3,
                    StartDate = new DateTime(2025, 2, 19), EndDate = new DateTime(2025, 2, 22),
                    TrailerLengthFeet = 42, BaseAmount = 75.00m, TotalAmount = 75.00m, BalanceDue = 0m,
                    ScheduledCheckInTime  = new DateTime(2025, 2, 19, 13, 0, 0),
                    ActualCheckInTime     = new DateTime(2025, 2, 19, 13, 15, 0),
                    ScheduledCheckOutTime = new DateTime(2025, 2, 22, 12, 0, 0),
                    ActualCheckOutTime    = new DateTime(2025, 2, 22, 11, 45, 0),
                    DateCreated = new DateTime(2025, 2, 14), LastUpdated = new DateTime(2025, 2, 22)
                },
                new Reservation
                {
                    ReservationID = 2, CustomerID = 2, SiteID = 27, ReservationStatusID = 2,
                    StartDate = new DateTime(2025, 2, 27), EndDate = new DateTime(2025, 3, 8),
                    TrailerLengthFeet = 60, BaseAmount = 175.00m, TotalAmount = 200.00m, BalanceDue = 0m,
                    ScheduledCheckInTime  = new DateTime(2025, 2, 27, 13, 0, 0),
                    ActualCheckInTime     = new DateTime(2025, 2, 27, 14, 30, 0),
                    ScheduledCheckOutTime = new DateTime(2025, 3, 8, 12, 0, 0),
                    ActualCheckOutTime = null,
                    DateCreated = new DateTime(2025, 2, 19), LastUpdated = new DateTime(2025, 2, 27)
                },
                // 14-night stay - valid because CustomerID 3 has DoDStatus = PCS_ORDERS
                new Reservation
                {
                    ReservationID = 3, CustomerID = 3, SiteID = 20, ReservationStatusID = 1,
                    StartDate = new DateTime(2025, 3, 8), EndDate = new DateTime(2025, 3, 22),
                    TrailerLengthFeet = 38, BaseAmount = 350.00m, TotalAmount = 350.00m, BalanceDue = 0m,
                    ScheduledCheckInTime  = new DateTime(2025, 3, 8, 13, 0, 0),
                    ActualCheckInTime = null,
                    ScheduledCheckOutTime = new DateTime(2025, 3, 22, 12, 0, 0),
                    ActualCheckOutTime = null,
                    DateCreated = new DateTime(2025, 2, 24), LastUpdated = new DateTime(2025, 2, 24)
                },
                new Reservation
                {
                    ReservationID = 4, CustomerID = 4, SiteID = 5, ReservationStatusID = 4,
                    StartDate = new DateTime(2025, 3, 11), EndDate = new DateTime(2025, 3, 15),
                    TrailerLengthFeet = 35, BaseAmount = 100.00m, TotalAmount = 110.00m, BalanceDue = 0m,
                    ScheduledCheckInTime  = new DateTime(2025, 3, 11, 13, 0, 0),
                    ActualCheckInTime = null,
                    ScheduledCheckOutTime = new DateTime(2025, 3, 15, 12, 0, 0),
                    ActualCheckOutTime = null,
                    DateCreated = new DateTime(2025, 2, 9), LastUpdated = new DateTime(2025, 2, 26),
                    Notes = "Cancelled due to change in travel plans"
                },
                new Reservation
                {
                    ReservationID = 5, CustomerID = 5, SiteID = 44, ReservationStatusID = 1,
                    StartDate = new DateTime(2025, 3, 4), EndDate = new DateTime(2025, 3, 7),
                    TrailerLengthFeet = null, BaseAmount = 90.00m, TotalAmount = 105.00m, BalanceDue = 0m,
                    ScheduledCheckInTime  = new DateTime(2025, 3, 4, 8, 0, 0),
                    ActualCheckInTime = null,
                    ScheduledCheckOutTime = new DateTime(2025, 3, 7, 12, 0, 0),
                    ActualCheckOutTime = null,
                    DateCreated = new DateTime(2025, 2, 22), LastUpdated = new DateTime(2025, 2, 22)
                },
                new Reservation
                {
                    ReservationID = 6, CustomerID = 6, SiteID = 50, ReservationStatusID = 1,
                    StartDate = new DateTime(2025, 3, 2), EndDate = new DateTime(2025, 3, 4),
                    TrailerLengthFeet = null, BaseAmount = 34.00m, TotalAmount = 34.00m, BalanceDue = 0m,
                    ScheduledCheckInTime  = new DateTime(2025, 3, 2, 13, 0, 0),
                    ActualCheckInTime = null,
                    ScheduledCheckOutTime = new DateTime(2025, 3, 4, 12, 0, 0),
                    ActualCheckOutTime = null,
                    DateCreated = new DateTime(2025, 2, 27), LastUpdated = new DateTime(2025, 2, 27)
                }
            );

            // ============================================================================
            // SEED DATA - Reservation Fees
            // ============================================================================

            modelBuilder.Entity<ReservationFee>().HasData(
                new ReservationFee { ReservationFeeID = 1, ReservationID = 2, FeeID = 1, Amount = 25.00m, DateApplied = new DateTime(2025, 2, 27), Notes = "Requested late checkout until 3:00 PM" },
                new ReservationFee { ReservationFeeID = 2, ReservationID = 4, FeeID = 3, Amount = 10.00m, DateApplied = new DateTime(2025, 2, 26), Notes = "Cancelled more than 72 hours before arrival" },
                new ReservationFee { ReservationFeeID = 3, ReservationID = 5, FeeID = 2, Amount = 15.00m, DateApplied = new DateTime(2025, 2, 22), Notes = "Early check-in requested for 8:00 AM" }
            );

            // ============================================================================
            // SEED DATA - Invoices & Payments
            // ============================================================================

            modelBuilder.Entity<Invoice>().HasData(
                new Invoice { InvoiceID = 1, ReservationID = 1, CustomerID = 1, SubTotal = 75.00m,  TotalFees = 0m,     TotalAmount = 75.00m,  InvoiceDate = new DateTime(2025, 2, 14), DueDate = new DateTime(2025, 2, 19), IsPaid = true, DatePaid = new DateTime(2025, 2, 14) },
                new Invoice { InvoiceID = 2, ReservationID = 2, CustomerID = 2, SubTotal = 175.00m, TotalFees = 25.00m, TotalAmount = 200.00m, InvoiceDate = new DateTime(2025, 2, 19), DueDate = new DateTime(2025, 2, 27), IsPaid = true, DatePaid = new DateTime(2025, 2, 19) },
                new Invoice { InvoiceID = 3, ReservationID = 3, CustomerID = 3, SubTotal = 350.00m, TotalFees = 0m,     TotalAmount = 350.00m, InvoiceDate = new DateTime(2025, 2, 24), DueDate = new DateTime(2025, 3, 8),  IsPaid = true, DatePaid = new DateTime(2025, 2, 24) },
                new Invoice { InvoiceID = 4, ReservationID = 4, CustomerID = 4, SubTotal = 100.00m, TotalFees = 10.00m, TotalAmount = 110.00m, InvoiceDate = new DateTime(2025, 2, 9),  DueDate = new DateTime(2025, 3, 11), IsPaid = true, DatePaid = new DateTime(2025, 2, 9) },
                new Invoice { InvoiceID = 5, ReservationID = 5, CustomerID = 5, SubTotal = 90.00m,  TotalFees = 15.00m, TotalAmount = 105.00m, InvoiceDate = new DateTime(2025, 2, 22), DueDate = new DateTime(2025, 3, 4),  IsPaid = true, DatePaid = new DateTime(2025, 2, 22) },
                new Invoice { InvoiceID = 6, ReservationID = 6, CustomerID = 6, SubTotal = 34.00m,  TotalFees = 0m,     TotalAmount = 34.00m,  InvoiceDate = new DateTime(2025, 2, 27), DueDate = new DateTime(2025, 3, 2),  IsPaid = true, DatePaid = new DateTime(2025, 2, 27) }
            );

            modelBuilder.Entity<Payment>().HasData(
                new Payment { PaymentID = 1, InvoiceID = 1, PaymentMethodID = 1, Amount = 75.00m,   PaymentDate = new DateTime(2025, 2, 14), StripeTransactionID = "txn_1ABC123",        PaymentStatus = "Completed", IsRefund = false },
                new Payment { PaymentID = 2, InvoiceID = 2, PaymentMethodID = 2, Amount = 200.00m,  PaymentDate = new DateTime(2025, 2, 19), StripeTransactionID = "txn_2DEF456",        PaymentStatus = "Completed", IsRefund = false },
                new Payment { PaymentID = 3, InvoiceID = 3, PaymentMethodID = 1, Amount = 350.00m,  PaymentDate = new DateTime(2025, 2, 24), StripeTransactionID = "txn_3GHI789",        PaymentStatus = "Completed", IsRefund = false },
                new Payment { PaymentID = 4, InvoiceID = 4, PaymentMethodID = 1, Amount = 110.00m,  PaymentDate = new DateTime(2025, 2, 9),  StripeTransactionID = "txn_4JKL012",        PaymentStatus = "Completed", IsRefund = false },
                new Payment { PaymentID = 5, InvoiceID = 4, PaymentMethodID = 1, Amount = -100.00m, PaymentDate = new DateTime(2025, 2, 26), StripeTransactionID = "txn_4JKL012_refund", PaymentStatus = "Refunded",  IsRefund = true,  Notes = "Refund issued: $100.00 (Original $110.00 - $10.00 cancellation fee)" },
                new Payment { PaymentID = 6, InvoiceID = 5, PaymentMethodID = 3, Amount = 105.00m,  PaymentDate = new DateTime(2025, 2, 22), PaymentStatus = "Completed", ProcessedByEmployeeID = null, IsRefund = false },
                new Payment { PaymentID = 7, InvoiceID = 6, PaymentMethodID = 2, Amount = 34.00m,   PaymentDate = new DateTime(2025, 2, 27), StripeTransactionID = "txn_6MNO345",        PaymentStatus = "Completed", IsRefund = false }
            );
        }
    }
}
