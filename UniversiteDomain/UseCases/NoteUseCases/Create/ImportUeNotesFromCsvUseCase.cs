using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos.BulkNotes;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Create;

public class ImportUeNotesFromCsvUseCase(IRepositoryFactory factory)
{
    public bool IsAuthorized(string role) => role.Equals(Roles.Scolarite);

    public async Task<(bool ok, List<string> errors)> ExecuteAsync(
        long idUe,
        List<BulkNoteCsvRowDto> rows,
        Func<string, Task<long?>> resolveEtudiantIdByNumEtud // fourni par repo/service
    )
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(rows);

        var errors = new List<string>();

        // Validation métier : notes entre 0 et 20 ou vide
        // + cohérence UE (NumeroUe/Intitulé) gérée côté API ou repo (voir plus bas)
        var parsedNotes = new List<(long etudiantId, decimal? note)>();

        for (int i = 0; i < rows.Count; i++)
        {
            var r = rows[i];
            var line = i + 2; // si ligne 1 = header CSV

            if (string.IsNullOrWhiteSpace(r.NumEtud))
            {
                errors.Add($"Ligne {line}: NumEtud manquant.");
                continue;
            }

            long? idEtud = await resolveEtudiantIdByNumEtud(r.NumEtud);
            if (idEtud == null)
            {
                errors.Add($"Ligne {line}: étudiant inconnu (NumEtud={r.NumEtud}).");
                continue;
            }

            decimal? noteValue = null;
            if (!string.IsNullOrWhiteSpace(r.Note))
            {
                if (!decimal.TryParse(r.Note.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var parsed))
                {
                    errors.Add($"Ligne {line}: note invalide '{r.Note}'.");
                    continue;
                }

                if (parsed < 0 || parsed > 20)
                {
                    errors.Add($"Ligne {line}: note hors bornes (0..20) : {parsed}.");
                    continue;
                }

                noteValue = parsed;
            }

            parsedNotes.Add((idEtud.Value, noteValue));
        }

        if (errors.Count > 0)
            return (false, errors);

        // Application en base (transactionnel) : uniquement si aucune erreur
        await factory.EtudiantRepository().ApplyNotesForUeAsync(idUe, parsedNotes);
        return (true, errors);
    }
}
