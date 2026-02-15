using System.Globalization;
using System.Security.Claims;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos.BulkNotes;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases.Create;
using UniversiteDomain.UseCases.NoteUseCases.Get;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
using UniversiteDomain.UseCases.SecurityUseCases.Create;
using UniversiteEFDataProvider.Entities;

namespace UniversiteRestApi.Controllers
{
    [Route("api/ue/{idUe:long}/notes/csv")]
    [ApiController]
    public class UeNotesCsvController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> DownloadTemplate(long idUe)
        {
            string role = "";
            string email = "";
            IUniversiteUser user = null!;
            CheckSecu(out role, out email, out user);

            var uc = new GetUeNotesCsvTemplateUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            var rows = await uc.ExecuteAsync(idUe);

            using var memory = new MemoryStream();
            using (var writer = new StreamWriter(memory, leaveOpen: true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(rows);
            }

            memory.Position = 0;
            return File(memory.ToArray(), "text/csv", $"ue_{idUe}_notes_template.csv");
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCsv(long idUe, [FromForm] UploadUeNotesCsvRequest request)
        {
            string role = "";
            string email = "";
            IUniversiteUser user = null!;
            CheckSecu(out role, out email, out user);

            var uc = new ImportUeNotesFromCsvUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            var file = request.File;
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Fichier CSV manquant." });

            List<BulkNoteCsvRowDto> rows;
            using (var stream = file.OpenReadStream())
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                rows = csv.GetRecords<BulkNoteCsvRowDto>().ToList();
            }

            // Résolution étudiant par NumEtud
            async Task<long?> ResolveEtudiantId(string numEtud)
            {
                var etud = await repositoryFactory.EtudiantRepository().GetByNumEtudAsync(numEtud);
                return etud?.Id;
            }

            var (ok, errors) = await uc.ExecuteAsync(idUe, rows, ResolveEtudiantId);

            if (!ok)
                return BadRequest(new { message = "Erreurs dans le CSV : aucune note enregistrée.", errors });

            return Ok(new { message = "Import CSV terminé : notes enregistrées." });
        }

        // Reprise de ta logique de sécurité
        private void CheckSecu(out string role, out string email, out IUniversiteUser user)
        {
            role = "";
            ClaimsPrincipal claims = HttpContext.User;

            if (claims.Identity?.IsAuthenticated != true) throw new UnauthorizedAccessException();
            if (claims.FindFirst(ClaimTypes.Email) == null) throw new UnauthorizedAccessException();
            email = claims.FindFirst(ClaimTypes.Email)!.Value;
            if (string.IsNullOrWhiteSpace(email)) throw new UnauthorizedAccessException();

            user = new FindUniversiteUserByEmailUseCase(repositoryFactory).ExecuteAsync(email).Result;
            if (user == null) throw new UnauthorizedAccessException();

            if (claims.FindFirst(ClaimTypes.Role) == null) throw new UnauthorizedAccessException();
            var ident = claims.Identities.FirstOrDefault();
            if (ident == null) throw new UnauthorizedAccessException();
            role = ident.FindFirst(ClaimTypes.Role)!.Value;
            if (string.IsNullOrWhiteSpace(role)) throw new UnauthorizedAccessException();

            bool isInRole = new IsInRoleUseCase(repositoryFactory).ExecuteAsync(email, role).Result;
            if (!isInRole) throw new UnauthorizedAccessException();
        }
    }
}
