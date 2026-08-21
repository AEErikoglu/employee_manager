export type WorkspaceRole = 'Manager' | 'Employee';

export interface WorkspaceDashboardItem {
  readonly id: string;
  readonly name: string;
  readonly employeeCount: number;
  readonly role: WorkspaceRole;
}

export interface CreateWorkspaceRequest {
  readonly name: string;
}
