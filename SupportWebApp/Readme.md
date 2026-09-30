# IBAS Support

Dette er vores lille supportside til IBAS-cykler. Man kan skrive en henvendelse med kontaktoplysninger, kategori og beskrivelse. Henvendelserne bliver gemt i Cosmos DB og vist på en oversigt.

## Sådan køres projektet

Du skal bruge .NET 10 og en Cosmos DB-database. Gem din connection string som user-secret:

```bash
dotnet user-secrets set "ConnectionStrings:CosmosDb" "<connection string>"
dotnet run 
```

Siden åbnes på `http://localhost:5163`. Man opretter en henvendelse på `/support/create` og ser dem på `/support`.

## Sådan oprettede vi databasen

Vi oprettede en resource group i `polandcentral` og derefter en Cosmos DB-konto med free tier. På kontoen lavede vi databasen `IBasSupportDB` og containeren `ibassupport`. Containeren bruger `/category` som partition key.

Vi brugte disse Azure CLI-kommandoer:

```bash
export DBACCOUNT="ibas-db-account"-$RANDOM
export RESGRP="IBasSupportRG"
az group create --name IBasSupportRG --location polandcentral
az cosmosdb create --name $DBACCOUNT --resource-group $RESGRP \
  --enable-free-tier true

export DATABASE="IBasSupportDB"
az cosmosdb sql database create --account-name $DBACCOUNT \
  --resource-group $RESGRP --name $DATABASE

export CONTAINER="ibassupport"
az cosmosdb sql container create --account-name $DBACCOUNT \
  --resource-group $RESGRP --database-name $DATABASE \
  --name $CONTAINER --partition-key-path "/category"
```

Vores konto fik navnet `ibas-db-account-26943`. Database- og containernavnet står i `appsettings.json`, og connection string er gemt som user-secret.

## Status

Vi har lavet formularen, validering, lagring i Cosmos DB og en side, der viser henvendelserne. Vi har testet, at en henvendelse kan gemmes og hentes igen.

Der mangler stadig login og adgangskontrol. Lige nu kan alle, der åbner oversigten, se kontaktoplysningerne. Det næste, vi bør lave, er derfor adgangskontrol. Derefter kunne vi tilføje status på en henvendelse og mulighed for at tildele den til en medarbejder eller forhandler.
