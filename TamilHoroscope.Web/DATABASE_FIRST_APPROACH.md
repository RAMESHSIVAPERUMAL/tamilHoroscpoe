# ??? Database-First Approach - TamilHoroscope

## ?? **Overview**

This project uses **Database-First** approach, meaning:
- ? Database schema is managed via **SQL scripts**
- ? Entity Framework is used for **data access only**
- ? **NO** automatic migrations
- ? Schema changes done in SQL, not C# code

---

## ?? **Why Database-First?**

### **Advantages:**
1. ? **Full control** over database schema
2. ? **Performance optimizations** in SQL
3. ? **Complex constraints** easier to implement
4. ? **Direct DBA collaboration**
5. ? **No migration conflicts**

### **Trade-offs:**
- ?? Schema changes require SQL scripts
- ?? Must manually keep C# entities in sync
- ?? Cannot use `dotnet ef migrations`

---

## ?? **How It Works**

### **1. Database Creation**
All schema managed in: `TamilHoroscope.Web/DatabaseSetup/`

```
DatabaseSetup/
??? 01_CreateDatabase.sql    # Database creation
??? 02_CreateTables.sql       # Table definitions
??? 03_CreateIndexes.sql      # Performance indexes
??? 04_InsertInitialData.sql  # Seed data
??? 05_VerifySetup.sql        # Verification
```

### **2. Entity Framework Usage**
EF is used **ONLY** for:
- ? Querying data (LINQ)
- ? Inserting/updating records
- ? Change tracking
- ? **NOT for schema management**

### **3. Application Startup**
```csharp
// Program.cs - Database-First approach
using (var scope = app.Services.CreateScope())
{
    var context = services.GetRequiredService<ApplicationDbContext>();
    
    // Just verify connection - NO migrations
    var canConnect = context.Database.CanConnect();
    
    if (canConnect)
    {
        app.Logger.LogInformation("? Database connection successful");
    }
}
```

---

## ?? **Database Schema Management**

### **Making Schema Changes:**

#### **? DON'T DO:**
```bash
# WRONG - This won't work in database-first
dotnet ef migrations add AddNewColumn
dotnet ef database update
```

#### **? DO THIS:**

1. **Create SQL script:**
```sql
-- Example: Add new column
USE TamilHoroscope;
GO

ALTER TABLE Users
ADD PreferredLanguage NVARCHAR(10) NULL;
GO

PRINT '? Column added: PreferredLanguage';
```

2. **Update C# entity:**
```csharp
public class User
{
    // ... existing properties
    
    /// <summary>
    /// User's preferred language (en/ta)
    /// </summary>
    public string? PreferredLanguage { get; set; }
}
```

3. **Update EF configuration (if needed):**
```csharp
builder.Property(u => u.PreferredLanguage)
    .HasColumnName("PreferredLanguage")
    .HasMaxLength(10)
    .IsRequired(false);
```

4. **Run SQL script in SSMS**

5. **Test in application**

---

## ?? **Setup Process**

### **First Time Setup:**

1. **Run SQL scripts** (in order):
   ```
   01_CreateDatabase.sql
   02_CreateTables.sql
   03_CreateIndexes.sql
   04_InsertInitialData.sql
   05_VerifySetup.sql
   ```

