using Challenge_Clyvo_NET.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Challenge_Clyvo_NET.Models;

namespace Challenge_Clyvo_NET.Controllers
{
    [ApiController]
    [Route("api/pessoas")]
    public class PessoaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PessoaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
            => Ok(await _context.Pessoas.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);

            if (pessoa == null)
                return NotFound();

            return Ok(pessoa);
        }

        [HttpGet("cpf/{cpf}")]
        public async Task<IActionResult> GetByCpf(string cpf)
        {
            var pessoa = await _context.Pessoas
                .FirstOrDefaultAsync(x => x.Cpf == cpf);

            if (pessoa == null)
                return NotFound();

            return Ok(pessoa);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Pessoa pessoa)
        {
            _context.Pessoas.Add(pessoa);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = pessoa.Id },
                pessoa
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Pessoa novaPessoa)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);

            if (pessoa == null)
                return NotFound();

            pessoa.Update(
                novaPessoa.Nome,
                novaPessoa.Cpf,
                novaPessoa.DataNascimento
            );

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);

            if (pessoa == null)
                return NotFound();

            _context.Pessoas.Remove(pessoa);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}