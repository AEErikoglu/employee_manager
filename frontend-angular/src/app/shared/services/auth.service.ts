import { computed, Injectable, signal } from '@angular/core';
import { createClient, Session, User } from '@supabase/supabase-js';
import { Observable, from, map, tap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly isConfigured =
    !environment.supabaseUrl.includes('your-project-ref') &&
    environment.supabasePublishableKey !== 'your-publishable-key';
  private readonly supabase = this.isConfigured
    ? createClient(environment.supabaseUrl, environment.supabasePublishableKey)
    : null;

  readonly session = signal<Session | null>(null);
  readonly user = computed<User | null>(() => this.session()?.user ?? null);
  readonly isAuthenticated = computed(() => this.session() !== null);

  /** Restores an existing Supabase session and keeps the signal in sync. */
  initialize(): Promise<void> {
    if (this.supabase === null) {
      return Promise.resolve();
    }

    return this.supabase.auth.getSession().then(({ data, error }) => {
      if (error !== null) {
        throw error;
      }

      this.session.set(data.session);
      this.supabase?.auth.onAuthStateChange((_event, session) => this.session.set(session));
    });
  }

  signInWithPassword(email: string, password: string): Observable<Session> {
    if (this.supabase === null) {
      return this.missingConfiguration();
    }

    return from(this.supabase.auth.signInWithPassword({ email, password })).pipe(
      map(({ data, error }) => {
        if (error !== null || data.session === null) {
          throw error ?? new Error('Supabase did not return a session.');
        }

        return data.session;
      }),
      tap((session) => this.session.set(session)),
    );
  }

  signUp(email: string, password: string): Observable<Session | null> {
    if (this.supabase === null) {
      return this.missingConfiguration();
    }

    return from(this.supabase.auth.signUp({ email, password })).pipe(
      map(({ data, error }) => {
        if (error !== null) {
          throw error;
        }

        return data.session;
      }),
      tap((session) => this.session.set(session)),
    );
  }

  signOut(): Observable<void> {
    if (this.supabase === null) {
      return this.missingConfiguration();
    }

    return from(this.supabase.auth.signOut()).pipe(
      map(({ error }) => {
        if (error !== null) {
          throw error;
        }
      }),
      tap(() => this.session.set(null)),
    );
  }

  private missingConfiguration<T>(): Observable<T> {
    return throwError(
      () => new Error('Supabase is not configured. Set the project URL and publishable key in environment.ts.'),
    );
  }
}
