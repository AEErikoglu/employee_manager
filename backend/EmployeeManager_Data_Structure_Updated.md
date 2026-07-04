# EmployeeManager Data Structure Documentation

## 1. Overview

EmployeeManager is a public mobile app for small businesses. Managers can create workplaces, invite employees or other managers, view employee availability, and assign shifts. Employees can join workplaces through invitations, enter their availability for specific calendar weeks, and view assigned working hours.

The planned tech stack is:

- Frontend: .NET MAUI
- Backend: .NET Web API
- Database: PostgreSQL
- ORM: Entity Framework Core
- Authentication: Microsoft Entra External ID / Azure authentication
- Supported login methods: email/password, email one-time passcode, Google, Apple, Microsoft, Facebook, and possibly custom OIDC providers later

The application is designed as a multi-workplace system. A single user can belong to multiple workplaces. The user's role is not stored globally on the user account. Instead, the role is stored per workplace membership.

Example:

```text
Anna can be an Employee in Workplace A.
Anna can also be an Employee in Workplace B.
Anna could even be a Manager in Workplace C.
A single workplace can also have multiple managers.
```

This is why the `workplace_members` table is central to the data model.

---

## 2. Main Entities

The first version of the application uses the following tables:

```text
app_users
workplaces
workplace_members
invitations
availability_slots
shifts
```

---

## 3. Entity Relationship Overview

```text
app_users
    └──< workplace_members >── workplaces
                    │
                    ├──< availability_slots
                    │
                    └──< shifts

workplaces
    └──< invitations
```

Important relationship:

```text
User → WorkplaceMember → AvailabilitySlot
```

Availability is not stored directly for a user. It is stored for a user's membership in a specific workplace.

This makes it possible for the same user to have different availability in different workplaces.

---

## 4. Tables

## 4.1 `app_users`

Stores users of the application.

Users authenticate through Microsoft Entra External ID. The app does not store user passwords. Even email/password login is handled by the external authentication provider.

### Purpose

Stores the local application representation of an authenticated user.

### Columns

| Column | Type | Required | Description |
|---|---:|---:|---|
| `id` | `UUID` | Yes | Internal primary key of the user. |
| `external_auth_user_id` | `VARCHAR(200)` | Yes | Unique user ID from the authentication provider. |
| `auth_provider` | `VARCHAR(50)` | Yes | Login provider, for example `EmailPassword`, `Google`, `Apple`, `Microsoft`. |
| `email` | `VARCHAR(255)` | Yes | User's email address. Used for invitations. |
| `display_name` | `VARCHAR(200)` | No | Optional display name. |
| `created_at` | `TIMESTAMPTZ` | Yes | Creation timestamp. |

### Notes

`email` is intentionally not globally unique in the first version. The same person might use the same email with different login providers. Account linking can be added later.

---

## 4.2 `workplaces`

Stores workplaces/businesses created by users.

### Purpose

Represents one business, team, branch, or workplace.

### Columns

| Column | Type | Required | Description |
|---|---:|---:|---|
| `id` | `UUID` | Yes | Primary key of the workplace. |
| `name` | `VARCHAR(200)` | Yes | Workplace name. |
| `created_by_user_id` | `UUID` | Yes | User who created the workplace. |
| `created_at` | `TIMESTAMPTZ` | Yes | Creation timestamp. |

### Business Rule

When a user creates a workplace, the backend must also create a `workplace_members` row with role `Manager`.

Example:

```text
User creates workplace "Pizza Shop"
→ create workplace
→ create workplace_member with role = Manager
```

---

## 4.3 `workplace_members`

Connects users to workplaces and stores their role inside that workplace.

This is the most important table in the system.

### Purpose

Represents a user inside a specific workplace.

### Columns

| Column | Type | Required | Description |
|---|---:|---:|---|
| `id` | `UUID` | Yes | Primary key of the workplace membership. |
| `workplace_id` | `UUID` | Yes | Workplace the user belongs to. |
| `user_id` | `UUID` | Yes | User who belongs to the workplace. |
| `role` | `VARCHAR(30)` | Yes | Role inside the workplace: `Manager` or `Employee`. |
| `joined_at` | `TIMESTAMPTZ` | Yes | Timestamp when the user joined the workplace. |

