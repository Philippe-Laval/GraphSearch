Projet: ITSM.ApiService.Client

But: bibliothèque cliente C# générée à partir de ITSM.ApiService/ITSM.ApiService.json (OpenAPI).

Utilisation:
- Ouvrir un terminal PowerShell dans le dossier ITSM.ApiService.Client
- Installer NSwag si nécessaire:
  dotnet tool install --global NSwag.ConsoleCore
- Lancer la génération:
  .\generate-client.ps1

pwsh -NoProfile -ExecutionPolicy Bypass -File .\generate-client.ps1

Note: le client est maintenant généré pour utiliser System.Text.Json (option /JsonLibrary:SystemTextJson).

Intégration:
- Ajouter une référence projet dans ITSM.Web.Client vers ITSM.ApiService.Client
- Enregistrer les clients générés dans DI (AddHttpClient / typed clients)

Remarque: le script suppose que vos DTOs sont définis dans ITSM.ApiService.Contracts et que la génération n'écrase pas ces types (option /GenerateDtoTypes:false).