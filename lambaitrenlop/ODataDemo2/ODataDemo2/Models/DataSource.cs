namespace ODataDemo2.Models;

public static class DataSource
{
    private static IList<Book>? _books;

    public static IList<Book> GetBooks()
    {
        if (_books != null)
        {
            return _books;
        }

        var addisonWesley = new Press
        {
            Name = "Addison-Wesley",
            Email = "contact@aw.com",
            Category = Category.Book
        };

        var oReilly = new Press
        {
            Name = "O'Reilly Media",
            Email = "info@oreilly.com",
            Category = Category.Book
        };

        var manning = new Press
        {
            Name = "Manning Publications",
            Email = "support@manning.com",
            Category = Category.Book
        };

        var packt = new Press
        {
            Name = "Packt Publishing",
            Email = "hello@packt.com",
            Category = Category.EBook
        };

        var natGeo = new Press
        {
            Name = "National Geographic",
            Email = "magazine@natgeo.com",
            Category = Category.Magazine
        };

        _books = new List<Book>
        {
            new()
            {
                ISBN = "978-0-321-87758-1",
                Title = "Essential C# 5.0",
                Author = "Mark Michaelis",
                Price = 59.99m,
                Location = new Address { City = "HCM City", Street = "D2, Thu Duc District" },
                Press = addisonWesley
            },
            new()
            {
                ISBN = "978-1-492-05345-4",
                Title = "Learning ASP.NET Core",
                Author = "Jess Chadwick",
                Price = 45.50m,
                Location = new Address { City = "Hanoi", Street = "Cau Giay District" },
                Press = oReilly
            },
            new()
            {
                ISBN = "978-1-617-29413-6",
                Title = "Entity Framework Core in Action",
                Author = "Jon P Smith",
                Price = 52.00m,
                Location = new Address { City = "Da Nang", Street = "Hai Chau District" },
                Press = manning
            },
            new()
            {
                ISBN = "978-1-788-83978-8",
                Title = "ASP.NET Core 8 Recipes",
                Author = "Mark J. Price",
                Price = 39.99m,
                Location = new Address { City = "HCM City", Street = "District 1" },
                Press = packt
            },
            new()
            {
                ISBN = "978-0-321-87758-2",
                Title = "Effective C#",
                Author = "Bill Wagner",
                Price = 48.75m,
                Location = new Address { City = "Can Tho", Street = "Ninh Kieu District" },
                Press = addisonWesley
            },
            new()
            {
                ISBN = "978-1-426-21487-5",
                Title = "Programming C# 12",
                Author = "Ian Griffiths",
                Price = 55.00m,
                Location = new Address { City = "Hue", Street = "Phu Hoi Ward" },
                Press = oReilly
            },
            new()
            {
                ISBN = "978-1-426-29900-1",
                Title = "National Geographic - Wildlife 2025",
                Author = "National Geographic Editors",
                Price = 12.99m,
                Location = new Address { City = "HCM City", Street = "District 3" },
                Press = natGeo
            }
        };

        return _books;
    }
}
