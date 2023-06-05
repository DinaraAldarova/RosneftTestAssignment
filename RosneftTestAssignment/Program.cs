using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;
using RosneftTestAssignment;
using RosneftTestAssignment.Models;
using RosneftTestAssignment.Models.Dictionary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

Storage storage = Storage.getInstance(app.Configuration.GetConnectionString("TestDatabase"));
storage.UpdateData();

/*Тестовое в Роснефть:
API:
getParents(): [{id, name}, ...]
getParent(id): {id, name, desc, ...}
getParentsWithChildren([parent_id]): [{id, name, children: [...]}, ...]
getChild(id): {id, name, desc, ...}

REST неприменим из-за иерархической структуры, но можно взять Flat REST:
/parent
/parent?id=1
/child?parent_id=1
/child?id=1*/
app.MapGet("/project", ([FromQuery] int? id, [FromQuery] bool? full_tree) =>
//[FromQuery] в .Net6 не работает со сложными типами, но у меня int, так что все ок
{
    try
    {
        if (id.HasValue)
        {
            if (full_tree.HasValue && full_tree.Value is true)
            {
                var project = storage.GetProject(id.Value);
                var response = Converter.GetProjectWithTreeResponse(project);
                return Results.Ok(response);
            }
            else
            {
                var project = storage.GetProject(id.Value);
                var response = Converter.GetProjectResponse(project);
                return Results.Ok(response);
            }
        }
        else
        {
            if (full_tree.HasValue && full_tree.Value is true)
            {
                var projects = storage.GetProjects();
                var response = new List<ProjectWithTreeResponse>();
                foreach (var project in projects) { response.Add(Converter.GetProjectWithTreeResponse(project)); }
                return Results.Ok(response.ToArray());
            }
            else
            {
                var projects = storage.GetProjects();
                var response = new List<ProjectResponse>();
                foreach (var project in projects) { response.Add(Converter.GetProjectResponse(project)); }
                return Results.Ok(response.ToArray());
            }
        }
    }
    catch { return Results.NotFound(); }
});

app.MapGet("/design_object", ([FromQuery] int? id, [FromQuery] bool? full_tree) =>
{
    try
    {
        if (id.HasValue)
        {
            if (full_tree.HasValue && full_tree.Value is true)
            {
                var designObject = storage.GetDesignObject(id.Value);
                var response = Converter.GetDesignObjectWithTreeResponse(designObject);
                return Results.Ok(response);
            }
            else
            {
                var designObject = storage.GetDesignObject(id.Value);
                var response = Converter.GetDesignObjectResponse(designObject);
                return Results.Ok(response);
            }
        }
        else
        {
            if (full_tree.HasValue && full_tree.Value is true)
            {
                var designObjects = storage.GetDesignObjects();
                var response = new List<DesignObjectWithTreeResponse>();
                foreach (var designObject in designObjects) { response.Add(Converter.GetDesignObjectWithTreeResponse(designObject)); }
                return Results.Ok(response.ToArray());
            }
            else
            {
                var designObjects = storage.GetDesignObjects();
                var response = new List<DesignObjectResponse>();
                foreach (var designObject in designObjects) { response.Add(Converter.GetDesignObjectResponse(designObject)); }
                return Results.Ok(response.ToArray());
            }
        }
    }
    catch { return Results.NotFound(); }
});

app.MapGet("/documentation_set", ([FromQuery] int? id) =>
{
    try
    {
        if (id.HasValue)
        {
            var documents = storage.GetDocumentationSet(id.Value);
            var response = Converter.GetDocumentationSetResponse(documents);
            return Results.Ok(response);
        }
        else
        {
            var documentationSets = storage.GetDocumentationSets();
            var response = new List<DocumentationSetResponse>();
            foreach (var documents in documentationSets) { response.Add(Converter.GetDocumentationSetResponse(documents)); }
            return Results.Ok(response.ToArray());
        }
    }
    catch { return Results.NotFound(); }
});

