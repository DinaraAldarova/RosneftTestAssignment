using Microsoft.Data.Sqlite;
using RosneftTestAssignment.Models;
using RosneftTestAssignment.Models.Dictionary;

namespace RosneftTestAssignment
{
    public class Storage
    {
        private static Storage? instanse;
        public static Storage getInstance(string connectionString)
        {
            if (instanse is null)
            {
                instanse = new Storage(connectionString);
            }
            return instanse;
        }

        private Dictionary<int, Project> projects = new Dictionary<int, Project>();
        private Dictionary<int, DesignObject> designObjects = new Dictionary<int, DesignObject>();
        private Dictionary<int, DocumentationSet> documentations = new Dictionary<int, DocumentationSet>();
        private Dictionary<int, Mark> marks = new Dictionary<int, Mark>();

        private string connectionString;
        private SqliteConnection connection;

        private Storage(string connectionString)
        {
            this.connectionString = connectionString;
            connection = new SqliteConnection(connectionString);
        }

        public Project[] GetProjects() { return projects.Values.ToArray(); }
        public DesignObject[] GetDesignObjects() { return designObjects.Values.ToArray(); }
        public DocumentationSet[] GetDocumentationSets() { return documentations.Values.ToArray(); }
        public Mark[] GetMarks() { return marks.Values.ToArray(); }

        public Project GetProject(int id) { return projects[id]; }
        public DesignObject GetDesignObject(int id) { return designObjects[id]; }
        public DocumentationSet GetDocumentationSet(int id) { return documentations[id]; }
        public Mark GetMark(int id) { return marks[id]; }

        public int GetTreeDepthByProject(int projectId)
        {
            var project = projects[projectId];
            int depth = project.DesignObjects
                .Select(child => GetTreeDepthByDesignObject(child.Id))
                .Max();
            return 0;
        }
        public int GetTreeDepthByDesignObject(int designObjectId)
        {
            var designObject = designObjects[designObjectId];
            int depth = designObject.DesignObjectChildren
                .Select(child => GetTreeDepthByDesignObject(child.Id))
                .Max();
            return depth;
        }

        public void UpdateData()
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "select * from project;";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("id"));
                string cipher = reader.GetString(reader.GetOrdinal("cipher"));
                string name = reader.GetString(reader.GetOrdinal("name"));

                var project = new Project(id, cipher, name);
                if (projects.ContainsKey(id)) { projects[id] = project; }
                else { projects.Add(id, project); }
            }
            reader.Close();

            command = connection.CreateCommand();
            command.CommandText = "select * from design_object;";
            reader = command.ExecuteReader();
            Dictionary<int, int> designObjectParentLinks = new Dictionary<int, int>();
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("id"));
                string code = reader.GetString(reader.GetOrdinal("code"));
                string name = reader.GetString(reader.GetOrdinal("name"));
                int projectId = reader.GetInt32(reader.GetOrdinal("project_id"));
                int? designObjectId = reader.IsDBNull(reader.GetOrdinal("design_object_id")) ? null : reader.GetInt32(reader.GetOrdinal("design_object_id"));

                var designObject = new DesignObject(id, code, name, projects[projectId]);
                if (designObjectId is not null) { designObjectParentLinks[designObject.Id] = designObjectId.Value; }
                else { designObject.Project.DesignObjects.Add(designObject); }
                if (designObjects.ContainsKey(id)) { designObjects[id] = designObject; }
                else { designObjects.Add(id, designObject); }
            }
            reader.Close();
            foreach (var parentLink in designObjectParentLinks)
            {
                int childKey = parentLink.Key;
                var child = designObjects[childKey];

                int parentKey = parentLink.Value;
                var parent = designObjects[parentKey];

                child.DesignObjectParent = parent;
                parent.DesignObjectChildren.Add(child);
            }

            command = connection.CreateCommand();
            command.CommandText = "select * from mark;";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("id"));
                string name = reader.GetString(reader.GetOrdinal("name"));
                string fullName = reader.GetString(reader.GetOrdinal("full_name"));

                var mark = new Mark(id, name, fullName);
                if (marks.ContainsKey(id)) { marks[id] = mark; }
                else { marks.Add(id, mark); }
            }
            reader.Close();

            command = connection.CreateCommand();
            command.CommandText = "select * from documentation_set;";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("id"));
                int number = reader.GetInt32(reader.GetOrdinal("number"));
                int markId = reader.GetInt32(reader.GetOrdinal("mark_id"));
                int desingObjectId = reader.GetInt32(reader.GetOrdinal("design_object_id"));

                var documentation = new DocumentationSet(id, marks[markId], number, designObjects[desingObjectId]);
                if (documentations.ContainsKey(id)) { documentations[id] = documentation; }
                else { documentations.Add(id, documentation); }
            }
            reader.Close();

            connection.Close();
        }
    }
}
