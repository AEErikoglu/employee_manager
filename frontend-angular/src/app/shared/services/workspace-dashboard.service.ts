import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateWorkspaceRequest, WorkspaceDashboardItem } from '../models/workspace-dashboard';

@Injectable({
  providedIn: 'root',
})
export class WorkspaceDashboardService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/dashboard/workspaces`;

  getWorkspaces(): Observable<readonly WorkspaceDashboardItem[]> {
    return this.http.get<readonly WorkspaceDashboardItem[]>(this.url);
  }

  createWorkspace(request: CreateWorkspaceRequest): Observable<WorkspaceDashboardItem> {
    return this.http.post<WorkspaceDashboardItem>(this.url, request);
  }
}
