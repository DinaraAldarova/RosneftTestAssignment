using Microsoft.AspNetCore.Mvc;
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

Storage storage = Storage.getInstance();
storage.UpdateData();

/*Тестовое в Роснефть:
API:
getParents(): [{id, name}, ...]
getParent(id): {id, name, desc, ...}
getParentsWithChildren([parent_id]): [{id, name, children: [...]}, ...]
getChild(id): {id, name, desc, ...}

REST неприменим, но можно взять Flat REST:
/parent
/parent?id=1
/child?parent_id=1
/child?id=1

Можно предусмотреть ограничение на количество сущностей, если слишком много - обрезать*/
app.MapGet("/project", () =>
{
    try
    {
        var projects = storage.GetProjects();
        var response = new List<ProjectResponse>();
        foreach (var project in projects)
        {
            response.Add(new ProjectResponse(
                id: project.Id,
                cipher: project.Cipher,
                name: project.Name,
                design_objects_id: project.DesignObjects.Select(designObject => designObject.Id).ToArray()
                ));
        }
        return Results.Ok(response.ToArray());
    }
    catch { return Results.NotFound(); }
});
app.MapGet("/project/{id}", ([FromRoute] int id) =>
{
    try
    {
        var project = storage.GetProject(id);
        var response = new ProjectResponse(
            id: project.Id,
            cipher: project.Cipher,
            name: project.Name,
            design_objects_id: project.DesignObjects.Select(designObject => designObject.Id).ToArray()
            );
        return Results.Ok(response);
    }
    catch { return Results.NotFound(); }
});

app.MapGet("/design_object", () =>
{
    try
    {
        var designObjects = storage.GetDesignObjects();
        var response = new List<DesignObjectResponse>();
        foreach (var designObject in designObjects)
        {
            response.Add(new DesignObjectResponse(
                id: designObject.Id,
                code: designObject.Code,
                name: designObject.Name,
                project_id: designObject?.Project?.Id ?? null,
                design_object_id: designObject?.DesignObjectParent?.Id ?? null,
                design_objects_id: designObject.DesignObjectChildren.Select(child => child.Id).ToArray()
                ));
        }
        return Results.Ok(response.ToArray());
    }
    catch { return Results.NotFound(); }
});
app.MapGet("/design_object/{id}", ([FromRoute] int id) =>
{
    try
    {
        var designObject = storage.GetDesignObject(id);
        var response = new DesignObjectResponse(
                id: designObject.Id,
                code: designObject.Code,
                name: designObject.Name,
                project_id: designObject?.Project?.Id ?? null,
                design_object_id: designObject?.DesignObjectParent?.Id ?? null,
                design_objects_id: designObject.DesignObjectChildren.Select(child => child.Id).ToArray()
                );
        return Results.Ok(response);
    }
    catch { return Results.NotFound(); }
});

app.MapGet("/documentation_set", () =>
{
    try
    {
        var documentationSets = storage.GetDocumentationSets();
        var response = new List<DocumentationSetResponse>();
        foreach (var documents in documentationSets)
        {
            response.Add(new DocumentationSetResponse(
                id: documents.Id,
                mark_id: documents.Mark.Id,
                number: documents.Number,
                design_object_id: documents.DesignObject.Id
                ));
        }
        return Results.Ok(response.ToArray());
    }
    catch { return Results.NotFound(); }
});
app.MapGet("/documentation_set/{id}", ([FromRoute] int id) =>
{
    try
    {
        var documents = storage.GetDocumentationSet(id);
        var response = new DocumentationSetResponse(
                id: documents.Id,
                mark_id: documents.Mark.Id,
                number: documents.Number,
                design_object_id: documents.DesignObject.Id
                );
        return Results.Ok(response);
    }
    catch { return Results.NotFound(); }
});

app.MapGet("/mark", () =>
{
    try
    {
        var marks = storage.GetMarks();
        var response = new List<MarkResponse>();
        foreach (var mark in marks)
        {
            response.Add(new MarkResponse(mark.Id, mark.Name, mark.FullName));
        }
        return Results.Ok(response.ToArray());
    }
    catch { return Results.NotFound(); }
});
app.MapGet("/mark/{id}", ([FromRoute] int id) =>
{
    try
    {
        var mark = storage.GetMark(id);
        var response = new MarkResponse(mark.Id, mark.Name, mark.FullName);
        return Results.Ok(response);
    }
    catch { return Results.NotFound(); }
});

app.Run();

internal record ProjectResponse(int id, string cipher, string name, int[] design_objects_id) { }
internal record DesignObjectResponse(int id, string code, string name, int? project_id, int? design_object_id, int[] design_objects_id) { }
internal record DocumentationSetResponse(int id, int mark_id, int number, int design_object_id) { }
internal record MarkResponse(int id, string name, string full_name) { }