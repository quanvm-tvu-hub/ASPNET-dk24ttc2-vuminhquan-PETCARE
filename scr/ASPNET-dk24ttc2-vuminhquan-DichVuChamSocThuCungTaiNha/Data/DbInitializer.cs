using WebThuCungNew.Models;

namespace WebThuCungNew.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PetServiceContext context)
        {
            context.Database.EnsureCreated();

            // Seed Services
            if (!context.Services.Any())
            {
                var services = new Service[]
                {
                    new Service
                    {
                        ServiceName = "Chăm sóc tại nhà 1 giờ",
                        Description = "Nhân viên đến nhà chăm sóc thú cưng trong 1 giờ",
                        Price = 100000,
                        ImageUrl = "https://images.unsplash.com/photo-1514888286974-6c03e2ca1dba?q=80&w=2043&auto=format&fit=crop"
                    },
                    new Service
                    {
                        ServiceName = "Dắt đi dạo",
                        Description = "Dắt thú cưng đi dạo quanh khu vực",
                        Price = 50000,
                        ImageUrl = "https://images.unsplash.com/photo-1601758228041-f3b2795255f1?q=80&w=2070&auto=format&fit=crop"
                    },
                    new Service
                    {
                        ServiceName = "Trông giữ cả ngày",
                        Description = "Trông giữ thú cưng tại nhà nhân viên hoặc tại shop",
                        Price = 300000,
                        ImageUrl = "https://images.unsplash.com/photo-1548199973-03cce0bbc87b?q=80&w=2069&auto=format&fit=crop"
                    }
                };

                foreach (Service s in services)
                {
                    context.Services.Add(s);
                }
                context.SaveChanges();
            }

            // Seed Users
            if (!context.Users.Any())
            {
                var users = new User[]
                {
                    new User
                    {
                        Email = "admin@petservice.com",
                        Password = "admin123",
                        FullName = "Nguyễn Văn Admin",
                        Role = "Admin",
                        Phone = "0909111111",
                        CreatedAt = DateTime.Now
                    },
                    new User
                    {
                        Email = "sitter1@petservice.com",
                        Password = "sitter123",
                        FullName = "Trần Thị Hoa",
                        Role = "Sitter",
                        Phone = "0909222222",
                        CreatedAt = DateTime.Now
                    },
                    new User
                    {
                        Email = "sitter2@petservice.com",
                        Password = "sitter123",
                        FullName = "Lê Văn Nam",
                        Role = "Sitter",
                        Phone = "0909333333",
                        CreatedAt = DateTime.Now
                    },
                    new User
                    {
                        Email = "client1@petservice.com",
                        Password = "client123",
                        FullName = "Phạm Minh Anh",
                        Role = "Client",
                        Phone = "0909444444",
                        Address = "123 Nguyễn Huệ, Q1, TP.HCM",
                        CreatedAt = DateTime.Now
                    },
                    new User
                    {
                        Email = "client2@petservice.com",
                        Password = "client123",
                        FullName = "Hoàng Thị Lan",
                        Role = "Client",
                        Phone = "0909555555",
                        Address = "456 Lê Lợi, Q1, TP.HCM",
                        CreatedAt = DateTime.Now
                    }
                };

                foreach (User u in users)
                {
                    context.Users.Add(u);
                }
                context.SaveChanges();

                // Seed Pets for clients
                var client1 = context.Users.First(u => u.Email == "client1@petservice.com");
                var client2 = context.Users.First(u => u.Email == "client2@petservice.com");

                var pets = new Pet[]
                {
                    new Pet
                    {
                        OwnerId = client1.UserId,
                        Name = "Milo",
                        Type = "Dog",
                        Breed = "Golden Retriever",
                        Notes = "Rất thân thiện, thích chơi bóng"
                    },
                    new Pet
                    {
                        OwnerId = client1.UserId,
                        Name = "Luna",
                        Type = "Cat",
                        Breed = "Persian",
                        Notes = "Ít vận động, thích ngủ"
                    },
                    new Pet
                    {
                        OwnerId = client2.UserId,
                        Name = "Max",
                        Type = "Dog",
                        Breed = "Husky",
                        Notes = "Năng động, cần vận động nhiều"
                    },
                    new Pet
                    {
                        OwnerId = client2.UserId,
                        Name = "Bella",
                        Type = "Cat",
                        Breed = "British Shorthair",
                        Notes = "Hiền lành, dễ chăm sóc"
                    }
                };

                foreach (Pet p in pets)
                {
                    context.Pets.Add(p);
                }
                context.SaveChanges();

                // Seed Bookings
                var service1 = context.Services.First(s => s.ServiceName.Contains("1 giờ"));
                var service2 = context.Services.First(s => s.ServiceName.Contains("dạo"));
                var service3 = context.Services.First(s => s.ServiceName.Contains("cả ngày"));
                var sitter1 = context.Users.First(u => u.Email == "sitter1@petservice.com");
                var sitter2 = context.Users.First(u => u.Email == "sitter2@petservice.com");
                var pet1 = context.Pets.First(p => p.Name == "Milo");
                var pet2 = context.Pets.First(p => p.Name == "Max");
                var pet3 = context.Pets.First(p => p.Name == "Luna");

                var bookings = new Booking[]
                {
                    // Booking from registered user - Completed
                    new Booking
                    {
                        ClientId = client1.UserId,
                        ServiceId = service1.ServiceId,
                        PetId = pet1.PetId,
                        SitterId = sitter1.UserId,
                        BookingDate = DateTime.Now.AddDays(-3),
                        Location = "123 Nguyễn Huệ, Q1, TP.HCM",
                        Status = "Completed",
                        PaymentMethod = "COD",
                        TotalPrice = service1.Price,
                        CreatedAt = DateTime.Now.AddDays(-5)
                    },
                    // Booking from registered user - Confirmed
                    new Booking
                    {
                        ClientId = client2.UserId,
                        ServiceId = service2.ServiceId,
                        PetId = pet2.PetId,
                        SitterId = sitter2.UserId,
                        BookingDate = DateTime.Now.AddDays(1),
                        Location = "456 Lê Lợi, Q1, TP.HCM",
                        Status = "Confirmed",
                        PaymentMethod = "BankTransfer",
                        TotalPrice = service2.Price,
                        CreatedAt = DateTime.Now.AddDays(-1)
                    },
                    // Booking from registered user - Pending
                    new Booking
                    {
                        ClientId = client1.UserId,
                        ServiceId = service3.ServiceId,
                        PetId = pet3.PetId,
                        BookingDate = DateTime.Now.AddDays(2),
                        Location = "123 Nguyễn Huệ, Q1, TP.HCM",
                        Status = "Pending",
                        PaymentMethod = "COD",
                        TotalPrice = service3.Price,
                        CreatedAt = DateTime.Now
                    },
                    // Guest booking - Pending
                    new Booking
                    {
                        GuestName = "Nguyễn Văn Khách",
                        GuestEmail = "khach@gmail.com",
                        GuestPhone = "0909999999",
                        PetName = "Lucky",
                        PetType = "Dog",
                        ServiceId = service1.ServiceId,
                        BookingDate = DateTime.Now.AddDays(3),
                        Location = "789 Trần Hưng Đạo, Q5, TP.HCM",
                        Status = "Pending",
                        PaymentMethod = "COD",
                        TotalPrice = service1.Price,
                        CreatedAt = DateTime.Now
                    },
                    // Guest booking - Confirmed
                    new Booking
                    {
                        GuestName = "Trần Thị Khách",
                        GuestEmail = "khach2@gmail.com",
                        GuestPhone = "0909888888",
                        PetName = "Kitty",
                        PetType = "Cat",
                        ServiceId = service2.ServiceId,
                        SitterId = sitter1.UserId,
                        BookingDate = DateTime.Now.AddDays(4),
                        Location = "321 Võ Văn Tần, Q3, TP.HCM",
                        Status = "Confirmed",
                        PaymentMethod = "BankTransfer",
                        TotalPrice = service2.Price,
                        CreatedAt = DateTime.Now.AddHours(-12)
                    }
                };

                foreach (Booking b in bookings)
                {
                    context.Bookings.Add(b);
                }
                context.SaveChanges();
            }
        }
    }
}
