# SQLite için .NET Core NuGet Paketleri

## 📋 İçindekiler
1. [Paket Seçenekleri](#paket-seçenekleri)
2. [Kurulum](#kurulum)
3. [Karşılaştırma](#karşılaştırma)
4. [Kullanım Örnekleri](#kullanım-örnekleri)
5. [En İyi Uygulamalar](#en-iyi-uygulamalar)

---

## 🎯 Paket Seçenekleri

### 1. System.Data.SQLite.Core ⭐ (Tavsiye Edilir)

**NuGet Paket Adı:** `System.Data.SQLite.Core`  
**En Güncel Versiyon:** 1.0.118  
**Uyumlu Versiyon Aralığı:** 1.0.x

#### Özellikleri:
- ✅ Tam ADO.NET uyumluluğu
- ✅ .NET Framework ve .NET Core'da çalışır
- ✅ Geniş komunite desteği ve dokümantasyon
- ✅ Gelişmiş özellikler (veritabanı şifreleme, backup, vb.)
- ✅ Performans optimizasyonları
- ✅ Aktif geliştirme ve destek

#### Avantajları:
- En uzun geçmişe sahip ve en güvenilir paket
- Windows, Linux, macOS'ta sorunsuz çalışır
- SQL sorgularında tam kontrol
- Şifreleme ve diğer ileri özellikler

#### Dezavantajları:
- Biraz daha ağır (native library içerir)
- Microsoft.Data.Sqlite'dan daha büyük paket boyutu

#### Önerilen Kullanım:
- Geleneksel ADO.NET yaklaşımını tercih edenler
- Karmaşık SQL sorgularıyla çalışanlar
- Şifreleme gibi ileri özeleklere ihtiyaç duyanlar

---

### 2. Microsoft.Data.Sqlite (Modern Alternatif)

**NuGet Paket Adı:** `Microsoft.Data.Sqlite`  
**En Güncel Versiyon:** 8.0.0  
**Uyumlu Versiyon Aralığı:** 6.0.x, 7.0.x, 8.0.x

#### Özellikleri:
- ✅ Microsoft tarafından resmi olarak desteklenmektedir
- ✅ Daha hafif ve performanslı
- ✅ Entity Framework Core ile sorunsuz entegrasyon
- ✅ Tam async/await desteği
- ✅ Modern .NET tasarım prensipleri
- ✅ Düzenli güvenlik güncellemeleri

#### Avantajları:
- Microsoft resmi desteği ve dokümantasyon
- Entity Framework Core'la perfect entegrasyon
- Daha hafif ve hızlı
- Async/await patterns için tam destek
- .NET Core'a özel olarak tasarlanmış

#### Dezavantajları:
- System.Data.SQLite'dan daha az özellik
- Bazı ileri özellikler mevcut değil (örn: şifreleme)
- Komunite desteği az biraz daha azdır

#### Önerilen Kullanım:
- Entity Framework Core kullanacaksanız
- Modern async/await patterns tercih edenler
- .NET 6.0 ve üzeri projelerde
- Performans kritik uygulamalar

---

### 3. Entity Framework Core SQLite Provider

**NuGet Paket Adı:** `Microsoft.EntityFrameworkCore.Sqlite`  
**En Güncel Versiyon:** 8.0.0  
**Uyumlu Versiyon Aralığı:** 6.0.x, 7.0.x, 8.0.x

#### Özellikleri:
- ✅ Entity Framework Core ile tam entegrasyon
- ✅ LINQ sorgularıyla veritabanı işlemleri
- ✅ Migration desteği
- ✅ Otomatik schema yönetimi
- ✅ ORM (Object-Relational Mapping) fonksiyonları

#### Avantajları:
- Veritabanı sorgularını LINQ ile yazabilirsiniz
- Otomatik migration yönetimi
- Type-safe sorgular
- Veritabanı schema'sını kod üzerinden yönetin

#### Dezavantajları:
- ORM yapısı nedeniyle biraz daha yavaş olabilir
- Basit uygulamalar için gereksiz karmaşıklık
- Performans kritik uygulamalarda optimizasyon gerekli

#### Önerilen Kullanım:
- Entity Framework Core ORM kullanmak isteyenler
- Büyük ölçekli uygulamalar
- Multiple database desteği gerekiyorsa
- Database-first veya Code-first yaklaşım

---

## 📦 Kurulum

### NuGet Package Manager Console Üzerinden

```powershell
# System.Data.SQLite.Core
Install-Package System.Data.SQLite.Core

# Microsoft.Data.Sqlite
Install-Package Microsoft.Data.Sqlite

# Entity Framework Core SQLite
Install-Package Microsoft.EntityFrameworkCore.Sqlite
```

### .NET CLI (Command Line) Üzerinden

```bash
# System.Data.SQLite.Core
dotnet add package System.Data.SQLite.Core

# Microsoft.Data.Sqlite
dotnet add package Microsoft.Data.Sqlite

# Entity Framework Core SQLite
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

### Belirli Versiyon Yüklemek

```bash
# System.Data.SQLite.Core 1.0.118
dotnet add package System.Data.SQLite.Core --version 1.0.118

# Microsoft.Data.Sqlite 8.0.0
dotnet add package Microsoft.Data.Sqlite --version 8.0.0
```

### .csproj Dosyasına Doğrudan Ekleme

```xml
<ItemGroup>
  <PackageReference Include="System.Data.SQLite.Core" Version="1.0.118" />
</ItemGroup>
```

---

## 🔄 Karşılaştırma Tablosu

| Özellik | System.Data.SQLite.Core | Microsoft.Data.Sqlite | EF Core Sqlite |
|---------|------------------------|-----------------------|-----------------|
| **ADO.NET Desteği** | ✅ Tam | ✅ Tam | ⚠️ Kısıtlı |
| **Entity Framework Core** | ⚠️ Sınırlı | ✅ Mükemmel | ✅ Mükemmel |
| **Async/Await** | ✅ Var | ✅ Tam | ✅ Tam |
| **Şifreleme** | ✅ Evet | ❌ Hayır | ❌ Hayır |
| **Paket Boyutu** | 📦 Büyük | 📦 Küçük | 📦 Orta |
| **Performans** | ⚡ İyi | ⚡⚡ Çok İyi | ⚡ İyi |
| **LINQ Desteği** | ❌ Hayır | ✅ Var | ✅ Tam |
| **Dokümantasyon** | 📚 İyi | 📚 Çok İyi | 📚 Çok İyi |
| **Komunite** | 👥 Büyük | 👥 Büyük | 👥 Çok Büyük |
| **.NET Core Uyumluluğu** | ✅ Tam | ✅ Tam | ✅ Tam |

---

## 💻 Kullanım Örnekleri

### Seçenek 1: System.Data.SQLite.Core Kullanımı

```csharp
using System;
using System.Data.SQLite;

public class UserRepository
{
    private string _connectionString = "Data Source=mydatabase.db;";

    public void CreateDatabase()
    {
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL UNIQUE,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            using (SQLiteCommand command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public void InsertUser(string name, string email)
    {
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            
            string insertQuery = "INSERT INTO Users (Name, Email) VALUES (@name, @email)";
            
            using (SQLiteCommand command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@email", email);
                command.ExecuteNonQuery();
            }
        }
    }

    public List<(int Id, string Name, string Email)> GetAllUsers()
    {
        var users = new List<(int, string, string)>();
        
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            
            string selectQuery = "SELECT Id, Name, Email FROM Users";
            
            using (SQLiteCommand command = new SQLiteCommand(selectQuery, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add((
                            (int)reader["Id"],
                            (string)reader["Name"],
                            (string)reader["Email"]
                        ));
                    }
                }
            }
        }
        
        return users;
    }
}
```

### Seçenek 2: Microsoft.Data.Sqlite Kullanımı

```csharp
using System;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;

public class UserRepositorySqlite
{
    private string _connectionString = "Data Source=mydatabase.db;";

    public async Task CreateDatabaseAsync()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL UNIQUE,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task InsertUserAsync(string name, string email)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Users (Name, Email) VALUES (@name, @email)";
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@email", email);

            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<(int Id, string Name, string Email)>> GetAllUsersAsync()
    {
        var users = new List<(int, string, string)>();
        
        using (var connection = new SqliteConnection(_connectionString))
        {
            await connection.OpenAsync();
            
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Email FROM Users";

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    users.Add((
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2)
                    ));
                }
            }
        }
        
        return users;
    }
}
```

### Seçenek 3: Entity Framework Core Kullanımı

```csharp
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=mydatabase.db");
}

