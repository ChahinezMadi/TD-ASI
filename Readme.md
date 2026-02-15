# Fonctionnalité : Saisie en masse des notes d’une UE (CSV)
# binome : Chahinez MADI et Kaouthar Sarah TIBA
# MIAGE M1 2025-2026
## Objectif
Permettre à la scolarité :
1. de télécharger un fichier CSV pré-rempli pour une UE (liste des étudiants + notes existantes),
2. de le compléter,
3. de l’uploader pour enregistrer les notes.

Contrainte : aucune note n’est enregistrée si une erreur est détectée dans le CSV.  
Autorisation : seule la scolarité peut accéder à la fonctionnalité.


## Architecture (Clean Architecture)
- **RestApi** : endpoints HTTP, parsing CSV (CsvHelper), retours HTTP.
- **Domain** : règles métier + use cases + interfaces repositories.
- **EFDataProvider** : requêtes EF Core et transaction d’écriture.

**CsvHelper** est utilisé uniquement côté RestApi pour respecter la séparation des responsabilités.


## Étapes de réalisation
### 1) Domain : DTO CSV
Création de `BulkNoteCsvRowDto` contenant :
- informations UE (NumeroUe, IntituleUe),
- informations étudiant (NumEtud, Nom, Prenom),
- colonne Note (texte nullable).

### 2) Domain : Use case "Télécharger template CSV"
Création de `GetUeNotesCsvTemplateUseCase` :
- autorisation : rôle `Scolarite`,
- récupération des lignes via repository.

### 3) EFDataProvider : génération template
Ajout dans `EtudiantRepository` :
- récupération de l’UE,
- sélection des étudiants dont le parcours enseigne l’UE,
- récupération des notes existantes,
- projection vers `BulkNoteCsvRowDto`.

### 4) Domain : Use case "Importer notes CSV"
Création de `ImportUeNotesFromCsvUseCase` :
- validation de toutes les lignes :
    - NumEtud présent,
    - étudiant existant,
    - note vide ou nombre compris entre 0 et 20,
- si erreurs : retour liste d’erreurs, aucune écriture en base,
- sinon : appel repository `ApplyNotesForUeAsync`.

### 5) EFDataProvider : application transactionnelle
`ApplyNotesForUeAsync` :
- exécution sous transaction (`BeginTransactionAsync`),
- pour chaque ligne :
    - création ou mise à jour d’une note,
    - (choix métier) suppression si note vide,
- commit si tout est OK, rollback sinon.

### 6) RestApi : endpoints CSV
Ajout de deux endpoints protégés :
- `GET api/ue/{idUe}/notes/csv` : téléchargement du template.
- `POST api/ue/{idUe}/notes/csv` : upload CSV.

Parsing CSV réalisé avec **CsvHelper**.
Si erreurs : HTTP 400 + liste des erreurs.
Si OK : HTTP 200 et confirmation.

## Tests réalisés
- téléchargement template pour UE existante,
- upload CSV vide → erreur,
- upload CSV avec note hors bornes (>20) → erreur + aucune écriture,
- upload CSV correct → notes insérées/mises à jour.
