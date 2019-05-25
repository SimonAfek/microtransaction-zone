# microtransaction-zone
microtransaction-zone is the source code repository for the website MICROTRANSACTION.ZONE.

# Building

The project was created in Visual Studio 2017, though it should also work just fine in VS2019.

You will need to rename ***AppSecrets_Dummy.config*** to ***AppSecrets.config*** and ***ConnectionStrings_Dummy.config*** to ***ConnectionStrings.config***.

API keys for GiantBomb and ReCaptcha go in ***AppSecrets.config***, database connection string goes in ***ConnectionStrings.config*** (you gotta place it in there in both of the specified spots).

# Database

The database is MSSQL. Any moderately recent version should work just fine.

The two scripts ***1 - schema.sql*** and ***2 - data.sql***, located in the ***DBSetup*** folder should be sufficient to populate the database with everything it needs to run.
