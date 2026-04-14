/*
    AuthorizationServer seed data
    Mirrors the dashboard mock data in:
    C:\Users\CSC\source\repos\MyGateKeeperUIDashboard\src\services\mockData.ts
*/

SET NOCOUNT ON;

BEGIN TRANSACTION;

DELETE FROM [RoleApplicationControlGrants];
DELETE FROM [RoleApplicationGrants];
DELETE FROM [UserAssignments];
DELETE FROM [BusinessUnits];
DELETE FROM [Tenants];
DELETE FROM [RoleDefinitions];

DBCC CHECKIDENT ('[RoleApplicationControlGrants]', RESEED, 0);
DBCC CHECKIDENT ('[RoleApplicationGrants]', RESEED, 0);
DBCC CHECKIDENT ('[UserAssignments]', RESEED, 0);
DBCC CHECKIDENT ('[BusinessUnits]', RESEED, 0);
DBCC CHECKIDENT ('[Tenants]', RESEED, 0);
DBCC CHECKIDENT ('[RoleDefinitions]', RESEED, 0);

INSERT INTO [Tenants] ([Code], [Name], [RegistrationNumber], [Description], [IsActive])
VALUES
('t1', 'StudioX53 Corp', 'REG-T1-2026', 'Primary corporate tenant', 1),
('t2', 'StudioX53 Global Inc', 'REG-T2-2026', 'Global operations tenant', 1);

INSERT INTO [BusinessUnits] ([Code], [TenantCode], [Name], [Description], [IsActive])
VALUES
('bu1', 't1', 'StudioX53 South India', 'South India business unit', 1),
('bu2', 't1', 'StudioX53 EMEA', 'EMEA business unit', 1),
('bu3', 't2', 'StudioX53 Global Logistics', 'Global logistics business unit', 1),
('bu4', 't2', 'StudioX53 Global R&D', 'Global R&D business unit', 1);

INSERT INTO [RoleDefinitions] ([Code], [Name], [ScopeType], [IsActive])
VALUES
('r_admin', 'Administrator', 'GLOBAL', 1),
('r_manager', 'Manager', 'BU_SCOPED', 1),
('r_user', 'Business User', 'BU_SCOPED', 1);

INSERT INTO [UserAssignments] ([UserCode], [TenantCode], [BusinessUnitCode], [RoleCode])
VALUES
('u1', 't1', NULL, 'r_admin'),
('u2', 't1', 'bu2', 'r_manager'),
('u2', 't2', 'bu3', 'r_manager'),
('u3', 't2', 'bu4', 'r_user'),
('u3', 't1', 'bu1', 'r_manager');

INSERT INTO [RoleApplicationGrants] ([RoleCode], [ApplicationCode], [IsActive])
VALUES
('r_admin', 'app1', 1),
('r_admin', 'app2', 1),
('r_admin', 'app3', 1),
('r_admin', 'app4', 1),
('r_manager', 'app1', 1),
('r_manager', 'app2', 1),
('r_manager', 'app4', 1),
('r_user', 'app2', 1),
('r_user', 'app4', 1);

INSERT INTO [RoleApplicationControlGrants] ([RoleCode], [ApplicationCode], [ControlCode], [IsActive])
VALUES
('r_admin', 'app1', 'View', 1),
('r_admin', 'app1', 'Edit', 1),
('r_admin', 'app1', 'Delete', 1),
('r_admin', 'app1', 'Approve', 1),
('r_admin', 'app2', 'View', 1),
('r_admin', 'app2', 'Edit', 1),
('r_admin', 'app2', 'Delete', 1),
('r_admin', 'app2', 'Approve', 1),
('r_admin', 'app3', 'View', 1),
('r_admin', 'app3', 'Edit', 1),
('r_admin', 'app3', 'Delete', 1),
('r_admin', 'app3', 'Approve', 1),
('r_admin', 'app4', 'View', 1),
('r_admin', 'app4', 'Edit', 1),
('r_admin', 'app4', 'Delete', 1),
('r_admin', 'app4', 'Approve', 1),
('r_manager', 'app1', 'View', 1),
('r_manager', 'app1', 'Edit', 1),
('r_manager', 'app1', 'Approve', 1),
('r_manager', 'app2', 'View', 1),
('r_manager', 'app2', 'Edit', 1),
('r_manager', 'app2', 'Approve', 1),
('r_manager', 'app4', 'View', 1),
('r_manager', 'app4', 'Edit', 1),
('r_user', 'app2', 'View', 1),
('r_user', 'app4', 'View', 1),
('r_user', 'app4', 'Edit', 1);

COMMIT TRANSACTION;
