namespace RosneftTestAssignment.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Cipher { get; set; }
        public string Name { get; set; }
        public List<DesignObject> DesignObjects { get; set; } = new List<DesignObject>();



        public Project(int id, string cipher, string name, List<DesignObject> designObjects)
        {
            Id = id;
            Cipher = cipher ?? throw new ArgumentNullException(nameof(cipher));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DesignObjects = designObjects ?? new List<DesignObject>();
        }
        public Project(int id = 0, string cipher = "", string name = "") : this(id, cipher, name, new List<DesignObject>()) { }
    }
}
