using Microsoft.EntityFrameworkCore;

namespace Demo3;

public class Demo3
{
    public static void Main()
    {
        // Ensure database is created and has a person in it
        using (var setupContext = new PersonContext())
        {
            setupContext.Database.EnsureDeleted();
            setupContext.Database.EnsureCreated();

            setupContext.People.Add(new Person { FirstName = "John", LastName = "Doe", PhoneNumber = "000-000-0000" });
            setupContext.SaveChanges();
        }

        using var context = new PersonContext();

        // Fetch a person from database and change phone number
        var person = context.People.Single(p => p.PersonId == 1);
        person.PhoneNumber = "555-555-5555";

        // Change the person's name in the database to simulate a concurrency conflict
        context.Database.ExecuteSqlRaw(
            "UPDATE dbo.People SET FirstName = 'Jane' WHERE PersonId = 1");

        var saved = false;

        while (!saved)
        {
            try
            {
                // Attempt to save changes to the database
                context.SaveChanges();
                saved = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is Person)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = entry.GetDatabaseValues();

                        foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            var databaseValue = databaseValues[property];

                            // decide which value should be written to database
                            proposedValues[property] = proposedValue;
                        }

                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                    else
                    {
                        throw new NotSupportedException(
                            "Don't know how to handle concurrency conflicts for "
                            + entry.Metadata.Name);
                    }
                }
            }
        }
    }
}