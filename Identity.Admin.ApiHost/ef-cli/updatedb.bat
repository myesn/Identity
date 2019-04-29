cd ..
dotnet ef database update -c IdentityDbContext
rmdir /S /Q Migrations
cd ef-cli