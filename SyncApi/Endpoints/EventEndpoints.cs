using SyncApi.Context;
using SyncApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SyncApi.Endpoints
{
    public static class EventEndpoints
    {
        static string endpoint = "/api/events";
        static string table = "Events";

        public static void MapEventEndpoints(this WebApplication app)
        {
            app.MapGet(endpoint + "/All", async (EventDbContext db) =>
                await db.Events.ToListAsync()
            )
            .Produces<List<Event>>(StatusCodes.Status200OK)
            .WithName($"GetAll{table}").WithTags(table);

            app.MapGet(endpoint, async (EventDbContext db) =>
                await db.Events.Where(x => x.IsActive).ToListAsync()
            )
            .Produces<List<Event>>(StatusCodes.Status200OK)
            .WithName($"Get{table}").WithTags(table);

            app.MapGet(endpoint + "/{id}", async (EventDbContext db, int id) =>
            {
                try
                {
                    return await db.Events.FindAsync(id)
                        is Event data ? Results.Ok(data) : Results.NotFound();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return Results.NotFound();
                }
            }
            )
            .Produces<Event>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName($"Get{table}ById").WithTags(table);

            app.MapPost(endpoint, async ([FromBody] Event data,
                [FromServices] EventDbContext db) =>
            {
                data.IsActive = true;
                db.Events.Add(data);
                await db.SaveChangesAsync();
                return Results.Ok(data);
            })
                .Accepts<Event>("application/json")
                .Produces<Event>(StatusCodes.Status201Created)
                .WithName($"Add{table}").WithTags(table);

            app.MapPost(endpoint + "/All", async ([FromBody] List<Event> data,
                [FromServices] EventDbContext db) =>
            {
                db.Events.AddRange(data);
                await db.SaveChangesAsync();
                return Results.Ok(data);
            })
                .Accepts<List<Event>>("application/json")
                .Produces<List<Event>>(StatusCodes.Status201Created)
                .WithName($"AddAll{table}").WithTags(table);

            app.MapPut(endpoint + "/{id}", async (int id,
                [FromBody] Event data,
                [FromServices] EventDbContext db) =>
            {
                if (id != data.IdServer)
                    return Results.BadRequest();

                db.Entry(data).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    return Results.NotFound();
                }
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName($"Update{table}").WithTags(table);

            app.MapPut(endpoint + "/All", async ([FromBody] List<Event> data,
                [FromServices] EventDbContext db) =>
            {
                var updated = 0;
                var error = 0;

                foreach (var item in data)
                {
                    db.Entry(item).State = EntityState.Modified;

                    try
                    {
                        await db.SaveChangesAsync();
                        updated++;
                    }
                    catch (Exception ex)
                    {
                        error++;
                    }
                }

                return Results.Ok(new { Updated = updated, Error = error});
            })
            .Produces(StatusCodes.Status200OK)
            .WithName($"UpdateAll{table}").WithTags(table);

            app.MapDelete(endpoint + "/{id}", async (int id,
                [FromServices] EventDbContext db) =>
            {
                var data = await db.Events.FindAsync(id);

                if (data == null)
                    return Results.NotFound();

                db.Events.Remove(data);

                await db.SaveChangesAsync();
                return Results.Ok(data);
            })
            .Produces<Event>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName($"Delete{table}").WithTags(table);

            app.MapDelete(endpoint + "/All", async ([FromBody]List<int> ids,
                [FromServices] EventDbContext db) =>
            {
                var deleted = 0;
                var error = 0;

                foreach (var id in ids)
                {
                    var data = await db.Events.FindAsync(id);

                    if (data != null)
                    {
                        db.Events.Remove(data);
                        await db.SaveChangesAsync();
                        deleted++;
                    }
                    else
                        error++;
                }

                return Results.Ok(new { Deleted = deleted, Error = error });
            })
            .Produces(StatusCodes.Status200OK)
            .WithName($"DeleteAll{table}").WithTags(table);

            app.MapPut(endpoint + "/SoftDelete/{id}", async (int id,
                [FromServices] EventDbContext db) =>
            {
                var data = await db.Events.FindAsync(id);

                if (data == null)
                    return Results.NotFound();

                data.IsActive = false;
                db.Entry(data).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    return Results.NotFound();
                }
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName($"SoftDelete{table}").WithTags(table);

            app.MapPut(endpoint + "/SoftDelete/All", async ([FromBody] List<int> ids,
                [FromServices] EventDbContext db) =>
            {
                var deleted = 0;
                var error = 0;

                foreach (var id in ids)
                {
                    var data = await db.Events.FindAsync(id);

                    if (data != null)
                    {
                        try
                        {
                            data.IsActive = false;
                            db.Entry(data).State = EntityState.Modified;
                            await db.SaveChangesAsync();

                            deleted++;
                        }
                        catch (Exception ex)
                        {
                            error++;
                        }
                    }
                    else
                    {
                        error++;
                    }
                }
            })
            .Produces(StatusCodes.Status200OK)
            .WithName($"SoftDeleteAll{table}").WithTags(table);
        }
    }
}