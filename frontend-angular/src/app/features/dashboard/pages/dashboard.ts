import { ChangeDetectionStrategy, Component, afterNextRender, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { WorkspaceDashboardItem } from '../../../shared/models/workspace-dashboard';
import { AuthService } from '../../../shared/services/auth.service';
import { WorkspaceDashboardService } from '../../../shared/services/workspace-dashboard.service';
import WorkspaceCardListComponent from '../components/workspace-card-list';

@Component({
  selector: 'app-dashboard',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatButtonModule, MatIconModule, WorkspaceCardListComponent],
  styles: `
    :host {
      display: block;
      min-height: 100dvh;
      background: #f8fafc;
    }
  `,
  template: `
    <main class="mx-auto max-w-7xl px-5 py-8 sm:px-8 lg:px-10">
      <header class="flex flex-col gap-5 border-b border-slate-200 pb-8 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <p class="text-xs font-semibold tracking-[0.2em] text-cyan-700">EMPLOYEE MANAGER</p>
          <h1 class="mt-2 text-3xl font-semibold tracking-tight text-slate-950">Your workspaces</h1>
          <p class="mt-2 text-sm text-slate-600">Choose a workspace to manage your team and schedules.</p>
        </div>
        <div class="flex items-center gap-3">
          <span class="hidden text-sm text-slate-500 sm:inline">{{ authService.user()?.email }}</span>
          <button mat-stroked-button type="button" (click)="signOut()">
            <mat-icon>logout</mat-icon>
            Sign out
          </button>
        </div>
      </header>

      <section class="pt-8" aria-label="Workspaces">
        <app-workspace-card-list
          [workspaces]="workspaces()"
          [isLoading]="isLoading()"
          [errorMessage]="errorMessage()"
          (createWorkspace)="openWorkspaceCreation()"
        />
      </section>
    </main>
  `,
})
export default class DashboardComponent {
  protected readonly authService = inject(AuthService);
  private readonly dashboardService = inject(WorkspaceDashboardService);
  private readonly router = inject(Router);
  protected readonly workspaces = signal<readonly WorkspaceDashboardItem[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);

  private readonly loadDashboard = afterNextRender(() => void this.loadWorkspaces());

  protected openWorkspaceCreation(): void {
    void this.router.navigateByUrl('/workspaces/create');
  }

  protected async signOut(): Promise<void> {
    await firstValueFrom(this.authService.signOut());
    await this.router.navigateByUrl('/login');
  }

  private async loadWorkspaces(): Promise<void> {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    try {
      this.workspaces.set(await firstValueFrom(this.dashboardService.getWorkspaces()));
    } catch (error: unknown) {
      this.errorMessage.set(error instanceof Error ? error.message : 'Workspaces could not be loaded.');
    } finally {
      this.isLoading.set(false);
    }
  }
}
