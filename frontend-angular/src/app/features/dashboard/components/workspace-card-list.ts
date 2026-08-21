import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { WorkspaceDashboardItem } from '../../../shared/models/workspace-dashboard';

@Component({
  selector: 'app-workspace-card-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [MatButtonModule, MatCardModule, MatIconModule],
  styles: `
    :host {
      display: block;
    }
  `,
  template: `
    @if (isLoading()) {
      <div class="grid gap-5 sm:grid-cols-2 xl:grid-cols-3">
        @for (skeleton of [1, 2, 3]; track skeleton) {
          <div class="h-52 animate-pulse rounded-2xl bg-slate-200"></div>
        }
      </div>
    } @else if (errorMessage() !== null) {
      <div class="rounded-xl border border-red-200 bg-red-50 p-5 text-red-800" role="alert">
        {{ errorMessage() }}
      </div>
    } @else if (workspaces().length > 0) {
      <div class="grid gap-5 sm:grid-cols-2 xl:grid-cols-3">
        @for (workspace of workspaces(); track workspace.id) {
          <mat-card class="group min-h-52 overflow-hidden border border-slate-200 bg-white p-6 shadow-sm transition hover:-translate-y-1 hover:shadow-xl hover:shadow-slate-200/80">
            <div class="flex items-start justify-between gap-4">
              <span class="grid size-12 place-items-center rounded-xl bg-cyan-100 text-lg font-bold text-cyan-800">
                {{ workspace.name.slice(0, 1).toUpperCase() }}
              </span>
              <span
                class="rounded-full px-3 py-1 text-xs font-semibold"
                [class.bg-amber-100]="workspace.role === 'Manager'"
                [class.text-amber-800]="workspace.role === 'Manager'"
                [class.bg-slate-100]="workspace.role === 'Employee'"
                [class.text-slate-700]="workspace.role === 'Employee'"
              >
                {{ workspace.role }}
              </span>
            </div>
            <h2 class="mt-7 text-xl font-semibold tracking-tight text-slate-950">{{ workspace.name }}</h2>
            <div class="mt-3 flex items-center gap-2 text-sm text-slate-500">
              <mat-icon class="!text-lg">groups</mat-icon>
              <span>{{ workspace.employeeCount }} employee{{ workspace.employeeCount === 1 ? '' : 's' }}</span>
            </div>
          </mat-card>
        }
      </div>
    } @else {
      <button
        type="button"
        class="grid min-h-64 w-full place-items-center rounded-2xl border-2 border-dashed border-cyan-300 bg-cyan-50 p-8 text-center transition hover:border-cyan-500 hover:bg-cyan-100"
        (click)="createWorkspace.emit()"
      >
        <span class="grid size-14 place-items-center rounded-full bg-white text-cyan-700 shadow-sm">
          <mat-icon>add</mat-icon>
        </span>
        <span class="mt-4 text-lg font-semibold text-slate-950">Create your first workspace</span>
        <span class="mt-1 text-sm text-slate-600">Invite your team and start planning work together.</span>
      </button>
    }
  `,
})
export default class WorkspaceCardListComponent {
  readonly workspaces = input.required<readonly WorkspaceDashboardItem[]>();
  readonly isLoading = input.required<boolean>();
  readonly errorMessage = input.required<string | null>();
  readonly createWorkspace = output<void>();
}
