import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormField, form, maxLength, required } from '@angular/forms/signals';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { WorkspaceDashboardService } from '../../../shared/services/workspace-dashboard.service';

interface CreateWorkspaceForm {
  readonly name: string;
}

@Component({
  selector: 'app-create-workspace',
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
      background: #f8fafc;
    }
  `,
  template: `
    <mat-card class="w-full max-w-md border border-slate-200 p-7 shadow-xl shadow-slate-200/70">
      <p class="text-xs font-semibold tracking-[0.2em] text-cyan-700">NEW WORKSPACE</p>
      <h1 class="mt-2 text-3xl font-semibold tracking-tight text-slate-950">Create a workspace</h1>
      <p class="mt-2 text-sm text-slate-600">You will be assigned as its manager.</p>

      <form class="mt-7 space-y-4" (submit)="createWorkspace($event)">
        <mat-form-field class="w-full" appearance="outline">
          <mat-label>Workspace name</mat-label>
          <input matInput autocomplete="organization" [formField]="workspaceForm.name" />
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
          [disabled]="isSubmitting() || !workspaceForm().valid()"
        >
          @if (isSubmitting()) {
            <mat-spinner diameter="20" />
          } @else {
            Create workspace
          }
        </button>
        <a mat-button class="w-full" routerLink="/dashboard">Cancel</a>
      </form>
    </mat-card>
  `,
})
export default class CreateWorkspaceComponent {
  private readonly dashboardService = inject(WorkspaceDashboardService);
  private readonly router = inject(Router);
  private readonly workspaceModel = signal<CreateWorkspaceForm>({ name: '' });

  protected readonly workspaceForm = form(this.workspaceModel, (workspace) => {
    required(workspace.name);
    maxLength(workspace.name, 200);
  });
  protected readonly isSubmitting = signal(false);
  protected readonly errorMessage = signal<string | null>(null);

  protected async createWorkspace(event: SubmitEvent): Promise<void> {
    event.preventDefault();

    if (!this.workspaceForm().valid() || this.isSubmitting()) {
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    try {
      await firstValueFrom(this.dashboardService.createWorkspace(this.workspaceModel()));
      await this.router.navigateByUrl('/dashboard');
    } catch (error: unknown) {
      this.errorMessage.set(error instanceof Error ? error.message : 'Workspace could not be created.');
    } finally {
      this.isSubmitting.set(false);
    }
  }
}
