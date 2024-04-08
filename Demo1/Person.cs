using System.ComponentModel.DataAnnotations;

namespace Demo1
{
    public class Person
    {
        public int PersonId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Timestamp]
        public byte[] Version { get; set; }
    }
}