public class UserRepositoryEF
{
    private readonly AppDbContext _context;

    public UserRepositoryEF(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateDatabaseAsync()
    {
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task InsertUserAsync(string name, string email)
    {
        var user = new User { Name = name, Email = email };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task UpdateUserAsync(int id, string name, string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user != null)
        {
            user.Name = name;
            user.Email = email;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
```

---

## 🎯 En İyi Uygulamalar

### 1. **Connection String Yönetimi**

```csharp
// ❌ Kötü Örnek
string connectionString = "Data Source=mydatabase.db;";

// ✅ İyi Örnek (appsettings.json'dan oku)
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=mydatabase.db;"
  }
}

// C# kodu
public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
}
```

### 2. **Connection Pooling**

```csharp
// Microsoft.Data.Sqlite ile connection pooling
var connectionString = "Data Source=mydatabase.db;Mode=ReadWriteCreate;Pooling=True;";
using (var connection = new SqliteConnection(connectionString))
{
    // Connection otomatik olarak pool'a geri dönecek
}
```

### 3. **Async/Await Kullanımı**

```csharp
// ❌ Kötü Örnek (Sync)
var users = GetAllUsers(); // UI thread bloklama riski

// ✅ İyi Örnek (Async)
var users = await GetAllUsersAsync();
```

### 4. **Resource Management**

```csharp
// Using statement ile otomatik kapatma
using (var connection = new SqliteConnection(connectionString))
{
    // Connection otomatik olarak kapatılacak
}

// Async using pattern
await using (var connection = new SqliteConnection(connectionString))
{
    // Connection otomatik olarak kapatılacak
}
```

### 5. **Transaction Kullanımı**

```csharp
using (var connection = new SqliteConnection(connectionString))
{
    await connection.OpenAsync();
    using (var transaction = await connection.BeginTransactionAsync())
    {
        try
        {
            // Veritabanı işlemleri
            await command.ExecuteNonQueryAsync();
            
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

### 6. **Dependency Injection**

```csharp
// Program.cs (Startup ayarı)
services.AddDbContext<AppDbContext>();
services.AddScoped<IUserRepository, UserRepository>();

// Controller kullanımı
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UsersController(IUserRepository repository)
    {
        _repository = repository;
    }
}
```

---

## 🚀 Performans İpuçları

1. **Index Kullanımı:** Sık sorgu yapılan sütunlara index ekleyin
2. **Batch Operations:** Toplu işlemleri batch'te yapın
3. **Connection Pooling:** Bağlantı havuzlarını etkinleştirin
4. **Query Optimization:** N+1 problemini önleyin
5. **Async Operations:** Async/await patterns kullanın

---

## 📚 Kaynaklar

- [System.Data.SQLite Resmi Sayfası](https://www.sqlite.org/)
- [Microsoft.Data.Sqlite Dokümantasyonu](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/)
- [Entity Framework Core SQLite Provider](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/)
- [SQLite Best Practices](https://sqlite.org/bestpractices.html)

---

## ✅ Sonuç

**Önerilen Paket Seçimi:**

- **ADO.NET klasik yaklaşım:** → `System.Data.SQLite.Core`
- **Modern async patterns:** → `Microsoft.Data.Sqlite`
- **ORM kullanmak istiyorsanız:** → `Microsoft.EntityFrameworkCore.Sqlite`
- **Başlangıç için:** → `Microsoft.Data.Sqlite`

**En iyi uygulama:** Projenizin ihtiyaçlarına göre paket seçin ve tutarlı bir şekilde kullanın.

