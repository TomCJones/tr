using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using tr.Models;
using tr.Data;
using static tr.Data.Base;
using CredStore;
using Microsoft.Extensions.Logging;

namespace tr.Controllers
{
    [ApiController]
    [Route("tr")]
    public class SubjectsController : ControllerBase
    {

        private readonly ICredStoreSvc _kss;
        private readonly SubjectDbContext _context;
        private readonly ILogger _logger;
        
        public SubjectsController (
            SubjectDbContext dbContext,
            ILogger<SubjectsController> logger,
            ICredStoreSvc kss)
        {
            _kss = kss;
            _logger = logger;
            _context = dbContext;
        }
        /*
        // GET: api/Subjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subject>>> GetSubjects()
        {
            return await _context.Subjects.ToListAsync();
        }
        
        // GET: api/Subjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> GetSubject(long id)
        {
            var subject = await _context.Subjects.FindAsync(id);

            if (subject == null)
            {
                return NotFound();
            }

            return subject;
        }
        
        // PUT: api/Subjects/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubject(long id, Subject subject)
        {
            if (id != subject.Id)
            {
                return BadRequest();
            }

            _context.Entry(subject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        */
        // POST: tr
        // To protect from overposting attacks, enable the specific properties you want to bind to, for more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<string>> Post()
        {
            string jsonOut = "{err=not_recognized}";
            string respBody = await new StreamReader(Request.Body).ReadToEndAsync();
            string contentType = Request.ContentType;
            StringValues userAgent;
            Request.Headers.TryGetValue("User-Agent", out userAgent);

            JsonError jerr = new JsonError
            {
                operation = "fetch csp",
                error = "invalid request",
                error_description = "must be at least 3 parts to a jose request"
            };

            string[] splitBody = respBody.Split('.');
            if (splitBody.Length < 3)
            {
                string jeOut = JsonSerializer.Serialize(jerr);
                CredentialDocResult cdr1 = new CredentialDocResult("{\"alg\": \"RS256\"}", jeOut, 0);
                return (await cdr1.SignCDR(_kss))[0];
            }

            string json = "{ }";

            return CreatedAtAction("Post",jsonOut);
        }
        /*
        // DELETE: api/Subjects/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Subject>> DeleteSubject(long id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return subject;
        }

        private bool SubjectExists(long id)
        {
            return _context.Subjects.Any(e => e.Id == id);
        }
        */
    }
}
