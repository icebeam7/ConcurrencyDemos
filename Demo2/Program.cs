using Demo2;

public class Program
{
    public static void Main()
    {
        // Ensure database is created and has a person in it
        using (var setupContext = new PersonContext())
        {
            setupContext.Database.EnsureDeleted();
            setupContext.Database.EnsureCreated();

            setupContext.People.Add(new Person { FirstName = "John", LastName = "Doe", Version = Guid.NewGuid() });
            setupContext.People.Add(new Person { FirstName = "Marie", LastName = "Jane", Version = Guid.NewGuid() });
            setupContext.SaveChanges();
        }

        SuccessfulUpdate();
        ConcurrencyFailure();
    }

    // This shows a successful update, where no concurrent change happens
    private static void SuccessfulUpdate()
    {
        using var context = new PersonContext();

        var person = context.People.Single(b => b.FirstName == "John");
        person.FirstName = "Paul";
        person.Version = Guid.NewGuid();
        context.SaveChanges();

        Console.WriteLine("The change completed successfully.");
        Console.ReadLine();
    }

    // This simulates a concurrency failure by modifying the row via another context after it was loaded.
    private static void ConcurrencyFailure()
    {
        using var context1 = new PersonContext();

        var person = context1.People.Single(b => b.FirstName == "Marie");
        person.FirstName = "Stephanie";
        person.Version = Guid.NewGuid();

        // We loaded the Person instance - along with its concurrency token - and made a change on it.
        // Let's simulate a concurrent change by updating the row from another context
        using (var context2 = new PersonContext())
        {
            var person2 = context2.People.Single(b => b.FirstName == "Marie");
            person2.FirstName = "Rachel";
            person2.Version = Guid.NewGuid();
            context2.SaveChanges();
        }

        // The tracked person in EF's change tracker has an out of date concurrency token, so calling SaveChanges will now throw
        // a DbUpdateConcurrencyException
        Console.WriteLine("SaveChanges should now throw:");
        context1.SaveChanges();
        Console.ReadLine();
    }
}