using System;
using System.Linq;
using System.Threading.Tasks;
using LinkVault.Api.Dtos;
using LinkVault.Context;
using LinkVault.Models;
using LinkVault.Services;
using LinkVault.Models;
using Microsoft.AspNetCore.Mvc;

namespace LinkVault.Api.Controllers
{
    [Route("links")]
    [ApiController]
    public class LinksController : ControllerBase
    {

        private AppDbContext Context { get; }

        private MessageBusService MessageBusService { get; }

        public LinksController(AppDbContext context, MessageBusService messageBusService)
        {
            Context = context;
            MessageBusService = messageBusService;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] GetLinksDto getLinksDto)
        {
            Func<Link, bool> filterFunc = link =>
            {
                if (getLinksDto.CollectionId is null)
                    return true;

                return link.CollectionId == getLinksDto.CollectionId;
            };
            var links = Context.Links.Where(filterFunc).ToList().Select(x => x.AsDto());

            return Ok(links);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync(int id)
        {
            var link = await Context.Links.FindAsync(id);

            if (link is null)
                return NotFound();

            return Ok(link.AsDto());
        }

        [HttpPost]
        public async Task<ActionResult<LinkDto>> PostAsync(CreateLinkDto createLinkDto)
        {
            var link = new Link
            {
                Title = createLinkDto.Title!,
                URL = createLinkDto.URL!,
                Description = createLinkDto.Description!,
                CollectionId = createLinkDto.CollectionId
            };

            var res = await Context.Links.AddAsync(link);
            await Context.SaveChangesAsync();
            link = res.Entity;

            MessageBusService.Emit("LinkCreated", link);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = link.Id }, link.AsDto());
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<LinkDto>> PutAsync(int id, UpdateLinkDto updateLinkDto)
        {
            var link = await Context.Links.FindAsync(id);

            if (link is null)
                return NotFound();


            // Update dto is a partial of CreateLinkDto
            link.Title = updateLinkDto.Title ?? link.Title;
            link.Description = updateLinkDto.Description ?? link.Description;
            link.CollectionId = updateLinkDto.CollectionId ?? link.CollectionId;
            link.URL = updateLinkDto.URL ?? link.URL;
            link.UpdatedAt = DateTime.Now;

            Context.Update(link);
            await Context.SaveChangesAsync();

            MessageBusService.Emit("LinkUpdated", link);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = link.Id }, link.AsDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var link = await Context.Links.FindAsync(id);

            if (link is null)
                return NotFound();

            Context.Links.Remove(link);
            await Context.SaveChangesAsync();

            MessageBusService.Emit("LinkDeleted", id);
            return Ok();
        }
    }
}