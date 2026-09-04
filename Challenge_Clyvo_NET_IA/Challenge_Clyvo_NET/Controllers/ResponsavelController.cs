using Challenge_Clyvo_NET.Data;
using Challenge_Clyvo_NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Challenge_Clyvo_NET.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponsavelController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public ResponsavelController(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await dbContext.Responsaveis
                .Include(r => r.Pessoa)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var responsavel = await dbContext.Responsaveis
                .Include(r => r.Pessoa)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (responsavel == null)
                return NotFound();

            return Ok(responsavel);
        }

        [HttpGet("endereco/{endereco}")]
        public async Task<IActionResult> GetByEndereco(string endereco)
        {

            var responsaveis = await dbContext.Responsaveis
                .Include(r => r.Pessoa)
                .Where(r => r.Endereco.Contains(endereco))
                .ToListAsync();

            if (responsaveis == null)
                return NotFound();

            return Ok(responsaveis);
        }

        [HttpGet("pessoa/{pessoaId}")]
        public async Task<IActionResult> GetByPessoa(int pessoaId)
        {
            var responsaveis = await dbContext.Responsaveis
                .Include(r => r.Pessoa)
                .Where(r => r.PessoaId == pessoaId)
                .ToListAsync();

            if (responsaveis == null)
                return NotFound();

            return Ok(responsaveis);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Responsavel responsavelToSave)
        {
            var responsavel = new Responsavel(responsavelToSave.Endereco, responsavelToSave.PessoaId);

            dbContext.Responsaveis.Add(responsavel);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = responsavel.Id }, responsavel);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, Responsavel novoResponsavel)
        {
            var responsavel = await dbContext.Responsaveis.FindAsync(id);

            if (responsavel == null)
                return NotFound();

            responsavel.Update(novoResponsavel.Endereco, novoResponsavel.PessoaId);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var responsavel = await dbContext.Responsaveis.FindAsync(id);

            if (responsavel == null)
                return NotFound();

            dbContext.Responsaveis.Remove(responsavel);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}