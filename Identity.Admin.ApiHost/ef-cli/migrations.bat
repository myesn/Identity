cd ..
rmdir /S /Q Migrations

dotnet ef migrations add InitialIdentityDbContextMigration -c IdentityDbContext -o Migrations/IdentityDb

dotnet ef migrations script -c IdentityDbContext -o Migrations/IdentityDb.sql

cd ef-cli