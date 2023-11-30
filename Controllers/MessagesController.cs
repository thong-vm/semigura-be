using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using semigura.DAL;
using System.Data;
using System.Net;

namespace semigura.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private IMessageRepository _repository;

        public MessagesController(IMessageRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]

        // GET: api/Messages
        public IEnumerable<Message> GetMessage()
        {
            return _repository.GetAll().OrderByDescending(p => p.CreatedDate).Take(5);
        }

        [HttpGet("{limit?}")]
        public List<Message> GetMessage(int limit)
        {
            return _repository.GetAll().OrderByDescending(p => p.CreatedDate).Take(limit).ToList();
        }

        // GET: api/Messages/5
        [HttpGet("{id?}")]
        public async Task<IActionResult> GetMessage(string id)
        {
            Message message = await _repository.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        // PUT: api/Messages/5
        [HttpPut("{id?}/{message?}")]
        public async Task<IActionResult> PutMessage(string id, Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != message.Id)
            {
                return BadRequest();
            }

            _repository.SetModified(message);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if (!MessageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        // POST: api/Messages
        [HttpPost("{message?}")]
        public async Task<IActionResult> PostMessage(Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            message.Id = Guid.NewGuid().ToString("N");
            _repository.Add(message);

            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (MessageExists(message.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = message.Id }, message);
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id?}")]
        public async Task<IActionResult> DeleteMessage(string id)
        {
            Message message = await _repository.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _repository.Remove(message);
            await _repository.SaveChangesAsync();

            return Ok(message);
        }

        private bool MessageExists(string id)
        {
            return _repository.Exists(id);
        }
    }
}