### Important Concept

A user can have multiple memberships.

Example:

```text
User: Anna

workplace_member 1:
- User = Anna
- Workplace = Restaurant A
- Role = Employee

workplace_member 2:
- User = Anna
- Workplace = Bakery B
- Role = Employee
```

So the backend can distinguish:

```text
Anna in Restaurant A
Anna in Bakery B
```

### Multiple Managers

A workplace can have multiple managers because managers are stored as rows in `workplace_members` with `role = Manager`.

Example:

```text
workplace_members

Workspace A + Max  + Manager
Workspace A + Anna + Manager
Workspace A + Tom  + Employee
```

This means Max and Anna can both manage the same workspace.

### Constraints

The combination of `workplace_id` and `user_id` must be unique.

This prevents the same user from being added twice to the same workplace. It does not prevent multiple managers in the same workplace.

Recommended backend rule:

```text
A workspace must always have at least one Manager.
```

Before deleting a manager or changing a manager to employee, the backend should check that another manager still exists in the workspace.

---

## 4.4 `invitations`

Stores invitations sent by managers to employees or other managers.

### Purpose

Allows managers to invite users to a workplace by email and define whether the invited user should become an `Employee` or `Manager`.

### Columns

| Column | Type | Required | Description |
|---|---:|---:|---|
| `id` | `UUID` | Yes | Primary key of the invitation. |
| `workplace_id` | `UUID` | Yes | Workplace to which the user is invited. |
| `invited_email` | `VARCHAR(255)` | Yes | Email address of the invited person. |
| `invited_role` | `VARCHAR(30)` | Yes | Role the invited person will receive after accepting: `Employee` or `Manager`. |
| `token` | `VARCHAR(300)` | Yes | Unique invitation token. |
| `status` | `VARCHAR(30)` | Yes | `Pending`, `Accepted`, `Expired`, or `Cancelled`. |
| `invited_by_member_id` | `UUID` | Yes | Workplace member who sent the invitation. Usually a manager. |
| `accepted_by_member_id` | `UUID` | No | Workplace membership created after the invitation is accepted. |
| `created_at` | `TIMESTAMPTZ` | Yes | Creation timestamp. |
| `expires_at` | `TIMESTAMPTZ` | Yes | Expiration timestamp. |
| `accepted_at` | `TIMESTAMPTZ` | No | Timestamp when the invitation was accepted. |

### Meaning of Invitation Fields

```text
invited_email = who gets invited
invited_by_member_id = manager who sent the invitation
accepted_by_member_id = workplace membership created after acceptance
```

### Invitation Flow

```text
1. Manager invites a user by email.
2. Manager chooses the invited role: `Employee` or `Manager`.
3. Backend creates invitation with status `Pending` and stores `invited_role`.
4. Invited user signs in or signs up.
5. Backend checks that authenticated user's email matches `invited_email`.
6. Backend creates `workplace_member` with `role = invitation.invited_role`.
7. Backend sets invitation status to `Accepted`.
8. Backend stores `accepted_by_member_id` and `accepted_at`.
```

---

## 4.5 `availability_slots`

Stores employee availability for concrete calendar dates.

This supports a week-based calendar UI, similar to Microsoft Teams or Outlook calendar views.

### Purpose

Stores when an employee is available in a specific workplace on a specific date.

### Columns

| Column | Type | Required | Description |
|---|---:|---:|---|
| `id` | `UUID` | Yes | Primary key of the availability slot. |
| `workplace_member_id` | `UUID` | Yes | Employee membership this availability belongs to. |
| `date` | `DATE` | Yes | Concrete calendar date. |
| `start_time` | `TIME` | Yes | Start time of availability. |
| `end_time` | `TIME` | Yes | End time of availability. |
| `created_at` | `TIMESTAMPTZ` | Yes | Creation timestamp. |

