using Challenge_Clyvo_NET.Data;
using Challenge_Clyvo_NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Challenge_Clyvo_NET.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContatoController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public ContatoController(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await dbContext.Contatos
                .Include(c => c.Pessoa)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contato = await dbContext.Contatos
                .Include(c => c.Pessoa)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contato == null)
                return NotFound();

            return Ok(contato);
        }

        [HttpGet("numero/{numero}")]
        public async Task<IActionResult> GetByNumero(string numero)
        {
            var contatos = await dbContext.Contatos
                .Include(c => c.Pessoa)
                .Where(c => c.Numero.Contains(numero))
                .ToListAsync();

            if (contatos == null)
                return NotFound();

            return Ok(contatos);

            
        }

        [HttpGet("pessoa/{pessoaId}")]
        public async Task<IActionResult> GetByPessoa(int pessoaId)
        {
            var contatos = (await dbContext.Contatos
                .Include(c => c.Pessoa)
                .Where(c => c.PessoaId == pessoaId)
                .ToListAsync());

            if (contatos == null)
                return NotFound();

            return Ok(contatos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Contato contatoToSave)
        {
            var contato = new Contato(
                contatoToSave.Numero,
                contatoToSave.Email,
                contatoToSave.PessoaId
            );

            dbContext.Contatos.Add(contato);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = contato.Id },
                contato
            );
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, Contato contatoUpdate)
        {
            var contato = await dbContext.Contatos.FindAsync(id);

            if (contato == null)
                return NotFound();

            contato.Update(
                contatoUpdate.Numero,
                contatoUpdate.Email
            );

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var contato = await dbContext.Contatos.FindAsync(id);

            if (contato == null)
                return NotFound();

            dbContext.Contatos.Remove(contato);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}