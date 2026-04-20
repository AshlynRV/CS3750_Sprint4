using Microsoft.EntityFrameworkCore;
using SiteReservationSystem.Web.Models;

namespace SiteReservationSystem.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

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
            modelBuilder
                .Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Admin>()
                .HasOne(a => a.User)
                .WithOne(u => u.Admin)
                .HasForeignKey<Admin>(a => a.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Site relationships
            modelBuilder
                .Entity<Site>()
                .HasOne(s => s.SiteType)
                .WithMany(st => st.Sites)
                .HasForeignKey(s => s.SiteTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<SitePhoto>()
                .HasOne(sp => sp.Site)
                .WithMany(s => s.SitePhotos)
                .HasForeignKey(sp => sp.SiteID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<SiteTypePricing>()
                .HasOne(stp => stp.SiteType)
                .WithMany(st => st.SiteTypePricings)
                .HasForeignKey(stp => stp.SiteTypeID)
                .OnDelete(DeleteBehavior.Cascade);

            // Reservation relationships
            modelBuilder
                .Entity<Reservation>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Reservations)
                .HasForeignKey(r => r.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Reservation>()
                .HasOne(r => r.Site)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.SiteID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Reservation>()
                .HasOne(r => r.ReservationStatus)
                .WithMany(rs => rs.Reservations)
                .HasForeignKey(r => r.ReservationStatusID)
                .OnDelete(DeleteBehavior.Restrict);

            // ReservationFee relationships
            modelBuilder
                .Entity<ReservationFee>()
                .HasOne(rf => rf.Reservation)
                .WithMany(r => r.ReservationFees)
                .HasForeignKey(rf => rf.ReservationID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<ReservationFee>()
                .HasOne(rf => rf.Fee)
                .WithMany(f => f.ReservationFees)
                .HasForeignKey(rf => rf.FeeID)
                .OnDelete(DeleteBehavior.Restrict);

            // Invoice relationships
            modelBuilder
                .Entity<Invoice>()
                .HasOne(i => i.Reservation)
                .WithOne(r => r.Invoice)
                .HasForeignKey<Invoice>(i => i.ReservationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Invoice>()
                .HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment relationships
            modelBuilder
                .Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Payment>()
                .HasOne(p => p.PaymentMethod)
                .WithMany(pm => pm.Payments)
                .HasForeignKey(p => p.PaymentMethodID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Payment>()
                .HasOne(p => p.ProcessedByEmployee)
                .WithMany(e => e.ProcessedPayments)
                .HasForeignKey(p => p.ProcessedByEmployeeID)
                .OnDelete(DeleteBehavior.SetNull);

            // Unique constraints
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Site>().HasIndex(s => s.SiteNumber).IsUnique();

            // ============================================================================
            // SEED DATA - Reservation Statuses
            // ============================================================================

            modelBuilder
                .Entity<ReservationStatus>()
                .HasData(
                    new ReservationStatus { ReservationStatusID = 1, StatusName = "Upcoming",    Description = "Reservation is scheduled for future dates" },
                    new ReservationStatus { ReservationStatusID = 2, StatusName = "In Progress", Description = "Guest is currently checked in" },
                    new ReservationStatus { ReservationStatusID = 3, StatusName = "Completed",   Description = "Reservation has been completed" },
                    new ReservationStatus { ReservationStatusID = 4, StatusName = "Cancelled",   Description = "Reservation was cancelled" }
                );

            // ============================================================================
            // SEED DATA - Payment Methods
            // ============================================================================

            modelBuilder
                .Entity<PaymentMethod>()
                .HasData(
                    new PaymentMethod { PaymentMethodID = 1, MethodName = "Credit Card", RequiresOnlineProcessing = true,  IsActive = true },
                    new PaymentMethod { PaymentMethodID = 2, MethodName = "Debit Card",  RequiresOnlineProcessing = true,  IsActive = true },
                    new PaymentMethod { PaymentMethodID = 3, MethodName = "Cash",        RequiresOnlineProcessing = false, IsActive = true },
                    new PaymentMethod { PaymentMethodID = 4, MethodName = "Check",       RequiresOnlineProcessing = false, IsActive = true }
                );

            // ============================================================================
            // SEED DATA - Common Fees
            // ============================================================================

            modelBuilder
                .Entity<Fee>()
                .HasData(
                    new Fee { FeeID = 1, FeeName = "Late Checkout",    DefaultAmount = 25.00m, Description = "Fee for checking out after designated time", IsActive = true },
                    new Fee { FeeID = 2, FeeName = "Early Check-in",   DefaultAmount = 15.00m, Description = "Fee for checking in before standard time",   IsActive = true },
                    new Fee { FeeID = 3, FeeName = "Cancellation Fee", DefaultAmount = 10.00m, Description = "Standard cancellation fee",                   IsActive = true }
                );

            // ============================================================================
            // SEED DATA - Demo Admin
            // ============================================================================

            modelBuilder
                .Entity<User>()
                .HasData(
                    new User
                    {
                        UserID = 1,
                        Email = "admin@rvpark.com",
                        PasswordHash = "Password123!",
                        Role = UserRole.Admin,
                        IsActive = true,
                        DateCreated = new DateTime(2026, 1, 1),
                    }
                );

            modelBuilder
                .Entity<Admin>()
                .HasData(
                    new Admin
                    {
                        AdminID = 1,
                        UserID = 1,
                        FirstName = "Demo",
                        LastName = "Admin",
                        DateCreated = new DateTime(2026, 1, 1),
                    }
                );

            // ============================================================================
            // SEED DATA - Demo Employee (Jane Doe)
            // ============================================================================

            modelBuilder
                .Entity<User>()
                .HasData(
                    new User
                    {
                        UserID = 2,
                        Email = "jane.doe@rvpark.com",
                        PasswordHash = "Password123!",
                        Role = UserRole.Employee,
                        IsActive = true,
                        DateCreated = new DateTime(2026, 1, 15),
                    }
                );

            modelBuilder
                .Entity<Employee>()
                .HasData(
                    new Employee
                    {
                        EmployeeID = 1,
                        UserID = 2,
                        FirstName = "Jane",
                        LastName = "Doe",
                        AccessPermissions =
                            AccessPermissions.ManageReservations | AccessPermissions.ViewReports,
                        IsLockedOut = false,
                        DateHired = new DateTime(2026, 1, 15),
                        IsActive = true,
                    }
                );

            // ============================================================================
            // SEED DATA - Demo Customer (John Smith - ACTIVE_DUTY, NOT PCS_ORDERS)
            // ============================================================================

            modelBuilder
                .Entity<User>()
                .HasData(
                    new User
                    {
                        UserID = 3,
                        Email = "john.smith@military.com",
                        PasswordHash = "Password123!",
                        Role = UserRole.Customer,
                        IsActive = true,
                        DateCreated = new DateTime(2026, 1, 1),
                    }
                );

            modelBuilder
                .Entity<Customer>()
                .HasData(
                    new Customer
                    {
                        CustomerID = 1,
                        UserID = 3,
                        FirstName = "John",
                        LastName = "Smith",
                        PhoneNumber = "555-123-4567",
                        MilitaryAffiliation = MilitaryAffiliation.AIR_FORCE,
                        DoDStatus = DoDStatus.ACTIVE_DUTY,
                        DateCreated = new DateTime(2026, 1, 1),
                    }
                );

            // ============================================================================
            // SEED DATA - Site Types & Pricing
            // ============================================================================

            modelBuilder
                .Entity<SiteType>()
                .HasData(
                    new SiteType { SiteTypeID = 1, TypeName = "Trailers (1-45)",             Description = "Standard trailer sites for vehicles 1-45 feet", IsActive = true },
                    new SiteType { SiteTypeID = 2, TypeName = "Walk-in Trailers (11B, 12B)", Description = "Walk-in trailer sites with linens provided",     IsActive = true },
                    new SiteType { SiteTypeID = 3, TypeName = "Dry Storage",                 Description = "Dry storage sites for RV parking",              IsActive = true },
                    new SiteType { SiteTypeID = 4, TypeName = "Tent Site",                   Description = "Tent camping site near Dog Park",                IsActive = true }
                );

            modelBuilder
                .Entity<SiteTypePricing>()
                .HasData(
                    new SiteTypePricing { PricingID = 1, SiteTypeID = 1, StartDate = new DateTime(2025, 10, 1), EndDate = null, BasePrice = 25.00m, Description = "Current rate for standard trailers" },
                    new SiteTypePricing { PricingID = 2, SiteTypeID = 2, StartDate = new DateTime(2025, 10, 1), EndDate = null, BasePrice = 30.00m, Description = "Current rate for walk-in trailers" },
                    new SiteTypePricing { PricingID = 3, SiteTypeID = 3, StartDate = new DateTime(2025, 10, 1), EndDate = null, BasePrice = 5.00m,  Description = "Current daily rate for dry storage" },
                    new SiteTypePricing { PricingID = 4, SiteTypeID = 4, StartDate = new DateTime(2025, 10, 1), EndDate = null, BasePrice = 17.00m, Description = "Current rate for tent site" }
                );

            // ============================================================================
            // SEED DATA - Sites (2-3 per type)
            // ============================================================================

            modelBuilder
                .Entity<Site>()
                .HasData(
                    // Type 1: Trailers (3 sites - varied max lengths)
                    new Site { SiteID = 1, SiteNumber = "2",      SiteTypeID = 1, MaxLengthFeet = 40, IsActive = true },
                    new Site { SiteID = 2, SiteNumber = "17",     SiteTypeID = 1, MaxLengthFeet = 45, IsActive = true },
                    new Site { SiteID = 3, SiteNumber = "32",     SiteTypeID = 1, MaxLengthFeet = 65, IsActive = true },
                    // Type 2: Walk-in Trailers (2 sites)
                    new Site { SiteID = 4, SiteNumber = "11B",    SiteTypeID = 2, MaxLengthFeet = 30, Notes = "Walk-in trailer with linens", IsActive = true },
                    new Site { SiteID = 5, SiteNumber = "12B",    SiteTypeID = 2, MaxLengthFeet = 30, Notes = "Walk-in trailer with linens", IsActive = true },
                    // Type 3: Dry Storage (2 sites)
                    new Site { SiteID = 6, SiteNumber = "A",      SiteTypeID = 3, MaxLengthFeet = 65, Notes = "Dry storage site", IsActive = true },
                    new Site { SiteID = 7, SiteNumber = "B",      SiteTypeID = 3, MaxLengthFeet = 65, Notes = "Dry storage site", IsActive = true },
                    // Type 4: Tent (1 site)
                    new Site { SiteID = 8, SiteNumber = "TENT-1", SiteTypeID = 4, MaxLengthFeet = 0,  Notes = "Tent site near Dog Park", IsActive = true }
                );

            // ============================================================================
            // SEED DATA - Site Photos (1-2 per site)
            // ============================================================================

            modelBuilder
                .Entity<SitePhoto>()
                .HasData(
                    new SitePhoto { PhotoID = 1, SiteID = 1, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0694.jpg?w=4032&ssl=1",  Caption = "Site 2 - 40ft Trailer",      DisplayOrder = 1, DateUploaded = new DateTime(2026, 1, 1) },
                    new SitePhoto { PhotoID = 2, SiteID = 2, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0954.jpg?w=4032&ssl=1",  Caption = "Site 17 - 45ft Trailer",     DisplayOrder = 1, DateUploaded = new DateTime(2026, 1, 1) },
                    new SitePhoto { PhotoID = 9, SiteID = 2, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0512.jpg?w=4032&ssl=1",  Caption = "Site 17 - Hookup View",      DisplayOrder = 2, DateUploaded = new DateTime(2026, 1, 1) },
                    new SitePhoto { PhotoID = 3, SiteID = 3, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1226.jpg?w=4032&ssl=1",  Caption = "Site 32 - 65ft Trailer",     DisplayOrder = 1, DateUploaded = new DateTime(2026, 1, 1) },
                    new SitePhoto { PhotoID = 4, SiteID = 4, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/06/IMG_1309.jpg?w=4032&ssl=1",  Caption = "Site 11B - Walk-in Trailer", DisplayOrder = 1, DateUploaded = new DateTime(2026, 1, 1) },
                    new SitePhoto { PhotoID = 5, SiteID = 5, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0439.jpg?w=4032&ssl=1",  Caption = "Site 12B - Walk-in Trailer", DisplayOrder = 1, DateUploaded = new DateTime(2026, 1, 1) },
                    new SitePhoto { PhotoID = 6, SiteID = 6, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0603.jpg?w=4032&ssl=1",  Caption = "Dry Storage A",              DisplayOrder = 1, DateUploaded = new DateTime(2026, 1, 1) },
                    new SitePhoto { PhotoID = 7, SiteID = 7, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0444.jpg?w=4032&ssl=1",  Caption = "Dry Storage B",              DisplayOrder = 1, DateUploaded = new DateTime(2026, 1, 1) },
                    new SitePhoto { PhotoID = 8, SiteID = 8, PhotoURL = "https://i0.wp.com/pointofrocksrvcampground.com/wp-content/uploads/2019/05/IMG_0515.jpg?w=4032&ssl=1",  Caption = "Tent Site - Near Dog Park",  DisplayOrder = 1, DateUploaded = new DateTime(2026, 1, 1) }
                );

            // ============================================================================
            // SEED DATA - Reservations
            // 12 reservations, 3 per site type, varied statuses
            // Rates: Trailer=$25/night, Walk-in=$30/night, Dry Storage=$5/night, Tent=$17/night
            //
            // Type 1 - Trailers (1-45) : Res 1 (Completed), Res 2 (Cancelled), Res 3 (Upcoming)
            // Type 2 - Walk-in Trailers: Res 4 (Completed), Res 5 (In Progress), Res 6 (Upcoming)
            // Type 3 - Dry Storage     : Res 7 (Completed), Res 8 (Cancelled),   Res 9 (Upcoming)
            // Type 4 - Tent Site       : Res 10 (Completed), Res 11 (In Progress), Res 12 (Upcoming)
            // ============================================================================

            modelBuilder
                .Entity<Reservation>()
                .HasData(

                    // ── TYPE 1: TRAILERS (1-45) ──────────────────────────────────────────────

                    // 1. COMPLETED - Site 2 (40ft), 3 nights @ $25 = $75
                    new Reservation
                    {
                        ReservationID = 1,
                        CustomerID = 1,
                        SiteID = 1,
                        ReservationStatusID = 3,
                        StartDate = new DateTime(2026, 2, 10),
                        EndDate = new DateTime(2026, 2, 13),
                        TrailerLengthFeet = 38,
                        BaseAmount = 75.00m,
                        TotalAmount = 75.00m,
                        BalanceDue = 0m,
                        ScheduledCheckInTime  = new DateTime(2026, 2, 10, 13, 0, 0),
                        ActualCheckInTime     = new DateTime(2026, 2, 10, 13, 20, 0),
                        ScheduledCheckOutTime = new DateTime(2026, 2, 13, 12, 0, 0),
                        ActualCheckOutTime    = new DateTime(2026, 2, 13, 11, 50, 0),
                        DateCreated = new DateTime(2026, 2, 1),
                        LastUpdated = new DateTime(2026, 2, 13),
                    },
                    // 2. CANCELLED - Site 17 (45ft), 4 nights @ $25 = $100 + $10 cancel fee = $110
                    new Reservation
                    {
                        ReservationID = 2,
                        CustomerID = 1,
                        SiteID = 2,
                        ReservationStatusID = 4,
                        StartDate = new DateTime(2026, 3, 5),
                        EndDate = new DateTime(2026, 3, 9),
                        TrailerLengthFeet = 42,
                        BaseAmount = 100.00m,
                        TotalAmount = 110.00m,
                        BalanceDue = 0m,
                        ScheduledCheckInTime  = new DateTime(2026, 3, 5, 13, 0, 0),
                        ActualCheckInTime     = null,
                        ScheduledCheckOutTime = new DateTime(2026, 3, 9, 12, 0, 0),
                        ActualCheckOutTime    = null,
                        DateCreated = new DateTime(2026, 2, 15),
                        LastUpdated = new DateTime(2026, 2, 28),
                        Notes = "Cancelled - plans changed",
                    },
                    // 3. UPCOMING - Site 32 (65ft), 7 nights @ $25 = $175
                    new Reservation
                    {
                        ReservationID = 3,
                        CustomerID = 1,
                        SiteID = 3,
                        ReservationStatusID = 1,
                        StartDate = new DateTime(2026, 4, 25),
                        EndDate = new DateTime(2026, 5, 2),
                        TrailerLengthFeet = 60,
                        BaseAmount = 175.00m,
                        TotalAmount = 175.00m,
                        BalanceDue = 0m,
                        ScheduledCheckInTime  = new DateTime(2026, 4, 25, 13, 0, 0),
                        ActualCheckInTime     = null,
                        ScheduledCheckOutTime = new DateTime(2026, 5, 2, 12, 0, 0),
                        ActualCheckOutTime    = null,
                        DateCreated = new DateTime(2026, 4, 10),
                        LastUpdated = new DateTime(2026, 4, 10),
                    },

                    // ── TYPE 2: WALK-IN TRAILERS ─────────────────────────────────────────────

                    // 4. COMPLETED - Site 11B, 3 nights @ $30 = $90
                    new Reservation
                    {
                        ReservationID = 4,
                        CustomerID = 1,
                        SiteID = 4,
                        ReservationStatusID = 3,
                        StartDate = new DateTime(2026, 2, 20),
                        EndDate = new DateTime(2026, 2, 23),
                        TrailerLengthFeet = null,
                        BaseAmount = 90.00m,
                        TotalAmount = 90.00m,
                        BalanceDue = 0m,
                        ScheduledCheckInTime  = new DateTime(2026, 2, 20, 13, 0, 0),
                        ActualCheckInTime     = new DateTime(2026, 2, 20, 13, 10, 0),
                        ScheduledCheckOutTime = new DateTime(2026, 2, 23, 12, 0, 0),
                        ActualCheckOutTime    = new DateTime(2026, 2, 23, 11, 30, 0),
                        DateCreated = new DateTime(2026, 2, 10),
                        LastUpdated = new DateTime(2026, 2, 23),
                    },
                    // 5. IN PROGRESS - Site 12B, 5 nights @ $30 = $150 + $15 early check-in = $165
                    new Reservation
                    {
                        ReservationID = 5,
                        CustomerID = 1,
                        SiteID = 5,
                        ReservationStatusID = 2,
                        StartDate = new DateTime(2026, 4, 18),
                        EndDate = new DateTime(2026, 4, 23),
                        TrailerLengthFeet = null,
                        BaseAmount = 150.00m,
                        TotalAmount = 165.00m,
                        BalanceDue = 0m,
                        ScheduledCheckInTime  = new DateTime(2026, 4, 18, 8, 0, 0),
                        ActualCheckInTime     = new DateTime(2026, 4, 18, 8, 10, 0),
                        ScheduledCheckOutTime = new DateTime(2026, 4, 23, 12, 0, 0),
                        ActualCheckOutTime    = null,
                        DateCreated = new DateTime(2026, 4, 5),
                        LastUpdated = new DateTime(2026, 4, 18),
                    },
                    // 6. UPCOMING - Site 11B, 4 nights @ $30 = $120
                    new Reservation
                    {
                        ReservationID = 6,
                        CustomerID = 1,
                        SiteID = 4,
                        ReservationStatusID = 1,
                        StartDate = new DateTime(2026, 5, 10),
                        EndDate = new DateTime(2026, 5, 14),
                        TrailerLengthFeet = null,
                        BaseAmount = 120.00m,
                        TotalAmount = 120.00m,
                        BalanceDue = 0m,
                        ScheduledCheckInTime  = new DateTime(2026, 5, 10, 13, 0, 0),
                        ActualCheckInTime     = null,
                        ScheduledCheckOutTime = new DateTime(2026, 5, 14, 12, 0, 0),
                        ActualCheckOutTime    = null,
                        DateCreated = new DateTime(2026, 4, 20),
                        LastUpdated = new DateTime(2026, 4, 20),
                    },

                    // ── TYPE 3: DRY STORAGE ──────────────────────────────────────────────────

                    // 7. COMPLETED - Site A, 10 nights @ $5 = $50
                    new Reservation
                    {
                        ReservationID = 7,
                        CustomerID = 1,
                        SiteID = 6,
                        ReservationStatusID = 3,
                        StartDate = new DateTime(2026, 2, 1),
                        EndDate = new DateTime(2026, 2, 11),
                        TrailerLengthFeet = 45,
                        BaseAmount = 50.00m,
                        TotalAmount = 50.00m,
                        BalanceDue = 0m,
                        ScheduledCheckInTime  = new DateTime(2026, 2, 1, 13, 0, 0),
                        ActualCheckInTime     = new DateTime(2026, 2, 1, 13, 30, 0),
                        ScheduledCheckOutTime = new DateTime(2026, 2, 11, 12, 0, 0),
                        ActualCheckOutTime    = new DateTime(2026, 2, 11, 12, 0, 0),
                        DateCreated = new DateTime(2026, 1, 20),
                        LastUpdated = new DateTime(2026, 2, 11),
                    },
                    // 8. CANCELLED - Site B, 6 nights @ $5 = $30 + $10 cancel fee = $40
                    new Reservation
                    {
                        ReservationID = 8,
                        CustomerID = 1,
                        SiteID = 7,
                        ReservationStatusID = 4,
                        StartDate = new DateTime(2026, 3, 15),
                        EndDate = new DateTime(2026, 3, 21),
                        TrailerLengthFeet = 50,
                        BaseAmount = 30.00m,
                        TotalAmount = 40.00m,
                        BalanceDue = 0m,
                        ScheduledCheckInTime  = new DateTime(2026, 3, 15, 13, 0, 0),
                        ActualCheckInTime     = null,
                        ScheduledCheckOutTime = new DateTime(2026, 3, 21, 12, 0, 0),
                        ActualCheckOutTime    = null,
                        DateCreated = new DateTime(2026, 3, 1),
                        LastUpdated = new DateTime(2026, 3, 12),
                        Notes = "Cancelled - vehicle sold",
                    },
                    // 9. UPCOMING - Site A, 30 nights @ $5 = $150
                    new Reservation
                    {
                        ReservationID = 9,
                        CustomerID = 1,
                        SiteID = 6,
                        ReservationStatusID = 1,
                        StartDate = new DateTime(2026, 5, 1),
                        EndDate = new DateTime(2026, 5, 31),
                        TrailerLengthFeet = 48,
                        BaseAmount = 150.00m,
                        TotalAmount = 150.00m,
                        BalanceDue = 0m,
                        ScheduledCheckInTime  = new DateTime(2026, 5, 1, 13, 0, 0),
                        ActualCheckInTime     = null,
                        ScheduledCheckOutTime = new DateTime(2026, 5, 31, 12, 0, 0),
                        ActualCheckOutTime    = null,
                        DateCreated = new DateTime(2026, 4, 15),
                        LastUpdated = new DateTime(2026, 4, 15),
                        Notes = "30-day dry storage rental",
                    },

                    // ── TYPE 4: TENT SITE ────────────────────────────────────────────────────

                    // 10. COMPLETED - TENT-1, 2 nights @ $17 = $34
                    new Reservation
                    {
                        ReservationID = 10,
                        CustomerID = 1,
                        SiteID = 8,
                        ReservationStatusID = 3,
                        StartDate = new DateTime(2026, 2, 28),
                        EndDate = new DateTime(2026, 3, 2),
                        TrailerLengthFeet = null,
                        BaseAmount = 34.00m,
                        TotalAmount = 34.00m,
                        BalanceDue = 0m,
                        ScheduledCheckInTime  = new DateTime(2026, 2, 28, 13, 0, 0),
                        ActualCheckInTime     = new DateTime(2026, 2, 28, 14, 0, 0),
                        ScheduledCheckOutTime = new DateTime(2026, 3, 2, 12, 0, 0),
                        ActualCheckOutTime    = new DateTime(2026, 3, 2, 11, 45, 0),
                        DateCreated = new DateTime(2026, 2, 18),
                        LastUpdated = new DateTime(2026, 3, 2),
                    },
                    // 11. IN PROGRESS - TENT-1, 3 nights @ $17 = $51 + $25 late checkout = $76
                    new Reservation
                    {
                        ReservationID = 11,
                        CustomerID = 1,
                        SiteID = 8,
                        ReservationStatusID = 2,
                        StartDate = new DateTime(2026, 4, 17),
                        EndDate = new DateTime(2026, 4, 20),
                        TrailerLengthFeet = null,
                        BaseAmount = 51.00m,
                        TotalAmount = 76.00m,
                        BalanceDue = 0m,
                        ScheduledCheckInTime  = new DateTime(2026, 4, 17, 13, 0, 0),
                        ActualCheckInTime     = new DateTime(2026, 4, 17, 13, 5, 0),
                        ScheduledCheckOutTime = new DateTime(2026, 4, 20, 12, 0, 0),
                        ActualCheckOutTime    = null,
                        DateCreated = new DateTime(2026, 4, 7),
                        LastUpdated = new DateTime(2026, 4, 17),
                    },
                    // 12. UPCOMING - TENT-1, 4 nights @ $17 = $68
                    new Reservation
                    {
                        ReservationID = 12,
                        CustomerID = 1,
                        SiteID = 8,
                        ReservationStatusID = 1,
                        StartDate = new DateTime(2026, 5, 5),
                        EndDate = new DateTime(2026, 5, 9),
                        TrailerLengthFeet = null,
                        BaseAmount = 68.00m,
                        TotalAmount = 68.00m,
                        BalanceDue = 0m,
                        ScheduledCheckInTime  = new DateTime(2026, 5, 5, 13, 0, 0),
                        ActualCheckInTime     = null,
                        ScheduledCheckOutTime = new DateTime(2026, 5, 9, 12, 0, 0),
                        ActualCheckOutTime    = null,
                        DateCreated = new DateTime(2026, 4, 22),
                        LastUpdated = new DateTime(2026, 4, 22),
                    }
                );

            // ============================================================================
            // SEED DATA - Reservation Fees
            // ============================================================================

            modelBuilder
                .Entity<ReservationFee>()
                .HasData(
                    // Res 2: Cancellation fee
                    new ReservationFee
                    {
                        ReservationFeeID = 1,
                        ReservationID = 2,
                        FeeID = 3,
                        Amount = 10.00m,
                        DateApplied = new DateTime(2026, 2, 28),
                        Notes = "Cancellation fee applied",
                    },
                    // Res 5: Early Check-in fee
                    new ReservationFee
                    {
                        ReservationFeeID = 2,
                        ReservationID = 5,
                        FeeID = 2,
                        Amount = 15.00m,
                        DateApplied = new DateTime(2026, 4, 18),
                        Notes = "Early check-in at 8:00 AM",
                    },
                    // Res 8: Cancellation fee
                    new ReservationFee
                    {
                        ReservationFeeID = 3,
                        ReservationID = 8,
                        FeeID = 3,
                        Amount = 10.00m,
                        DateApplied = new DateTime(2026, 3, 12),
                        Notes = "Cancellation fee applied",
                    },
                    // Res 11: Late Checkout fee
                    new ReservationFee
                    {
                        ReservationFeeID = 4,
                        ReservationID = 11,
                        FeeID = 1,
                        Amount = 25.00m,
                        DateApplied = new DateTime(2026, 4, 17),
                        Notes = "Late checkout requested",
                    }
                );

            // ============================================================================
            // SEED DATA - Invoices
            // ============================================================================

            modelBuilder
                .Entity<Invoice>()
                .HasData(
                    new Invoice { InvoiceID = 1,  ReservationID = 1,  CustomerID = 1, SubTotal = 75.00m,  TotalFees = 0m,     TotalAmount = 75.00m,  InvoiceDate = new DateTime(2026, 2, 1),  DueDate = new DateTime(2026, 2, 10), IsPaid = true, DatePaid = new DateTime(2026, 2, 1) },
                    new Invoice { InvoiceID = 2,  ReservationID = 2,  CustomerID = 1, SubTotal = 100.00m, TotalFees = 10.00m, TotalAmount = 110.00m, InvoiceDate = new DateTime(2026, 2, 15), DueDate = new DateTime(2026, 3, 5),  IsPaid = true, DatePaid = new DateTime(2026, 2, 15) },
                    new Invoice { InvoiceID = 3,  ReservationID = 3,  CustomerID = 1, SubTotal = 175.00m, TotalFees = 0m,     TotalAmount = 175.00m, InvoiceDate = new DateTime(2026, 4, 10), DueDate = new DateTime(2026, 4, 25), IsPaid = true, DatePaid = new DateTime(2026, 4, 10) },
                    new Invoice { InvoiceID = 4,  ReservationID = 4,  CustomerID = 1, SubTotal = 90.00m,  TotalFees = 0m,     TotalAmount = 90.00m,  InvoiceDate = new DateTime(2026, 2, 10), DueDate = new DateTime(2026, 2, 20), IsPaid = true, DatePaid = new DateTime(2026, 2, 10) },
                    new Invoice { InvoiceID = 5,  ReservationID = 5,  CustomerID = 1, SubTotal = 150.00m, TotalFees = 15.00m, TotalAmount = 165.00m, InvoiceDate = new DateTime(2026, 4, 5),  DueDate = new DateTime(2026, 4, 18), IsPaid = true, DatePaid = new DateTime(2026, 4, 5) },
                    new Invoice { InvoiceID = 6,  ReservationID = 6,  CustomerID = 1, SubTotal = 120.00m, TotalFees = 0m,     TotalAmount = 120.00m, InvoiceDate = new DateTime(2026, 4, 20), DueDate = new DateTime(2026, 5, 10), IsPaid = true, DatePaid = new DateTime(2026, 4, 20) },
                    new Invoice { InvoiceID = 7,  ReservationID = 7,  CustomerID = 1, SubTotal = 50.00m,  TotalFees = 0m,     TotalAmount = 50.00m,  InvoiceDate = new DateTime(2026, 1, 20), DueDate = new DateTime(2026, 2, 1),  IsPaid = true, DatePaid = new DateTime(2026, 1, 20) },
                    new Invoice { InvoiceID = 8,  ReservationID = 8,  CustomerID = 1, SubTotal = 30.00m,  TotalFees = 10.00m, TotalAmount = 40.00m,  InvoiceDate = new DateTime(2026, 3, 1),  DueDate = new DateTime(2026, 3, 15), IsPaid = true, DatePaid = new DateTime(2026, 3, 1) },
                    new Invoice { InvoiceID = 9,  ReservationID = 9,  CustomerID = 1, SubTotal = 150.00m, TotalFees = 0m,     TotalAmount = 150.00m, InvoiceDate = new DateTime(2026, 4, 15), DueDate = new DateTime(2026, 5, 1),  IsPaid = true, DatePaid = new DateTime(2026, 4, 15) },
                    new Invoice { InvoiceID = 10, ReservationID = 10, CustomerID = 1, SubTotal = 34.00m,  TotalFees = 0m,     TotalAmount = 34.00m,  InvoiceDate = new DateTime(2026, 2, 18), DueDate = new DateTime(2026, 2, 28), IsPaid = true, DatePaid = new DateTime(2026, 2, 18) },
                    new Invoice { InvoiceID = 11, ReservationID = 11, CustomerID = 1, SubTotal = 51.00m,  TotalFees = 25.00m, TotalAmount = 76.00m,  InvoiceDate = new DateTime(2026, 4, 7),  DueDate = new DateTime(2026, 4, 17), IsPaid = true, DatePaid = new DateTime(2026, 4, 7) },
                    new Invoice { InvoiceID = 12, ReservationID = 12, CustomerID = 1, SubTotal = 68.00m,  TotalFees = 0m,     TotalAmount = 68.00m,  InvoiceDate = new DateTime(2026, 4, 22), DueDate = new DateTime(2026, 5, 5),  IsPaid = true, DatePaid = new DateTime(2026, 4, 22) }
                );

            // ============================================================================
            // SEED DATA - Payments
            // ============================================================================

            modelBuilder
                .Entity<Payment>()
                .HasData(
                    // Res 1 - Completed trailer, credit card
                    new Payment { PaymentID = 1,  InvoiceID = 1,  PaymentMethodID = 1, Amount = 75.00m,   PaymentDate = new DateTime(2026, 2, 1),  StripeTransactionID = "txn_demo_001",        PaymentStatus = "Completed", IsRefund = false },
                    // Res 2 - Cancelled: paid then refunded minus $10 fee
                    new Payment { PaymentID = 2,  InvoiceID = 2,  PaymentMethodID = 1, Amount = 110.00m,  PaymentDate = new DateTime(2026, 2, 15), StripeTransactionID = "txn_demo_002",        PaymentStatus = "Completed", IsRefund = false },
                    new Payment { PaymentID = 3,  InvoiceID = 2,  PaymentMethodID = 1, Amount = -100.00m, PaymentDate = new DateTime(2026, 2, 28), StripeTransactionID = "txn_demo_002_refund", PaymentStatus = "Refunded",  IsRefund = true,  Notes = "Refund: $100.00 (kept $10.00 cancellation fee)" },
                    // Res 3 - Upcoming trailer, debit card
                    new Payment { PaymentID = 4,  InvoiceID = 3,  PaymentMethodID = 2, Amount = 175.00m,  PaymentDate = new DateTime(2026, 4, 10), StripeTransactionID = "txn_demo_003",        PaymentStatus = "Completed", IsRefund = false },
                    // Res 4 - Completed walk-in trailer, credit card
                    new Payment { PaymentID = 5,  InvoiceID = 4,  PaymentMethodID = 1, Amount = 90.00m,   PaymentDate = new DateTime(2026, 2, 10), StripeTransactionID = "txn_demo_004",        PaymentStatus = "Completed", IsRefund = false },
                    // Res 5 - In Progress walk-in trailer + early check-in fee, credit card
                    new Payment { PaymentID = 6,  InvoiceID = 5,  PaymentMethodID = 1, Amount = 165.00m,  PaymentDate = new DateTime(2026, 4, 5),  StripeTransactionID = "txn_demo_005",        PaymentStatus = "Completed", IsRefund = false },
                    // Res 6 - Upcoming walk-in trailer, debit card
                    new Payment { PaymentID = 7,  InvoiceID = 6,  PaymentMethodID = 2, Amount = 120.00m,  PaymentDate = new DateTime(2026, 4, 20), StripeTransactionID = "txn_demo_006",        PaymentStatus = "Completed", IsRefund = false },
                    // Res 7 - Completed dry storage, cash (processed by employee)
                    new Payment { PaymentID = 8,  InvoiceID = 7,  PaymentMethodID = 3, Amount = 50.00m,   PaymentDate = new DateTime(2026, 1, 20), PaymentStatus = "Completed", ProcessedByEmployeeID = 1, IsRefund = false },
                    // Res 8 - Cancelled dry storage: paid then refunded minus $10 fee
                    new Payment { PaymentID = 9,  InvoiceID = 8,  PaymentMethodID = 1, Amount = 40.00m,   PaymentDate = new DateTime(2026, 3, 1),  StripeTransactionID = "txn_demo_008",        PaymentStatus = "Completed", IsRefund = false },
                    new Payment { PaymentID = 10, InvoiceID = 8,  PaymentMethodID = 1, Amount = -30.00m,  PaymentDate = new DateTime(2026, 3, 12), StripeTransactionID = "txn_demo_008_refund", PaymentStatus = "Refunded",  IsRefund = true,  Notes = "Refund: $30.00 (kept $10.00 cancellation fee)" },
                    // Res 9 - Upcoming dry storage, cash (processed by employee)
                    new Payment { PaymentID = 11, InvoiceID = 9,  PaymentMethodID = 3, Amount = 150.00m,  PaymentDate = new DateTime(2026, 4, 15), PaymentStatus = "Completed", ProcessedByEmployeeID = 1, IsRefund = false },
                    // Res 10 - Completed tent, debit card
                    new Payment { PaymentID = 12, InvoiceID = 10, PaymentMethodID = 2, Amount = 34.00m,   PaymentDate = new DateTime(2026, 2, 18), StripeTransactionID = "txn_demo_010",        PaymentStatus = "Completed", IsRefund = false },
                    // Res 11 - In Progress tent + late checkout fee, credit card
                    new Payment { PaymentID = 13, InvoiceID = 11, PaymentMethodID = 1, Amount = 76.00m,   PaymentDate = new DateTime(2026, 4, 7),  StripeTransactionID = "txn_demo_011",        PaymentStatus = "Completed", IsRefund = false },
                    // Res 12 - Upcoming tent, credit card
                    new Payment { PaymentID = 14, InvoiceID = 12, PaymentMethodID = 1, Amount = 68.00m,   PaymentDate = new DateTime(2026, 4, 22), StripeTransactionID = "txn_demo_012",        PaymentStatus = "Completed", IsRefund = false }
                );
        }
    }
}