### Important Concept

Availability belongs to `workplace_member_id`, not directly to `user_id`.

This allows different availability in different workplaces.

Example:

```text
Anna is employee in Workplace A:
- 2026-08-03, 08:00 - 12:00

Anna is employee in Workplace B:
- 2026-08-03, 14:00 - 16:00
```

These are stored as two separate availability rows, each pointing to a different `workplace_member_id`.

### Business Rules

```text
start_time must be before end_time.
Only employees should create their own availability.
Managers can view availability for employees in their workplace.
```

---

## 4.6 `shifts`

Stores working hours assigned by managers.

### Purpose

Represents a shift assigned to an employee for a concrete date and time.

### Columns

| Column | Type | Required | Description |
|---|---:|---:|---|
| `id` | `UUID` | Yes | Primary key of the shift. |
| `workplace_id` | `UUID` | Yes | Workplace where the shift belongs. |
| `workplace_member_id` | `UUID` | Yes | Employee membership receiving the shift. |
| `date` | `DATE` | Yes | Concrete work date. |
| `start_time` | `TIME` | Yes | Shift start time. |
| `end_time` | `TIME` | Yes | Shift end time. |
| `assigned_by_member_id` | `UUID` | Yes | Manager membership that assigned the shift. |
| `created_at` | `TIMESTAMPTZ` | Yes | Creation timestamp. |

### Business Rules

```text
start_time must be before end_time.
Only managers can assign shifts.
The target employee must belong to the same workplace.
The assigned shift must be inside an availability slot of the employee.
```

Example:

```text
Employee availability:
2026-08-03, 08:00 - 12:00

Allowed shift:
2026-08-03, 09:00 - 11:00

Not allowed shift:
2026-08-03, 07:00 - 10:00
```

---

## 5. PostgreSQL Schema

Create the tables in this order:

```text
1. app_users
2. workplaces
3. workplace_members
4. invitations
5. availability_slots
6. shifts
7. indexes
```

### Full SQL

