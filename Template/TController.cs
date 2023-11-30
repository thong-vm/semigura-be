using Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Template
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class TController<TEntity, TRepository> : ControllerBase
        where TEntity : class, IEntity
        where TRepository : IRepository<TEntity>
    {
        private readonly TRepository repository;

        public TController(TRepository repository)
        {
            this.repository = repository;
        }


        // GET: api/[controller]
        [HttpGet]
        public IQueryable<TEntity> Get()
        {
            return repository.GetAll();
        }

        // GET: api/[controller]/5
        [HttpGet("{id}")]
        public IQueryable<TEntity> Get(string id)
        {
            return repository.Get(id);
        }

        // PUT: api/[controller]/5
        [HttpPut("{id}")]
        public async Task<ActionResult<TEntity>> Put(string id, TEntity obj)
        {
            if (id != obj.Id)
            {
                return NotFound();
            }
            await repository.Update(obj);
            return obj;
        }

        // POST: api/[controller]
        [HttpPost]
        public async Task<ActionResult<TEntity>> Post(TEntity obj)
        {
            await repository.Add(obj);
            return CreatedAtAction("Get", new { id = obj.Id }, obj);
        }

        // DELETE: api/[controller]/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TEntity>> Delete(string id)
        {
            var obj = await repository.Delete(id);
            if (obj == null)
            {
                return NotFound();
            }
            return obj;
        }

    }
}
