namespace RosneftTestAssignment.Models
{
    public class DesignObject
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Project? Project { get; set; }
        public DesignObject? DesignObjectParent { get; set; }
        public List<DesignObject> DesignObjectChildren { get; set; }

        public string FullCode => $"{(Project is not null ? Project.Cipher : DesignObjectParent is not null ? DesignObjectParent.Code : "")}.{Code}";

        public DesignObject(int id, string code, string name, Project? project, DesignObject? designObjectParent, List<DesignObject> designObjectChildren)
        {
            Id = id;
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Project = project; //allow null
            DesignObjectParent = designObjectParent; //allow null
            DesignObjectChildren = designObjectChildren ?? new List<DesignObject>();
        }
        public DesignObject(int id = 0, string code = "", string name = "", Project? project = null, DesignObject? designObjectParent = null) : this(id, code, name, project, designObjectParent, new List<DesignObject>()) { }
    }
}