```sql
CREATE TABLE app_users (
    id UUID PRIMARY KEY,

    external_auth_user_id VARCHAR(200) NOT NULL UNIQUE,
    auth_provider VARCHAR(50) NOT NULL,

    email VARCHAR(255) NOT NULL,
    display_name VARCHAR(200),

    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE workplaces (
    id UUID PRIMARY KEY,

    name VARCHAR(200) NOT NULL,

    created_by_user_id UUID NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT fk_workplaces_created_by_user
        FOREIGN KEY (created_by_user_id)
        REFERENCES app_users(id)
        ON DELETE RESTRICT
);

CREATE TABLE workplace_members (
    id UUID PRIMARY KEY,

    workplace_id UUID NOT NULL,
    user_id UUID NOT NULL,

    role VARCHAR(30) NOT NULL,

    joined_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT fk_workplace_members_workplace
        FOREIGN KEY (workplace_id)
        REFERENCES workplaces(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_workplace_members_user
        FOREIGN KEY (user_id)
        REFERENCES app_users(id)
        ON DELETE CASCADE,

    CONSTRAINT ck_workplace_members_role
        CHECK (role IN ('Manager', 'Employee')),

    CONSTRAINT uq_workplace_members_workplace_user
        UNIQUE (workplace_id, user_id)
);

CREATE TABLE invitations (
    id UUID PRIMARY KEY,

    workplace_id UUID NOT NULL,

    invited_email VARCHAR(255) NOT NULL,
    invited_role VARCHAR(30) NOT NULL DEFAULT 'Employee',

    token VARCHAR(300) NOT NULL UNIQUE,

    status VARCHAR(30) NOT NULL DEFAULT 'Pending',

    invited_by_member_id UUID NOT NULL,
    accepted_by_member_id UUID,

    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    expires_at TIMESTAMPTZ NOT NULL,
    accepted_at TIMESTAMPTZ,

    CONSTRAINT fk_invitations_workplace
        FOREIGN KEY (workplace_id)
        REFERENCES workplaces(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_invitations_invited_by_member
        FOREIGN KEY (invited_by_member_id)
        REFERENCES workplace_members(id)
        ON DELETE RESTRICT,

    CONSTRAINT fk_invitations_accepted_by_member
        FOREIGN KEY (accepted_by_member_id)
        REFERENCES workplace_members(id)
        ON DELETE SET NULL,

    CONSTRAINT ck_invitations_status
        CHECK (status IN ('Pending', 'Accepted', 'Expired', 'Cancelled')),

    CONSTRAINT ck_invitations_invited_role
        CHECK (invited_role IN ('Manager', 'Employee'))
);

CREATE TABLE availability_slots (
    id UUID PRIMARY KEY,

    workplace_member_id UUID NOT NULL,

    date DATE NOT NULL,

    start_time TIME NOT NULL,
    end_time TIME NOT NULL,

    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT fk_availability_slots_workplace_member
        FOREIGN KEY (workplace_member_id)
        REFERENCES workplace_members(id)
        ON DELETE CASCADE,

    CONSTRAINT ck_availability_slots_time
        CHECK (start_time < end_time)
);

CREATE TABLE shifts (
    id UUID PRIMARY KEY,

    workplace_id UUID NOT NULL,
    workplace_member_id UUID NOT NULL,

    date DATE NOT NULL,

    start_time TIME NOT NULL,
    end_time TIME NOT NULL,

    assigned_by_member_id UUID NOT NULL,

    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT fk_shifts_workplace
        FOREIGN KEY (workplace_id)
        REFERENCES workplaces(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_shifts_workplace_member
        FOREIGN KEY (workplace_member_id)
        REFERENCES workplace_members(id)
        ON DELETE CASCADE,

    CONSTRAINT fk_shifts_assigned_by_member
        FOREIGN KEY (assigned_by_member_id)
        REFERENCES workplace_members(id)
        ON DELETE RESTRICT,

    CONSTRAINT ck_shifts_time
        CHECK (start_time < end_time)
);

CREATE INDEX ix_workplace_members_user_id
ON workplace_members(user_id);

CREATE INDEX ix_workplace_members_workplace_id
ON workplace_members(workplace_id);

CREATE INDEX ix_invitations_workplace_id
ON invitations(workplace_id);

CREATE INDEX ix_invitations_invited_email
ON invitations(invited_email);

CREATE INDEX ix_availability_slots_member_date
ON availability_slots(workplace_member_id, date);

CREATE INDEX ix_shifts_workplace_date
ON shifts(workplace_id, date);

CREATE INDEX ix_shifts_member_date
ON shifts(workplace_member_id, date);
```

---

## 6. Backend Business Logic

## 6.1 Create Workplace

```text
1. Authenticated user sends request to create workplace.
2. Backend creates row in workplaces.
3. Backend creates row in workplace_members:
   - user_id = current user
   - workplace_id = new workplace
   - role = Manager
```

---

## 6.2 Invite User

```text
1. Backend checks current user is Manager in this workplace.
2. Manager chooses whether the invited user should become `Employee` or `Manager`.
3. Backend creates invitation:
   - workplace_id
   - invited_email
   - invited_role = Employee or Manager
   - token
   - status = Pending
   - invited_by_member_id = current manager membership
   - expires_at
```

---

## 6.3 Accept Invitation

```text
1. User signs in or signs up.
2. Backend reads authenticated user's email.
3. Backend checks invitation token.
4. Backend checks invited_email equals authenticated user's email.
5. Backend creates workplace_member:
   - workplace_id = invitation.workplace_id
   - user_id = current user
   - role = invitation.invited_role
6. Backend updates invitation:
   - status = Accepted
   - accepted_at = now
   - accepted_by_member_id = new workplace_member.id
```

---

## 6.4 Protect Last Manager

Before removing a manager from a workplace or changing a manager to employee, the backend should check whether this is the last manager.

Logic:

