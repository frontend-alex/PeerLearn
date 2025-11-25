export enum WorkspaceVisibility {
  PRIVATE = "PRIVATE",
  PUBLIC = "PUBLIC",
}

export enum UserWorkspaceRole {
  OWNER = 0,
  COHOST = 1,
  MEMBER = 2,
}

export interface UserWorkspace {
  userId: number;
  workspaceId: number;
  role: UserWorkspaceRole;
  joinedAt: string;
  lastActiveAt?: string;
  user?: User;
  workspace?: Workspace;
}

export interface Workspace {
  id: number;
  name: string;
  description?: string;
  visibility: WorkspaceVisibility;
  createdBy: number;
  createdAt: string;
  updatedAt?: string;
  creatorName: string;
  memberCount: number;
  documentCount: number;
  userRole: UserWorkspaceRole;
  colorHex?: string;
  // Optional fields for full workspace data (when needed)
  
  creator?: User;
  userWorkspaces?: UserWorkspace[];
  invitations?: WorkspaceInvitation[];
  documents?: Document[];
}

export interface WorkspaceInvitation {
  id: number;
  workspaceId: number;
  invitedEmail: string;
  invitedBy: number;
  token: string;
  status: WorkspaceInvitationStatus;
  expiresAt?: string;
  createdAt: string;
}

export enum WorkspaceInvitationStatus {
  PENDING = 0,
  ACCEPTED = 1,
  EXPIRED = 2,
  REVOKED = 3,
}

export interface Document {
  id: number;
  workspaceId: number;
  title: string;
  kind: DocumentKind;
  yDocId: string;
  content?: string;
  colorHex?: string;
  createdBy: number;
  isArchived: boolean;
  createdAt: string;
  updatedAt: string;
  visibility: WorkspaceVisibility;
}

export enum DocumentKind {
  DOCUMENT = "Document",
  WHITEBOARD = "Whiteboard",
}

export interface User {
  id: number;
  username: string;
  firstName: string;
  lastName: string;
  email: string;
  emailVerified: boolean;
  onboarding: boolean;
  profilePicture?: string;
  xp: number;
}
