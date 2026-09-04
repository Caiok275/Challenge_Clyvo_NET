
using Challenge_Clyvo_NET.Data;
using Challenge_Clyvo_NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Challenge_Clyvo_NET.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimaisController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public AnimaisController(AppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var animais = await dbContext.Animais
                .Include(a => a.Responsavel)
                .ThenInclude(r => r.Pessoa)
                .ToListAsync();

            return Ok(animais);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var animal = await dbContext.Animais
                .Include(a => a.Responsavel)
                .ThenInclude(r => r.Pessoa)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (animal == null)
                return NotFound();

            return Ok(animal);
        }

        [HttpGet("especie/{especie}")]
        public async Task<IActionResult> GetByEspecie(string especie)
        {
            var animais = await dbContext.Animais
                .Include(a => a.Responsavel)
                .ThenInclude(r => r.Pessoa)
                .Where(a => a.Especie == especie)
                .ToListAsync();

            if (animais == null)
                return NotFound();

            return Ok(animais);
        }

        [HttpGet("raca/{raca}")]
        public async Task<IActionResult> GetByRaca(string raca)
        {
            var animais = await dbContext.Animais
                .Include(a => a.Responsavel)
                .ThenInclude(r => r.Pessoa)
                .Where(a => a.Raca == raca)
                .ToListAsync();

            if (animais == null)
                return NotFound();

            return Ok(animais);
        }

        [HttpGet("responsavel/{id}")]
        public async Task<IActionResult> GetByResponsavel(int id)
        {
            var animais = await dbContext.Animais
                .Include(a => a.Responsavel)
                .ThenInclude(r => r.Pessoa)
                .Where(a => a.ResponsavelId == id)
                .ToListAsync();

            if (animais == null)
                return NotFound();


            return Ok(animais);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Animal animalToSave)
        {
            var animal = new Animal(animalToSave.Nome, animalToSave.Idade, animalToSave.Especie, animalToSave.Raca, animalToSave.Sexo, animalToSave.DataNascimento, animalToSave.Peso, animalToSave.ResponsavelId);

            dbContext.Animais.Add(animal);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = animal.Id },
                animal
            );
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, Animal novoAnimal)
        {
            var animal = await dbContext.Animais.FindAsync(id);

            if (animal == null)
            {
                return NotFound();
            }

            animal.Update(novoAnimal.Nome, novoAnimal.Idade, novoAnimal.Especie, novoAnimal.Raca, novoAnimal.Sexo, novoAnimal.DataNascimento, novoAnimal.Peso, novoAnimal.ResponsavelId);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var animal = await dbContext.Animais.FindAsync(id);

            if (animal == null)
            {
                return NotFound();
            }

            dbContext.Animais.Remove(animal);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}