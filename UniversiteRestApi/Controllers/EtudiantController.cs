using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteApplication.UseCases.EtudiantUseCases.Get;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EtudiantController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<EtudiantDto>> GetUnEtudiant(string id)
        {
            var getEtudiantUc = new GetEtudiantByNumUseCase(repositoryFactory);
            var etudiant = await getEtudiantUc.ExecuteAsync(id);

            if (etudiant == null)
                return NotFound();

            return Ok(new EtudiantDto().ToDto(etudiant));
        }

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

            //on renvoie l’URL /api/Etudiant/{NumEtud}
            return CreatedAtAction(nameof(GetUnEtudiant), new { id = dto.NumEtud }, dto);
        }
    }
}