using FixItNow.Domain.Models.Accesses;
using FixItNow.Domain.Models.BookingRequest.Chat;
using FixItNow.Domain.Models.BookingRequest.Reviews;
using FixItNow.Domain.Models.BookingRequest.Tickets;
using FixItNow.Domain.Models.Bookings;
using FixItNow.Domain.Models.Tickets;
using FixItNow.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Application.Services;

public class SeedService(FixItNowDataContext context)
{
    public async Task SeedAsync()
    {
        // Guard — don't re-seed if data already exists
        if (await context.Users.CountAsync() > 3)
            return;

        var now = DateTime.UtcNow;

        // ----------------------------------------------------------------
        // CUSTOMERS
        // ----------------------------------------------------------------
        var customers = new List<User>
        {
            new() { FullName = "Budi Santoso",      Username = "budisantoso",   Email = "budi.santoso@gmail.com",       PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!") },
            new() { FullName = "Siti Rahayu",       Username = "sitirahayu",    Email = "siti.rahayu@gmail.com",        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!") },
            new() { FullName = "Andi Wijaya",       Username = "andiwijaya",    Email = "andi.wijaya@gmail.com",        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!") },
            new() { FullName = "Dewi Kusuma",       Username = "dewikusuma",    Email = "dewi.kusuma@gmail.com",        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!") },
            new() { FullName = "Riko Firmansyah",   Username = "rikofirmansyah",Email = "riko.firmansyah@gmail.com",   PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!") },
            new() { FullName = "Nadia Putri",       Username = "nadiaputri",    Email = "nadia.putri@gmail.com",        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!") },
        };

        await context.Users.AddRangeAsync(customers);
        await context.SaveChangesAsync();

        // ----------------------------------------------------------------
        // TECHNICIANS (User + TechnicianProfile)
        // ----------------------------------------------------------------
        var techUsers = new List<User>
        {
            new() { FullName = "Ahmad Fauzi",       Username = "ahmadfauzi",    Email = "ahmad.fauzi@gmail.com",        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!") },
            new() { FullName = "Rizky Pratama",     Username = "rizkypratama",  Email = "rizky.pratama@gmail.com",      PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!") },
            new() { FullName = "Hendra Gunawan",    Username = "hendragunawan", Email = "hendra.gunawan@gmail.com",     PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!") },
            new() { FullName = "Yusuf Hakim",       Username = "yusufhakim",    Email = "yusuf.hakim@gmail.com",        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!") },
            new() { FullName = "Bagas Nugroho",     Username = "bagasnugroho",  Email = "bagas.nugroho@gmail.com",      PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!") },
        };

        await context.Users.AddRangeAsync(techUsers);
        await context.SaveChangesAsync();

        var techProfiles = new List<TechnicianProfile>
        {
            new()
            {
                UserId = techUsers[0].Id,
                Bio = "Spesialis perbaikan AC dan instalasi listrik rumahan dengan pengalaman lebih dari 7 tahun. Siap membantu kebutuhan teknis Anda dengan cepat dan profesional.",
                Skills = "AC Repair, Electrical Installation, Wiring",
                Location = "Jakarta Selatan",
                YearsExperience = 7,
                HourlyRate = 85000,
                CallOutFee = 50000,
                PhoneNumber = "081234567801",
                IsApproved = true,
                IsOnline = true,
            },
            new()
            {
                UserId = techUsers[1].Id,
                Bio = "Tukang ledeng profesional berpengalaman dalam pemasangan pipa, perbaikan kebocoran, dan instalasi water heater. Kerja rapi dan tepat waktu.",
                Skills = "Plumbing, Pipe Installation, Water Heater, Leak Repair",
                Location = "Jakarta Barat",
                YearsExperience = 5,
                HourlyRate = 75000,
                CallOutFee = 40000,
                PhoneNumber = "081234567802",
                IsApproved = true,
                IsOnline = true,
            },
            new()
            {
                UserId = techUsers[2].Id,
                Bio = "Teknisi elektronik dan peralatan rumah tangga. Berpengalaman memperbaiki kulkas, mesin cuci, dan kompor gas. Garansi perbaikan 30 hari.",
                Skills = "Electronics Repair, Refrigerator, Washing Machine, Gas Stove",
                Location = "Bekasi",
                YearsExperience = 9,
                HourlyRate = 90000,
                CallOutFee = 45000,
                PhoneNumber = "081234567803",
                IsApproved = true,
                IsOnline = false,
            },
            new()
            {
                UserId = techUsers[3].Id,
                Bio = "Ahli pengecatan rumah interior dan eksterior. Hasil rapi, cat berkualitas, dan harga terjangkau. Sudah menangani lebih dari 200 proyek.",
                Skills = "House Painting, Interior, Exterior, Waterproofing",
                Location = "Tangerang",
                YearsExperience = 6,
                HourlyRate = 70000,
                CallOutFee = 35000,
                PhoneNumber = "081234567804",
                IsApproved = true,
                IsOnline = true,
            },
            new()
            {
                UserId = techUsers[4].Id,
                Bio = "Spesialis kunci dan keamanan rumah. Melayani duplikat kunci, ganti kunci pintu, dan instalasi kunci digital. Layanan darurat 24 jam.",
                Skills = "Locksmith, Door Lock, Digital Lock, Key Duplication",
                Location = "Depok",
                YearsExperience = 4,
                HourlyRate = 65000,
                CallOutFee = 50000,
                PhoneNumber = "081234567805",
                IsApproved = true,
                IsOnline = true,
            },
        };

        await context.TechnicianProfiles.AddRangeAsync(techProfiles);
        await context.SaveChangesAsync();

        // ----------------------------------------------------------------
        // COMPLETED BOOKINGS + TICKETS + REVIEWS
        // ----------------------------------------------------------------
        var completedData = new[]
        {
            new { CustIdx = 0, TechIdx = 0, Title = "AC tidak dingin di ruang tamu", Category = "AC Repair",             Location = "Jakarta Selatan", Desc = "AC sudah tidak mengeluarkan udara dingin sejak 2 hari lalu. Sudah coba reset tapi tidak membantu.",         DaysAgo = 50, Rating = 5, Comment = "Ahmad sangat profesional dan cepat. AC langsung dingin setelah diperbaiki. Sangat direkomendasikan!" },
            new { CustIdx = 1, TechIdx = 1, Title = "Pipa bocor di bawah wastafel",  Category = "Plumbing",              Location = "Jakarta Barat",   Desc = "Ada kebocoran di sambungan pipa di bawah wastafel dapur. Air menetes terus dan sudah merusak kabinet.",     DaysAgo = 48, Rating = 5, Comment = "Rizky datang tepat waktu dan langsung tahu masalahnya. Pekerjaan rapi dan bersih. Terima kasih!" },
            new { CustIdx = 2, TechIdx = 2, Title = "Mesin cuci tidak mau berputar", Category = "Electronics Repair",    Location = "Bekasi",          Desc = "Mesin cuci front loading tidak mau spin. Sudah dicoba beberapa kali tapi drum tidak berputar sama sekali.",  DaysAgo = 45, Rating = 4, Comment = "Hendra berhasil memperbaiki mesin cuci saya. Harga sesuai estimasi. Puas dengan hasilnya." },
            new { CustIdx = 3, TechIdx = 3, Title = "Cat dinding kamar mengelupas",  Category = "House Painting",        Location = "Tangerang",       Desc = "Cat dinding kamar tidur utama mengelupas di beberapa titik akibat rembesan air. Perlu dicat ulang.",         DaysAgo = 42, Rating = 5, Comment = "Yusuf kerja sangat rapi! Warna catnya rata dan hasilnya bersih. Kamar jadi kelihatan baru." },
            new { CustIdx = 4, TechIdx = 4, Title = "Kunci pintu utama rusak",       Category = "Locksmith",             Location = "Depok",           Desc = "Kunci pintu depan macet dan tidak bisa dibuka dari luar. Handle sudah longgar dan perlu diganti segera.",    DaysAgo = 40, Rating = 5, Comment = "Bagas sangat responsif dan datang dalam 1 jam. Kunci baru terpasang dengan baik. Sangat profesional!" },
            new { CustIdx = 5, TechIdx = 0, Title = "Instalasi stop kontak tambahan", Category = "Electrical Installation", Location = "Jakarta Selatan", Desc = "Perlu tambah 3 stop kontak di ruang kerja untuk keperluan komputer dan peralatan kantor rumahan.",          DaysAgo = 38, Rating = 4, Comment = "Pekerjaan bagus dan aman. Ahmad juga menjelaskan dengan detail tentang kapasitas listrik yang aman." },
            new { CustIdx = 0, TechIdx = 1, Title = "Water heater tidak panas",      Category = "Water Heater",          Location = "Jakarta Selatan", Desc = "Water heater listrik sudah 3 hari tidak menghasilkan air panas. Lampu indikator menyala tapi air tetap dingin.", DaysAgo = 35, Rating = 5, Comment = "Rizky sangat ramah dan profesional. Masalah ternyata di elemen pemanas dan langsung diganti. Mantap!" },
            new { CustIdx = 1, TechIdx = 2, Title = "Kulkas tidak dingin",           Category = "Electronics Repair",    Location = "Jakarta Barat",   Desc = "Kulkas 2 pintu tiba-tiba tidak dingin. Kompresor terdengar berbunyi tapi suhu tidak turun.",                DaysAgo = 32, Rating = 4, Comment = "Hendra tahu persis masalahnya. Freon habis dan langsung diisi. Kulkas kembali normal. Recommended!" },
            new { CustIdx = 2, TechIdx = 3, Title = "Pengecatan ulang pagar depan",  Category = "House Painting",        Location = "Bekasi",          Desc = "Pagar besi depan rumah sudah karatan dan catnya terkelupas. Perlu diamplas, anti karat, dan dicat ulang.",   DaysAgo = 30, Rating = 5, Comment = "Hasil pengecatan luar biasa! Yusuf sangat teliti dan pakai cat anti karat berkualitas. Puas sekali!" },
            new { CustIdx = 3, TechIdx = 0, Title = "AC bocor air ke lantai",        Category = "AC Repair",             Location = "Tangerang",       Desc = "AC split di kamar tidur mengeluarkan air yang menetes ke lantai. Sudah terjadi sejak seminggu lalu.",        DaysAgo = 28, Rating = 5, Comment = "Ahmad cepat menemukan masalahnya — selang pembuangan tersumbat. Bersih dan profesional!" },
            new { CustIdx = 4, TechIdx = 1, Title = "Saluran WC tersumbat",          Category = "Plumbing",              Location = "Depok",           Desc = "WC tidak bisa flush dengan benar. Air naik dan pembuangan sangat lambat. Sudah terjadi 2 hari.",             DaysAgo = 25, Rating = 4, Comment = "Rizky berhasil bersihkan saluran yang tersumbat. Agak lama tapi hasilnya memuaskan." },
            new { CustIdx = 5, TechIdx = 4, Title = "Pasang kunci digital pintu kamar", Category = "Digital Lock",       Location = "Jakarta Selatan", Desc = "Ingin pasang kunci digital di pintu kamar utama. Sudah beli kuncinya tinggal perlu instalasi.",              DaysAgo = 22, Rating = 5, Comment = "Bagas sangat ahli dalam instalasi kunci digital. Dijelaskan cara pakainya juga. Sangat membantu!" },
        };

        foreach (var d in completedData)
        {
            var createdAt = now.AddDays(-d.DaysAgo);
            var customer = customers[d.CustIdx];
            var techUser = techUsers[d.TechIdx];

            var booking = new BookingRequest
            {
                CustomerId = customer.Id,
                TechnicianId = techUser.Id,
                Title = d.Title,
                Description = d.Desc,
                Category = d.Category,
                Location = d.Location,
                Status = BookingStatus.Accepted,
                CreatedAt = createdAt,
            };
            await context.BookingRequests.AddAsync(booking);
            await context.SaveChangesAsync();

            var ticket = new Ticket
            {
                Title = d.Title,
                Description = d.Desc,
                Category = d.Category,
                Location = d.Location,
                Status = TicketStatus.Completed,
                CreatedAt = createdAt,
                CustomerId = customer.Id,
                AssignedTechnicianId = techUser.Id,
                BookingRequestId = booking.Id,
            };
            await context.Tickets.AddAsync(ticket);
            await context.SaveChangesAsync();

            var review = new Review
            {
                TicketId = ticket.Id,
                CustomerId = customer.Id,
                TechnicianId = techUser.Id,
                Rating = d.Rating,
                Comment = d.Comment,
                CreatedAt = createdAt.AddHours(3),
            };
            await context.Reviews.AddAsync(review);
        }

        await context.SaveChangesAsync();

        // ----------------------------------------------------------------
        // IN-PROGRESS TICKETS
        // ----------------------------------------------------------------
        var inProgressData = new[]
        {
            new { CustIdx = 0, TechIdx = 2, Title = "Kompor gas tidak menyala",     Category = "Gas Stove",          Location = "Jakarta Selatan", Desc = "Dua tungku kompor gas tidak bisa menyala. Sudah ganti tabung gas tapi masih tidak keluar api." },
            new { CustIdx = 1, TechIdx = 3, Title = "Cat kamar mandi mengelupas",   Category = "House Painting",     Location = "Jakarta Barat",   Desc = "Cat di kamar mandi mengelupas akibat lembab. Area sekitar shower dan bawah wastafel paling parah." },
            new { CustIdx = 2, TechIdx = 0, Title = "AC remote tidak berfungsi",    Category = "AC Repair",          Location = "Bekasi",          Desc = "Remote AC tidak merespons sama sekali. Sudah ganti baterai tapi tetap tidak berfungsi." },
        };

        foreach (var d in inProgressData)
        {
            var createdAt = now.AddDays(-5);
            var customer = customers[d.CustIdx];
            var techUser = techUsers[d.TechIdx];

            var booking = new BookingRequest
            {
                CustomerId = customer.Id,
                TechnicianId = techUser.Id,
                Title = d.Title,
                Description = d.Desc,
                Category = d.Category,
                Location = d.Location,
                Status = BookingStatus.Accepted,
                CreatedAt = createdAt,
            };
            await context.BookingRequests.AddAsync(booking);
            await context.SaveChangesAsync();

            var ticket = new Ticket
            {
                Title = d.Title,
                Description = d.Desc,
                Category = d.Category,
                Location = d.Location,
                Status = TicketStatus.InProgress,
                CreatedAt = createdAt,
                CustomerId = customer.Id,
                AssignedTechnicianId = techUser.Id,
                BookingRequestId = booking.Id,
            };
            await context.Tickets.AddAsync(ticket);
        }

        await context.SaveChangesAsync();

        // ----------------------------------------------------------------
        // PENDING BOOKING REQUESTS (not yet accepted)
        // ----------------------------------------------------------------
        var pendingData = new[]
        {
            new { CustIdx = 3, TechIdx = 1, Title = "Wastafel kamar mandi bocor",   Category = "Plumbing",           Location = "Tangerang",       Desc = "Sambungan wastafel kamar mandi utama bocor. Air menetes ke bawah cabinet setiap kali dipakai." },
            new { CustIdx = 4, TechIdx = 0, Title = "Pasang lampu taman baru",      Category = "Electrical Installation", Location = "Depok",      Desc = "Ingin pasang 4 titik lampu taman di halaman depan rumah. Sudah ada kabel tapi belum ada stop kontak luar." },
        };

        foreach (var d in pendingData)
        {
            var booking = new BookingRequest
            {
                CustomerId = customers[d.CustIdx].Id,
                TechnicianId = techUsers[d.TechIdx].Id,
                Title = d.Title,
                Description = d.Desc,
                Category = d.Category,
                Location = d.Location,
                Status = BookingStatus.Pending,
                CreatedAt = now.AddHours(-6),
            };
            await context.BookingRequests.AddAsync(booking);
        }

        await context.SaveChangesAsync();

        // ----------------------------------------------------------------
        // CHAT MESSAGES on first completed ticket
        // ----------------------------------------------------------------
        var firstTicket = await context.Tickets
            .Where(t => t.Status == TicketStatus.Completed)
            .OrderBy(t => t.CreatedAt)
            .FirstOrDefaultAsync();

        if (firstTicket is not null)
        {
            var cust = await context.Users.FindAsync(firstTicket.CustomerId);
            var tech = await context.Users.FindAsync(firstTicket.AssignedTechnicianId);

            if (cust is not null && tech is not null)
            {
                var messages = new List<ChatMessage>
                {
                    new() { TicketId = firstTicket.Id, SenderId = cust.Id,  ReceiverId = tech.Id,  Message = "Halo, apakah bisa datang hari ini?",                                         CreatedAt = firstTicket.CreatedAt.AddMinutes(10), IsRead = true },
                    new() { TicketId = firstTicket.Id, SenderId = tech.Id,  ReceiverId = cust.Id,  Message = "Halo! Bisa, saya bisa datang sekitar jam 2 siang. Alamat lengkapnya?",        CreatedAt = firstTicket.CreatedAt.AddMinutes(15), IsRead = true },
                    new() { TicketId = firstTicket.Id, SenderId = cust.Id,  ReceiverId = tech.Id,  Message = "Jl. Kemang Raya No. 12, Jakarta Selatan. Ada gerbang warna biru.",           CreatedAt = firstTicket.CreatedAt.AddMinutes(18), IsRead = true },
                    new() { TicketId = firstTicket.Id, SenderId = tech.Id,  ReceiverId = cust.Id,  Message = "Baik, saya akan datang tepat waktu. Tolong sediakan akses ke unit AC-nya.",  CreatedAt = firstTicket.CreatedAt.AddMinutes(20), IsRead = true },
                    new() { TicketId = firstTicket.Id, SenderId = cust.Id,  ReceiverId = tech.Id,  Message = "Siap, terima kasih!",                                                        CreatedAt = firstTicket.CreatedAt.AddMinutes(22), IsRead = true },
                    new() { TicketId = firstTicket.Id, SenderId = tech.Id,  ReceiverId = cust.Id,  Message = "Sudah selesai ya Pak/Bu. Masalahnya di freon yang habis, sudah diisi ulang.", CreatedAt = firstTicket.CreatedAt.AddHours(2),   IsRead = true },
                    new() { TicketId = firstTicket.Id, SenderId = cust.Id,  ReceiverId = tech.Id,  Message = "Wah sudah dingin lagi! Terima kasih banyak ya.",                             CreatedAt = firstTicket.CreatedAt.AddHours(2).AddMinutes(5), IsRead = true },
                };

                await context.ChatMessages.AddRangeAsync(messages);
                await context.SaveChangesAsync();
            }
        }
    }
}