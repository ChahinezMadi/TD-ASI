using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtudiantController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        // GET api/Etudiant/5
        [HttpGet("{id:long}")]
        public async Task<ActionResult<EtudiantDto>> GetUnEtudiant(long id)
        {
            var repo = repositoryFactory.EtudiantRepository();
            var etudiant = await repo.FindAsync(id);

            if (etudiant == null)
                return NotFound();

            return Ok(new EtudiantDto().ToDto(etudiant));
        }

        //POST api/Etudiant
        [HttpPost]
        public async Task<ActionResult<EtudiantDto>> PostAsync([FromBody] EtudiantDto etudiantDto)
        {
            var createEtudiantUc = new CreateEtudiantUseCase(repositoryFactory);
            Etudiant etud = etudiantDto.ToEntity();

            try
            {
                etud = await createEtudiantUc.ExecuteAsync(etud);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(nameof(e), e.Message);
                return ValidationProblem();
            }

            EtudiantDto dto = new EtudiantDto().ToDto(etud);

            // GetUnEtudiant existe + route cohérente (id long)
            return CreatedAtAction(nameof(GetUnEtudiant), new { id = dto.Id }, dto);
        }
    }
}