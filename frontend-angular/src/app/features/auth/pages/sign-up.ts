import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormField, email, form, minLength, required } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../../../shared/services/auth.service';

interface SignUpCredentials {
  readonly email: string;
  readonly password: string;
}

@Component({
  selector: 'app-sign-up',
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
        radial-gradient(circle at 85% 20%, rgb(186 230 253 / 0.75), transparent 35%),
        radial-gradient(circle at 10% 80%, rgb(167 243 208 / 0.7), transparent 35%),
        #f8fafc;
    }
  `,
  template: `
    <mat-card class="w-full max-w-md overflow-hidden border border-slate-200 shadow-xl shadow-slate-300/40">
      <div class="border-b border-slate-100 bg-slate-950 px-7 py-8 text-white">
        <p class="text-xs font-semibold tracking-[0.22em] text-cyan-300">EMPLOYEE MANAGER</p>
        <h1 class="mt-3 text-3xl font-semibold tracking-tight">Konto erstellen</h1>
        <p class="mt-2 text-sm text-slate-300">Registriere einen Benutzer in Supabase Auth.</p>
      </div>

      <div class="p-7">
        @if (successMessage() !== null) {
          <div class="space-y-4">
            <p class="rounded-md bg-emerald-50 px-3 py-3 text-sm text-emerald-800" role="status">
              {{ successMessage() }}
            </p>
            <a mat-flat-button class="w-full !bg-slate-950 !py-6" routerLink="/login">Zum Login</a>
          </div>
        } @else {
          <form class="space-y-4" (submit)="signUp($event)">
            <mat-form-field class="w-full" appearance="outline">
              <mat-label>E-Mail</mat-label>
              <input matInput type="email" autocomplete="email" [formField]="signUpForm.email" />
            </mat-form-field>

            <mat-form-field class="w-full" appearance="outline">
              <mat-label>Passwort</mat-label>
              <input matInput type="password" autocomplete="new-password" [formField]="signUpForm.password" />
              <mat-hint>Mindestens 8 Zeichen</mat-hint>
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
              [disabled]="isSubmitting() || !signUpForm().valid()"
            >
              @if (isSubmitting()) {
                <mat-spinner diameter="20" />
              } @else {
                Konto erstellen
              }
            </button>

            <p class="text-center text-sm text-slate-600">
              Bereits registriert?
              <a class="font-semibold text-cyan-700 hover:underline" routerLink="/login">Zum Login</a>
            </p>
          </form>
        }
      </div>
    </mat-card>
  `,
})
export default class SignUpComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly signUpModel = signal<SignUpCredentials>({
    email: '',
    password: '',
  });

  protected readonly signUpForm = form(this.signUpModel, (credentials) => {
    required(credentials.email);
    email(credentials.email);
    required(credentials.password);
    minLength(credentials.password, 8);
  });
  protected readonly isSubmitting = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly successMessage = signal<string | null>(null);

  protected async signUp(event: SubmitEvent): Promise<void> {
    event.preventDefault();

    if (!this.signUpForm().valid() || this.isSubmitting()) {
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    try {
      const credentials = this.signUpModel();
      const session = await firstValueFrom(this.authService.signUp(credentials.email, credentials.password));

      if (session !== null) {
        await this.router.navigateByUrl('/dashboard');
        return;
      }

      this.successMessage.set('Konto erstellt. Bitte bestaetige den Link in deiner E-Mail und melde dich danach an.');
    } catch (error: unknown) {
      this.errorMessage.set(error instanceof Error ? error.message : 'Registrierung fehlgeschlagen.');
    } finally {
      this.isSubmitting.set(false);
    }
  }
}
