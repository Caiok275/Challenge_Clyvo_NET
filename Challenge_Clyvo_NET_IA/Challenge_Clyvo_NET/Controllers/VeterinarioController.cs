using Challenge_Clyvo_NET.Data;
using Challenge_Clyvo_NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Challenge_Clyvo_NET.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeterinarioController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public VeterinarioController(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await dbContext.Veterinarios
                .Include(v => v.Pessoa)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var veterinario = await dbContext.Veterinarios
                .Include(v => v.Pessoa)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veterinario == null)
                return NotFound();

            return Ok(veterinario);
        }

        [HttpGet("especialidade/{especialidade}")]
        public async Task<IActionResult> GetByEspecialidade(string especialidade)
        {
            var veterinarios = await dbContext.Veterinarios
                .Include(v => v.Pessoa)
                .Where(v => v.Especialidade == especialidade)
                .ToListAsync();

            if (veterinarios == null)
                return NotFound();

            return Ok(veterinarios);
        }

        [HttpGet("pessoa/{pessoaId}")]
        public async Task<IActionResult> GetByPessoa(int pessoaId)
        {

            var veterinarios = await dbContext.Veterinarios
                .Include(v => v.Pessoa)
                .Where(v => v.PessoaId == pessoaId)
                .ToListAsync();

            if (veterinarios == null)
                return NotFound();

            return Ok(veterinarios);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Veterinario veterinarioToSave)
        {
            var veterinario = new Veterinario(veterinarioToSave.Especialidade, veterinarioToSave.PessoaId);

            dbContext.Veterinarios.Add(veterinario);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = veterinario.Id }, veterinario);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, Veterinario novoVeterinario)
        {
            var veterinario = await dbContext.Veterinarios.FindAsync(id);

            if (veterinario == null)
                return NotFound();

            veterinario.Update(novoVeterinario.Especialidade, novoVeterinario.PessoaId);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var veterinario = await dbContext.Veterinarios.FindAsync(id);

            if (veterinario == null)
                return NotFound();

            dbContext.Veterinarios.Remove(veterinario);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}