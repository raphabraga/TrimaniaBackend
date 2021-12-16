namespace Backend.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Street { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public override string ToString()
        {
            return $"{Number} {Street} St., {Neighborhood}, {City}-{State}";
        }
    }
}