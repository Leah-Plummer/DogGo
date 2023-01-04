namespace DogGo.Models
{
    public class Dog
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int OwnerId { get; set; }

        public Owner Owner { get; set; }
    }
}
