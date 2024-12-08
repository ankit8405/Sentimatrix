// using Microsoft.AspNetCore.Mvc;

// using MongoDB.Driver;

// public class YourController : ControllerBase
// {
//     private readonly MongoDbContext _mongoDbContext;

//     public YourController(MongoDbContext mongoDbContext)
//     {
//         _mongoDbContext = mongoDbContext;
//     }

//     [HttpGet]
//     public async Task<IActionResult> GetItems()
//     {
//         var items = await _mongoDbContext.GetCollection<Item>("items").Find(_ => true).ToListAsync();
//         return Ok(items);
//     }

//     [HttpPost]
//     public async Task<IActionResult> CreateItem(Item item)
//     {
//         await _mongoDbContext.GetCollection<Item>("items").InsertOneAsync(item);
//         return CreatedAtAction(nameof(GetItems), item);
//     }
// }