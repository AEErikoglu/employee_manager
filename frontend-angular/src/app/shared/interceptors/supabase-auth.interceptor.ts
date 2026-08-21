import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

/** Adds the current Supabase access token to API requests. */
export const supabaseAuthInterceptor: HttpInterceptorFn = (request, next) => {
  const accessToken = inject(AuthService).session()?.access_token;

  return next(
    accessToken === undefined
      ? request
      : request.clone({
          setHeaders: {
            Authorization: `Bearer ${accessToken}`,
          },
        }),
  );
};
