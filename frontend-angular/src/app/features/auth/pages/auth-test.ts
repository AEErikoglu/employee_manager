import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../../../shared/services/auth.service';

@Component({
  selector: 'app-auth-test',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatButtonModule, MatCardModule],
  styles: `
    :host {
      display: grid;
      min-height: 100dvh;
      place-items: center;
      padding: 1.5rem;
      background: #f8fafc;
    }
  `,
  template: `
    <mat-card class="w-full max-w-lg border border-emerald-200 p-8 shadow-lg shadow-emerald-100">
      <p class="text-sm font-semibold uppercase tracking-[0.18em] text-emerald-700">Auth-Test</p>
      <h1 class="mt-3 text-3xl font-semibold tracking-tight text-slate-950">Anmeldung erfolgreich</h1>
      <p class="mt-3 text-slate-600">
        Die aktive Supabase-Session gehoert zu
        <strong class="font-semibold text-slate-950">{{ authService.user()?.email }}</strong>.
      </p>
      <p class="mt-2 text-sm text-slate-500">
        Die Route ist durch den Auth Guard geschuetzt und API-Anfragen erhalten automatisch den
        Access-Token.
      </p>
      <button mat-stroked-button type="button" class="mt-7" [disabled]="isSigningOut()" (click)="signOut()">
        Abmelden
      </button>
    </mat-card>
  `,
})
export default class AuthTestComponent {
  protected readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  protected readonly isSigningOut = signal(false);

  protected async signOut(): Promise<void> {
    this.isSigningOut.set(true);

    try {
      await firstValueFrom(this.authService.signOut());
      await this.router.navigateByUrl('/login');
    } finally {
      this.isSigningOut.set(false);
    }
  }
}
