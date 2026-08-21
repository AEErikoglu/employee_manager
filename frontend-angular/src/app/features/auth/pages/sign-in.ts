import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FormField, email, form, required } from '@angular/forms/signals';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../../../shared/services/auth.service';

interface SignInCredentials {
  readonly email: string;
  readonly password: string;
}

@Component({
  selector: 'app-sign-in',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    FormField,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    RouterLink,
  ],
  styles: `
    :host {
      display: grid;
      min-height: 100dvh;
      place-items: center;
      padding: 1.5rem;
      background:
        radial-gradient(circle at 15% 15%, rgb(186 230 253 / 0.75), transparent 35%),
        radial-gradient(circle at 85% 80%, rgb(167 243 208 / 0.7), transparent 35%),
        #f8fafc;
    }
  `,
  template: `
    <mat-card class="w-full max-w-md overflow-hidden border border-slate-200 shadow-xl shadow-slate-300/40">
      <div class="border-b border-slate-100 bg-slate-950 px-7 py-8 text-white">
        <p class="text-xs font-semibold tracking-[0.22em] text-cyan-300">EMPLOYEE MANAGER</p>
        <h1 class="mt-3 text-3xl font-semibold tracking-tight">Willkommen zuruck</h1>
        <p class="mt-2 text-sm text-slate-300">Melde dich mit einem Supabase-Benutzerkonto an.</p>
      </div>

      <form class="space-y-4 p-7" (submit)="signIn($event)">
        <mat-form-field class="w-full" appearance="outline">
          <mat-label>E-Mail</mat-label>
          <input matInput type="email" autocomplete="email" [formField]="signInForm.email" />
        </mat-form-field>

        <mat-form-field class="w-full" appearance="outline">
          <mat-label>Passwort</mat-label>
          <input
            matInput
            type="password"
            autocomplete="current-password"
            [formField]="signInForm.password"
          />
        </mat-form-field>

        @if (errorMessage() !== null) {
          <p class="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700" role="alert">
            {{ errorMessage() }}
          </p>
        }

        <button
          mat-flat-button
          type="submit"
          class="w-full !bg-slate-950 !py-6"
          [disabled]="isSubmitting() || !signInForm().valid()"
        >
          @if (isSubmitting()) {
            <mat-spinner diameter="20" />
          } @else {
            Anmelden
          }
        </button>

        <p class="text-center text-sm text-slate-600">
          Noch kein Konto?
          <a class="font-semibold text-cyan-700 hover:underline" routerLink="/register">Jetzt registrieren</a>
        </p>
      </form>
    </mat-card>
  `,
})
export default class SignInComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly signInModel = signal<SignInCredentials>({
    email: '',
    password: '',
  });

  protected readonly signInForm = form(this.signInModel, (credentials) => {
    required(credentials.email);
    email(credentials.email);
    required(credentials.password);
  });
  protected readonly isSubmitting = signal(false);
  protected readonly errorMessage = signal<string | null>(null);

  protected async signIn(event: SubmitEvent): Promise<void> {
    event.preventDefault();

    if (!this.signInForm().valid() || this.isSubmitting()) {
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    try {
      const credentials = this.signInModel();
      await firstValueFrom(this.authService.signInWithPassword(credentials.email, credentials.password));
      await this.router.navigateByUrl('/dashboard');
    } catch (error: unknown) {
      this.errorMessage.set(error instanceof Error ? error.message : 'Anmeldung fehlgeschlagen.');
    } finally {
      this.isSubmitting.set(false);
    }
  }
}
