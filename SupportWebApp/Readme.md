# IBAS Support

Dette er vores lille supportside til IBAS-cykler. Man kan skrive en henvendelse med kontaktoplysninger, kategori og beskrivelse. Henvendelserne bliver gemt i Cosmos DB og vist på en oversigt.

## Sådan køres projektet

Du skal bruge .NET 10 og en Cosmos DB-database. Gem din connection string som user-secret:

```bash
dotnet user-secrets set "ConnectionStrings:CosmosDb" "<connection string>"
dotnet run 
```

Siden åbnes på `http://localhost:5163`. Man opretter en henvendelse på `/support/create` og ser dem på `/support`.

## Opret databasen med Azure CLI

Ret variablerne, især kontonavnet, som skal være unikt. Disse kommandoer opretter Azure-ressourcer, der kan koste penge.

```bash
az login

RESOURCE_GROUP="ibas-support-rg"
LOCATION="polandcentral"
COSMOS_ACCOUNT="<unikt-kontonavn>"

az group create -n "$RESOURCE_GROUP" -l "$LOCATION"
az cosmosdb create -n "$COSMOS_ACCOUNT" -g "$RESOURCE_GROUP" \
  --locations regionName="$LOCATION" failoverPriority=0 isZoneRedundant=False
az cosmosdb sql database create -a "$COSMOS_ACCOUNT" -g "$RESOURCE_GROUP" \
  -n IBasSupportDB
az cosmosdb sql container create -a "$COSMOS_ACCOUNT" -g "$RESOURCE_GROUP" \
  -d IBasSupportDB -n ibassupport --partition-key-path "/category" --throughput 400
```

Connection string kan findes med `az cosmosdb keys list -n "$COSMOS_ACCOUNT" -g "$RESOURCE_GROUP" --type connection-strings`. Den skal gemmes som user-secret med kommandoen ovenfor. Database- og containernavnet i `appsettings.json` skal passe med navnene i kommandoerne. Partition key skal være `/category`.

## Status

Vi har lavet formularen, validering, lagring i Cosmos DB og en side, der viser henvendelserne. Vi har testet, at en henvendelse kan gemmes og hentes igen.

Der mangler stadig login og adgangskontrol. Lige nu kan alle, der åbner oversigten, se kontaktoplysningerne. Det næste, vi bør lave, er derfor adgangskontrol. Derefter kunne vi tilføje status på en henvendelse og mulighed for at tildele den til en medarbejder eller forhandler.