app.MapGet("/mark", ([FromQuery] int? id) =>
{
    try
    {
        if (id.HasValue)
        {
            var mark = storage.GetMark(id.Value);
            var response = Converter.GetMarkResponse(mark);
            return Results.Ok(response);
        }
        else
        {
            var marks = storage.GetMarks();
            var response = new List<MarkResponse>();
            foreach (var mark in marks) { response.Add(Converter.GetMarkResponse(mark)); }
            return Results.Ok(response.ToArray());
        }
    }
    catch { return Results.NotFound(); }
});

app.Run();

/*
Код объекта проектирования (полный код объекта формируется как "Полный код родительского объекта.Собственный код")
Номер (нумеруются комплекты с нуля). Марка + Номер = уникальный ключ комплекта внутри объекта проектирования
Полный шифр комплекта (формируется как "Шифр проекта-Полный код объекта-МаркаНомер")
*/

record ProjectResponse(
    int id,
    string cipher,
    string name,
    int[] design_objects_id)
{ }
record ProjectWithTreeResponse(
    int id,
    string cipher,
    string name,
    DesignObjectWithTreeResponse[] design_objects)
{ }
record DesignObjectResponse(
    int id,
    string code,
    string full_code,
    string name,
    int project_id,
    int? parent_design_object_id,
    int[] design_objects_id)
{ }
record DesignObjectWithTreeResponse(
    int id,
    string code,
    string full_code,
    string name,
    int project_id,
    int? parent_design_object_id,
    DesignObjectWithTreeResponse[] design_objects)
{ }
record DocumentationSetResponse(
    int id,
    int mark_id,
    int number,
    string full_number,
    string full_cipher,
    int design_object_id)
{ }
record MarkResponse(
    int id,
    string name,
    string full_name)
{ }

static class Converter
{
    public static ProjectResponse GetProjectResponse(Project project)
    {
        return new ProjectResponse(
            id: project.Id,
            cipher: project.Cipher,
            name: project.Name,
            design_objects_id: project.DesignObjects.Select(designObject => designObject.Id).ToArray());
    }
    public static DesignObjectResponse GetDesignObjectResponse(DesignObject designObject)
    {
        return new DesignObjectResponse(
            id: designObject.Id,
            code: designObject.Code,
            full_code: designObject.FullCode,
            name: designObject.Name,
            project_id: designObject.Project.Id,
            parent_design_object_id: designObject.DesignObjectParent?.Id ?? null,
            design_objects_id: designObject.DesignObjectChildren.Select(child => child.Id).ToArray());
    }
    public static ProjectWithTreeResponse GetProjectWithTreeResponse(Project project)
    {
        return new ProjectWithTreeResponse(
            id: project.Id,
            cipher: project.Cipher,
            name: project.Name,
            design_objects: project.DesignObjects.Select(designObject => GetDesignObjectWithTreeResponse(designObject)).ToArray());
    }
    public static DesignObjectWithTreeResponse GetDesignObjectWithTreeResponse(DesignObject designObject)
    {
        return new DesignObjectWithTreeResponse(
            id: designObject.Id,
            code: designObject.Code,
            full_code: designObject.FullCode,
            name: designObject.Name,
            project_id: designObject.Project.Id,
            parent_design_object_id: designObject.DesignObjectParent?.Id ?? null,
            design_objects: designObject.DesignObjectChildren.Select(child => GetDesignObjectWithTreeResponse(child)).ToArray());
    }
    public static DocumentationSetResponse GetDocumentationSetResponse(DocumentationSet documentation)
    {
        return new DocumentationSetResponse(
            id: documentation.Id,
            mark_id: documentation.Mark.Id,
            number: documentation.Number,
            full_number: documentation.FullNumber,
            full_cipher: documentation.FullCipher,
            design_object_id: documentation.DesignObject.Id);
    }
    public static MarkResponse GetMarkResponse(Mark mark)
    {
        return new MarkResponse(
            id: mark.Id,
            name: mark.Name,
            full_name: mark.FullName);
    }
}