2. **Configure connection string** in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=SERVER;Database=TamilHoroscope;..."
     }
   }
   ```

3. **Run application**:
   ```bash
   dotnet run
   ```

4. **Verify connection**:
   - Check logs for: "? Database connection successful"

---

## ?? **Project Structure**

### **Database Layer:**
```
TamilHoroscope.Web/
??? DatabaseSetup/          ? SQL scripts for schema
??? Data/
?   ??? ApplicationDbContext.cs    ? EF DbContext
?   ??? Entities/                  ? C# entity classes
?   ??? Configurations/            ? EF configurations
??? Migrations/            ? Documentation (no EF migrations)
```

### **Entity Framework Configuration:**
```csharp
// ApplicationDbContext.cs
public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    // ... other entities
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        // ... other configurations
    }
}
```

---

## ?? **Keeping Entities in Sync**

### **Entity Must Match Database:**

| Database Column | C# Property | EF Configuration |
|----------------|-------------|------------------|
| `UserId INT PK` | `int UserId` | `.HasKey(u => u.UserId)` |
| `Email NVARCHAR(256) NULL` | `string? Email` | `.HasMaxLength(256).IsRequired(false)` |
| `TrialStartDate DATETIME2 NULL` | `DateTime? TrialStartDate` | `.IsRequired(false)` |

### **Verification Query:**
```sql
-- Check if database matches code
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
ORDER BY ORDINAL_POSITION;
```

---

## ?? **Troubleshooting**

### **Issue: Migration error on startup**

**Error:**
```
Microsoft.Data.SqlClient.SqlException: Invalid object name '__EFMigrationsHistory'
```

**Cause:** EF is trying to run migrations

**Solution:** Update `Program.cs` to remove `context.Database.Migrate()`

### **Issue: Column not found**

**Error:**
```
Invalid column name 'NewColumn'
```

**Cause:** Database doesn't have column that C# entity expects

**Solution:** 
1. Check if SQL script ran successfully
2. Verify column exists in database
3. Ensure C# property name matches

### **Issue: Type mismatch**

**Error:**
```
Cannot convert from 'System.DateTime' to 'System.DateTime?'
```

**Cause:** Database column is NULL but C# property is non-nullable

**Solution:**
- Make C# property nullable: `DateTime?`
- Update EF config: `.IsRequired(false)`

---

## ? **Best Practices**

### **DO:**
1. ? Version control SQL scripts
2. ? Test scripts in dev environment first
3. ? Keep entities in sync with database
4. ? Document schema changes
5. ? Use transactions for complex changes

### **DON'T:**
1. ? Use `dotnet ef migrations`
2. ? Let EF manage schema
3. ? Modify database directly without script
4. ? Change entity without updating database
5. ? Run scripts in production without testing

---

## ?? **Common Tasks**

### **Add New Table:**

1. **SQL Script:**
```sql
CREATE TABLE Settings (
    SettingId INT IDENTITY(1,1) PRIMARY KEY,
    SettingKey NVARCHAR(100) NOT NULL,
    SettingValue NVARCHAR(500) NOT NULL
);
```

2. **C# Entity:**
```csharp
public class Setting
{
    public int SettingId { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
}
```

3. **Add to DbContext:**
```csharp
public DbSet<Setting> Settings => Set<Setting>();
```

4. **Create Configuration:**
```csharp
public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("Settings");
        builder.HasKey(s => s.SettingId);
        // ... other configurations
    }
}
```

### **Add New Column:**

1. **SQL Script:**
```sql
ALTER TABLE Users
ADD LastSeenDate DATETIME2 NULL;
```

2. **Update Entity:**
```csharp
public DateTime? LastSeenDate { get; set; }
```

3. **Update Configuration:**
```csharp
builder.Property(u => u.LastSeenDate)
    .HasColumnName("LastSeenDate")
    .IsRequired(false);
```

---

## ?? **Summary**

### **Database-First Workflow:**

```
1. Plan schema change
   ?
2. Write SQL script
   ?
3. Test in dev database
   ?
4. Update C# entities
   ?
5. Update EF configurations
   ?
6. Build and test app
   ?
7. Run SQL in production
```

### **Key Points:**
- ? SQL scripts are source of truth
- ? EF is for data access only
- ? No automatic migrations
- ? Manual sync between DB and code
- ? Full control over schema

---

## ?? **Need Help?**

### **Resources:**
- SQL scripts: `DatabaseSetup/` folder
- Entity configurations: `Data/Configurations/`
- Database verification: `DatabaseSetup/05_VerifySetup.sql`

### **Common Commands:**
```bash
# Build application
dotnet build

# Run application
dotnet run

# Check EF can connect
# (No migrations will run)
```

---

**Last Updated:** 2024-02-14  
**Approach:** Database-First  
**EF Version:** EF Core 8.0  
**Database:** SQL Server