```text
1. Count managers in the workspace.
2. If manager count is 1, do not allow removing or demoting that manager.
3. Otherwise, allow the change.
```

Example:

```text
Allowed:
Workspace has Max and Anna as managers.
Max can be changed to Employee because Anna remains Manager.

Not allowed:
Workspace has only Max as Manager.
Max cannot be changed to Employee because the workspace would have no manager.
```

---

## 6.5 Add Availability

Endpoint idea:

```http
POST /workplaces/{workplaceId}/availability
```

Backend logic:

```text
1. Backend finds workplace_member by current user and workplaceId.
2. Backend checks role is Employee.
3. Backend validates start_time < end_time.
4. Backend creates availability_slot using workplace_member.id.
```

Important:

```text
Do not store user_id directly in availability_slots.
Use workplace_member_id.
```

---

## 6.6 View Availability

Endpoint idea:

```http
GET /workplaces/{workplaceId}/availability?from=2026-08-03&to=2026-08-09
```

Backend logic:

```text
1. Backend checks current user is Manager in this workplace.
2. Backend loads availability_slots where:
   - availability_slot.workplace_member.workplace_id = workplaceId
   - date is between from and to
```

---

## 6.7 Assign Shift

Endpoint idea:

```http
POST /workplaces/{workplaceId}/shifts
```

Backend logic:

```text
1. Backend checks current user is Manager in this workplace.
2. Backend checks target employee belongs to the same workplace.
3. Backend checks shift start_time < end_time.
4. Backend checks shift is inside one availability slot of the target employee.
5. Backend creates shift.
```

Availability check:

```text
An employee can only receive a shift if:
- availability_slot.workplace_member_id = employee workplace member id
- availability_slot.date = shift.date
- availability_slot.start_time <= shift.start_time
- availability_slot.end_time >= shift.end_time
```

---

## 7. Example Scenario

### Users

```text
Anna = app_user_1
Max = app_user_2
```

### Workplaces

```text
Workspace A = Restaurant A
Workspace B = Bakery B
Workspace C = Cafe C
```

### Workplace Members

```text
member_1:
- user = Anna
- workplace = Restaurant A
- role = Employee

member_2:
- user = Anna
- workplace = Bakery B
- role = Employee

member_3:
- user = Max
- workplace = Cafe C
- role = Manager

member_4:
- user = Anna
- workplace = Cafe C
- role = Manager
```

### Availability

```text
availability_1:
- workplace_member = member_1
- date = 2026-08-03
- start = 08:00
- end = 12:00

availability_2:
- workplace_member = member_2
- date = 2026-08-03
- start = 14:00
- end = 16:00
```

This means:

```text
Anna can work in Restaurant A from 08:00 to 12:00.
Anna can work in Bakery B from 14:00 to 16:00.
```

The backend can distinguish this because both availability rows point to different `workplace_member_id` values.

---

## 8. Recommended Future Extensions

These are not necessary for version 1, but can be added later.

### 8.1 Workplace Details

```text
workplace address
phone number
business type
time zone
logo
```

### 8.2 Shift Details

```text
break duration
shift notes
shift status: Draft, Published, Cancelled
employee confirmation
```

### 8.3 Availability Templates

```text
copy previous week
repeat availability weekly
default availability template
```

### 8.4 Account Linking

If a user signs in with Google and later with Apple using the same email, you may want to merge those identities into one app user.

### 8.5 Roles and Permissions

For version 1, `Manager` and `Employee` are enough.

Later you could add:

```text
Owner
Manager
Employee
```

Or use a more advanced permission system. The current model already supports multiple managers because every manager is simply a `workplace_members` row with `role = Manager`.

---

## 9. Final Design Rule

The most important rule in this data model is:

```text
User = the real person.
Workplace = the business/team.
WorkplaceMember = the user's identity inside one workplace.
AvailabilitySlot = when that workplace member is available.
Shift = when that workplace member is assigned to work.
```

Therefore:

```text
Do not store availability directly on the user.
Store availability on the workplace member.
```

This keeps the app correct when one person works in multiple workplaces.
