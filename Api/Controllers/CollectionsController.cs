using System;
using System.Linq;
using System.Threading.Tasks;
using LinkVault.Api.Dtos;
using LinkVault.Context;
using LinkVault.Models;
using Microsoft.AspNetCore.Mvc;

namespace LinkVault.Api.Controllers
{


    [Route("/collections")]
    [ApiController]
    public class CollectionsController : ControllerBase
    {
        private AppDbContext Context { get; }

        public CollectionsController(AppDbContext context)
        {
            Context = context;
        }

        [HttpGet]
        public IActionResult Search([FromQuery] GetColsDto getColsDto)
        {
            Func<LinkCollection, bool> filterFunc = col =>
            {
                if (string.IsNullOrWhiteSpace(getColsDto.Keyword))
                    return true;

                return col.Name.Contains(getColsDto.Keyword, StringComparison.InvariantCultureIgnoreCase);
            };
            var collections = Context.Collections.Where(filterFunc).ToList();

            return Ok(collections);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var collection = await Context.Collections.FindAsync(id);

            if (collection is null)
                return NotFound();

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<LinkCollection>> CreateAsync(CreateColDto createColDto)
        {
            var response = await Context.Collections.AddAsync(new LinkCollection
            {
                Name = createColDto.Name!
            });
            await Context.SaveChangesAsync();

            var collection = response.Entity;


            return CreatedAtAction(nameof(GetByIdAsync), new { id = collection.Id }, collection);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LinkCollection>> UpdateAsync(int id, CreateColDto createColDto)
        {
            var collection = await Context.Collections.FindAsync(id);

            if (collection is null)
            {
                return NotFound();
            }

            collection.Name = createColDto.Name!;
            collection.UpdatedAt = DateTime.Now;
            Context.Collections.Update(collection);

            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByIdAsync), new { id = collection.Id }, collection);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var collection = await Context.Collections.FindAsync(id);

            if (collection is null)
                return NotFound();

            Context.Collections.Remove(collection);
            await Context.SaveChangesAsync();

            return Ok();
        }
    }
}