## Dev Development Tipps
- Use minimal api
- Store code in its related folder and file
- Example: store endpoints in /Endpoints folder 
Store Workspace-Related endpoints in its own file such as in WorkspaceEndpoints.cs
- Save secret information in Secrets.json 
- Use Entity Framework Core for PostgreSQL from NpgSql
- Do not use Migrations, work with the local database its ConnectionString is already stored in Secrets.json
- Add documentation comments for new Implementations
- use Scalar for debug and documentational purposes 
- Work with Interfaces and Services, Don't implement the actual logic in Endpoint
- Use DTOs for CRUD-Oprations
- Define all te DTOs for one entity in one single file