using Challenge_Clyvo_NET.Data;
using Challenge_Clyvo_NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Challenge_Clyvo_NET.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultaController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public ConsultaController(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var consultas = await dbContext.Consultas
                .Include(c => c.Animal)
                .ThenInclude(a => a.Responsavel)
                .ThenInclude(r => r.Pessoa)
                .Include(c => c.Veterinario)
                .ThenInclude(v => v.Pessoa).ToListAsync();

            return Ok(consultas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var consulta = await dbContext.Consultas
                .Include(c => c.Animal)
                    .ThenInclude(a => a.Responsavel)
                        .ThenInclude(r => r.Pessoa)
                .Include(c => c.Veterinario)
                    .ThenInclude(v => v.Pessoa)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consulta == null)
                return NotFound();

            return Ok(consulta);
        }

        [HttpGet("animal/{animalId}")]
        public async Task<IActionResult> GetByAnimal(int animalId)
        {
            var consultas = await dbContext.Consultas
                .Include(c => c.Animal)
                    .ThenInclude(a => a.Responsavel)
                        .ThenInclude(r => r.Pessoa)
                .Include(c => c.Veterinario)
                    .ThenInclude(v => v.Pessoa)
                .Where(c => c.AnimalId == animalId)
                .ToListAsync();

            if (consultas == null)
                return NotFound();

            return Ok(consultas);
        }

        [HttpGet("veterinario/{veterinarioId}")]
        public async Task<IActionResult> GetByVeterinario(int veterinarioId)
        {
            var consultas = await dbContext.Consultas
                .Include(c => c.Animal)
                    .ThenInclude(a => a.Responsavel)
                        .ThenInclude(r => r.Pessoa)
                .Include(c => c.Veterinario)
                    .ThenInclude(v => v.Pessoa)
                .Where(c => c.VeterinarioId == veterinarioId)
                .ToListAsync();

            if (consultas == null)
                return NotFound();

            return Ok(consultas);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Consulta consultaToSave)
        {
            var consulta = new Consulta(consultaToSave.DataAgendamento, consultaToSave.DataConsulta, consultaToSave.AnimalId, consultaToSave.VeterinarioId);

            dbContext.Consultas.Add(consulta);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = consulta.Id }, consulta);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, Consulta novaConsulta)
        {
            var consulta = await dbContext.Consultas.FindAsync(id);

            if (consulta == null)
                return NotFound();

            consulta.Update(novaConsulta.DataAgendamento, novaConsulta.DataConsulta, novaConsulta.AnimalId, novaConsulta.VeterinarioId);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var consulta = await dbContext.Consultas.FindAsync(id);

            if (consulta == null)
                return NotFound();

            dbContext.Consultas.Remove(consulta);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
