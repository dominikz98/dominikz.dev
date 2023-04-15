// using dominikz.Domain.Enums;
// using dominikz.Domain.Models;
// using dominikz.Infrastructure.Provider.Database;
// using dominikz.Infrastructure.Provider.Storage;
// using dominikz.Infrastructure.Provider.Storage.Requests;
// using Microsoft.EntityFrameworkCore;
//
// namespace dominikz.Tests;
//
// public class Playground
// {
//     [Fact(Skip = "!Playground!")]
//     public async Task Play()
//     {
//         var options = new DbContextOptionsBuilder<DatabaseContext>()
//             .UseSqlite("Data Source=/home/dominikzettl/RiderProjects/dominikz.dev/src/dominikz.Migrations/dominikz.db")
//             .EnableDetailedErrors()
//             .EnableSensitiveDataLogging()
//             .Options;
//
//         var context = new DatabaseContext(options);
//         var storage = new StorageProvider("/home/dominikzettl/DominikZ");
//
//         var articles = await context.From<ExtArticleShadow>().ToListAsync();
//         foreach (var article in articles)
//         {
//             if (Guid.TryParse(article.ImageUrl, out _))
//                 continue;
//
//             Guid imageId;
//             try
//             {
//                 var image = await new HttpClient().GetStreamAsync(article.ImageUrl);
//                 imageId = Guid.NewGuid();
//                 await storage.Upload(new UploadImageRequest(imageId, image!, ImageSizeEnum.ThumbnailHorizontal), default);
//             }
//             catch (Exception)
//             {
//                 imageId = Guid.Parse("6887b330-7e26-11ed-8afc-ccf04caa7138");
//             }
//             
//             article.ImageUrl = imageId.ToString();
//             context.Update(article);
//             await context.SaveChangesAsync();
//         }
//     }
// }