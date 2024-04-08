using System.ComponentModel.DataAnnotations;

namespace Demo2
{
    public class Person
    {
        public int PersonId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [ConcurrencyCheck]
        public Guid Version { get; set; }
    }
}
