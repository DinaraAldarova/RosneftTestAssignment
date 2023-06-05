namespace RosneftTestAssignment.Models
{
    public class DesignObject
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Project Project { get; set; }
        public DesignObject? DesignObjectParent { get; set; }
        public List<DesignObject> DesignObjectChildren { get; set; }

        public string FullCode => $"{(DesignObjectParent is not null ? DesignObjectParent.Code : Project.Cipher)}.{Code}";

        public DesignObject(int id, string code, string name, Project project, DesignObject? designObjectParent, List<DesignObject> designObjectChildren)
        {
            Id = id;
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Project = project ?? throw new ArgumentNullException(nameof(project));
            DesignObjectParent = designObjectParent; //allow null
            DesignObjectChildren = designObjectChildren ?? new List<DesignObject>();
        }
        public DesignObject(int id, string code, string name, Project project, DesignObject? designObjectParent = null) : this(id, code, name, project, designObjectParent, new List<DesignObject>()) { }
        public DesignObject(int id = 0, string code = "", string name = "") : this(id, code, name, new Project()) { }
    }
}
