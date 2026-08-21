# Supabase Auth Setup

1. Create a Supabase project and enable the desired sign-in providers under **Authentication**.
2. Add `http://localhost:4200` and the production frontend URL to **Authentication > URL Configuration > Redirect URLs**.
3. Copy the project URL and publishable key from **Connect** into `frontend-angular/src/environments/environment.ts`.
4. Configure the backend URL without committing it:

```powershell
dotnet user-secrets set "Supabase:Url" "https://your-project-ref.supabase.co" --project backend
```

The frontend sends the user's Supabase access token as a Bearer token. The backend validates it against Supabase's OpenID Connect metadata and protects every existing API group. Use Supabase asymmetric signing keys for local JWT validation.
