namespace RosneftTestAssignment.Models.Dictionary
{
    public class Mark
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }

        public Mark(int id = 0, string name = "", string fullName = "")
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        }
        public Mark() : this(0, "", "") { }
    }
}
