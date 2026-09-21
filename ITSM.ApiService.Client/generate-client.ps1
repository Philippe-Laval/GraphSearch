Param()

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$repoRoot = Resolve-Path "$scriptDir\.." | Select-Object -ExpandProperty Path
$openApi = "C:\CodeGithubPerso\GraphSearch\ITSM.ApiService\ITSM.ApiService.json"
$output = Join-Path $scriptDir "Generated\ITSM_ApiService_Client.cs"

if (-not (Test-Path $openApi)) {
	Write-Error "Fichier OpenAPI introuvable: $openApi"
	exit 1
}

# Assure que le dossier de sortie existe
New-Item -ItemType Directory -Force -Path (Split-Path $output) | Out-Null

Write-Host "Génération du client NSwag à partir de: $openApi"
Write-Host "Sortie: $output"

# Utilise l'outil global NSwag si présent, sinon essaie via dotnet tool run
if (Get-Command nswag -ErrorAction SilentlyContinue) {
	nswag openapi2csclient /input:$openApi /output:$output /namespace:ITSM.ApiService.Client /GenerateClientInterfaces:true /GenerateDtoTypes:false /UseHttpRequestMessageCreationMethod:false /JsonLibrary:SystemTextJson
} else {
	Write-Host "Commande 'nswag' non trouvée. Tentative via 'dotnet tool run nswag' (nécessite un manifest de tool local)."
	dotnet tool run nswag openapi2csclient /input:$openApi /output:$output /namespace:ITSM.ApiService.Client /GenerateClientInterfaces:true /GenerateDtoTypes:false /UseHttpRequestMessageCreationMethod:false /JsonLibrary:SystemTextJson
}

if ($LASTEXITCODE -ne 0) {
	Write-Error "La génération NSwag a échoué (code $LASTEXITCODE)."
	exit $LASTEXITCODE
}

Write-Host "Génération terminée." 
