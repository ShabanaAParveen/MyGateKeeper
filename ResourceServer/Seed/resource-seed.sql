/*
    ResourceServer seed data
    Application catalog only.
    App codes align with AuthorizationServer grants and dashboard mock data.
*/

SET NOCOUNT ON;

BEGIN TRANSACTION;

DELETE FROM [Applications];

INSERT INTO [Applications]
(
    [Id],
    [Code],
    [Name],
    [Description],
    [LaunchUrl],
    [IconUrl],
    [Maintainer],
    [ContactEmail],
    [IsActive],
    [SortOrder],
    [CreatedAtUtc],
    [UpdatedAtUtc]
)
VALUES
(
    '11111111-1111-1111-1111-111111111111',
    'app1',
    'Finance Portal',
    'Finance workflows and approvals',
    'http://localhost:5301',
    NULL,
    'Finance Platform Team',
    'finance-support@mygatekeeper.local',
    1,
    1,
    GETUTCDATE(),
    GETUTCDATE()
),
(
    '22222222-2222-2222-2222-222222222222',
    'app2',
    'Inventory Manager',
    'Inventory operations and stock actions',
    'http://localhost:5302',
    NULL,
    'Operations Platform Team',
    'inventory-support@mygatekeeper.local',
    1,
    2,
    GETUTCDATE(),
    GETUTCDATE()
),
(
    '33333333-3333-3333-3333-333333333333',
    'app3',
    'HR System',
    'People operations and HR records',
    'http://localhost:5303',
    NULL,
    'People Systems Team',
    'hr-support@mygatekeeper.local',
    1,
    3,
    GETUTCDATE(),
    GETUTCDATE()
),
(
    '44444444-4444-4444-4444-444444444444',
    'app4',
    'CRM',
    'Customer and relationship management',
    'http://localhost:5304',
    NULL,
    'Customer Systems Team',
    'crm-support@mygatekeeper.local',
    1,
    4,
    GETUTCDATE(),
    GETUTCDATE()
),
(
    '55555555-5555-5555-5555-555555555555',
    'app5',
    'Vendor Hub',
    'Vendor operations and onboarding',
    'http://localhost:5305',
    NULL,
    'Supplier Platform Team',
    'vendor-support@mygatekeeper.local',
    1,
    5,
    GETUTCDATE(),
    GETUTCDATE()
);

COMMIT TRANSACTION;
