using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TeamFlow.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class _20261124143718_InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AspNetRoles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUsers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                DisplayName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                AvatarUrl = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                IsDeactivated = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                PasswordHash = table.Column<string>(type: "text", nullable: true),
                SecurityStamp = table.Column<string>(type: "text", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                PhoneNumber = table.Column<string>(type: "text", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUsers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ProcessedEvent",
            columns: table => new
            {
                ConsumerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                EventId = table.Column<Guid>(type: "uuid", nullable: false),
                ProcessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProcessedEvent", x => new { x.ConsumerName, x.EventId });
            });

        migrationBuilder.CreateTable(
            name: "AspNetRoleClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                ClaimType = table.Column<string>(type: "text", nullable: true),
                ClaimValue = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserClaims",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                ClaimType = table.Column<string>(type: "text", nullable: true),
                ClaimValue = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserLogins",
            columns: table => new
            {
                LoginProvider = table.Column<string>(type: "text", nullable: false),
                ProviderKey = table.Column<string>(type: "text", nullable: false),
                ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                UserId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserRoles",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                RoleId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AspNetUserTokens",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                LoginProvider = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "NotificationPreference",
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                EventType = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                EmailEnabled = table.Column<bool>(type: "boolean", nullable: false),
                InAppEnabled = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NotificationPreference", x => new { x.UserId, x.EventType });
                table.ForeignKey(
                    name: "FK_NotificationPreference_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Workspace",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                Slug = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ArchivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Workspace", x => x.Id);
                table.ForeignKey(
                    name: "FK_Workspace_AspNetUsers_CreatedByUserId",
                    column: x => x.CreatedByUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Label",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                NormalizedName = table.Column<string>(type: "text", nullable: false, computedColumnSql: "lower(\"Name\")", stored: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Label", x => x.Id);
                table.UniqueConstraint("AK_Label_WorkspaceId_Id", x => new { x.WorkspaceId, x.Id });
                table.CheckConstraint("CK_Label_Color", "\"Color\" ~ '^#[0-9A-Fa-f]{6}$'");
                table.ForeignKey(
                    name: "FK_Label_Workspace_WorkspaceId",
                    column: x => x.WorkspaceId,
                    principalTable: "Workspace",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Notification",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                RecipientUserId = table.Column<Guid>(type: "uuid", nullable: false),
                SourceEventId = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                Payload = table.Column<JsonElement>(type: "jsonb", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ReadAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Notification", x => x.Id);
                table.ForeignKey(
                    name: "FK_Notification_AspNetUsers_RecipientUserId",
                    column: x => x.RecipientUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Notification_Workspace_WorkspaceId",
                    column: x => x.WorkspaceId,
                    principalTable: "Workspace",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "OutboxMessage",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                AggregateId = table.Column<Guid>(type: "uuid", nullable: false),
                EventType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                AggregateType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                Payload = table.Column<JsonElement>(type: "jsonb", nullable: false),
                OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                AvailableAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ProcessedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                LockedUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                LastError = table.Column<string>(type: "text", nullable: true),
                LockedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OutboxMessage", x => x.Id);
                table.ForeignKey(
                    name: "FK_OutboxMessage_Workspace_WorkspaceId",
                    column: x => x.WorkspaceId,
                    principalTable: "Workspace",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                ProjectKey = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ArchivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Project", x => x.Id);
                table.UniqueConstraint("AK_Project_WorkspaceId_Id", x => new { x.WorkspaceId, x.Id });
                table.ForeignKey(
                    name: "FK_Project_AspNetUsers_CreatedByUserId",
                    column: x => x.CreatedByUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Project_Workspace_WorkspaceId",
                    column: x => x.WorkspaceId,
                    principalTable: "Workspace",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "WorkspaceInvitation",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                InvitedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                EmailNormalized = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                TokenHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                AcceptedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WorkspaceInvitation", x => x.Id);
                table.ForeignKey(
                    name: "FK_WorkspaceInvitation_AspNetUsers_InvitedByUserId",
                    column: x => x.InvitedByUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_WorkspaceInvitation_Workspace_WorkspaceId",
                    column: x => x.WorkspaceId,
                    principalTable: "Workspace",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "WorkspaceMember",
            columns: table => new
            {
                WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                JoinedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                InvitedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WorkspaceMember", x => new { x.WorkspaceId, x.UserId });
                table.ForeignKey(
                    name: "FK_WorkspaceMember_AspNetUsers_InvitedByUserId",
                    column: x => x.InvitedByUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_WorkspaceMember_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_WorkspaceMember_Workspace_WorkspaceId",
                    column: x => x.WorkspaceId,
                    principalTable: "Workspace",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ProjectIssueSequence",
            columns: table => new
            {
                ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                NextNumber = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProjectIssueSequence", x => x.ProjectId);
                table.CheckConstraint("CK_ProjectIssueSequence_NextNumber", "\"NextNumber\" > 0");
                table.ForeignKey(
                    name: "FK_ProjectIssueSequence_Project_ProjectId",
                    column: x => x.ProjectId,
                    principalTable: "Project",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Sprint",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Goal = table.Column<string>(type: "text", nullable: true),
                StartAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                EndAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Sprint", x => x.Id);
                table.UniqueConstraint("AK_Sprint_WorkspaceId_ProjectId_Id", x => new { x.WorkspaceId, x.ProjectId, x.Id });
                table.CheckConstraint("CK_Sprint_DateRange", "\"StartAt\" < \"EndAt\"");
                table.ForeignKey(
                    name: "FK_Sprint_Project_WorkspaceId_ProjectId",
                    columns: x => new { x.WorkspaceId, x.ProjectId },
                    principalTable: "Project",
                    principalColumns: new[] { "WorkspaceId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Issue",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                ReporterUserId = table.Column<Guid>(type: "uuid", nullable: false),
                AssigneeUserId = table.Column<Guid>(type: "uuid", nullable: true),
                SprintId = table.Column<Guid>(type: "uuid", nullable: true),
                IssueNumber = table.Column<int>(type: "integer", nullable: false),
                Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                IssueType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Position = table.Column<decimal>(type: "numeric(20,10)", precision: 20, scale: 10, nullable: false),
                Version = table.Column<long>(type: "bigint", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DueAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                ArchivedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Issue", x => x.Id);
                table.UniqueConstraint("AK_Issue_WorkspaceId_Id", x => new { x.WorkspaceId, x.Id });
                table.ForeignKey(
                    name: "FK_Issue_AspNetUsers_AssigneeUserId",
                    column: x => x.AssigneeUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Issue_AspNetUsers_ReporterUserId",
                    column: x => x.ReporterUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Issue_Project_WorkspaceId_ProjectId",
                    columns: x => new { x.WorkspaceId, x.ProjectId },
                    principalTable: "Project",
                    principalColumns: new[] { "WorkspaceId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Issue_Sprint_WorkspaceId_ProjectId_SprintId",
                    columns: x => new { x.WorkspaceId, x.ProjectId, x.SprintId },
                    principalTable: "Sprint",
                    principalColumns: new[] { "WorkspaceId", "ProjectId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "ActivityLog",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                IssueId = table.Column<Guid>(type: "uuid", nullable: false),
                ActorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                Action = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                Changes = table.Column<JsonElement>(type: "jsonb", nullable: false),
                OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ActivityLog", x => x.Id);
                table.ForeignKey(
                    name: "FK_ActivityLog_AspNetUsers_ActorUserId",
                    column: x => x.ActorUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_ActivityLog_Issue_WorkspaceId_IssueId",
                    columns: x => new { x.WorkspaceId, x.IssueId },
                    principalTable: "Issue",
                    principalColumns: new[] { "WorkspaceId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Comment",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                IssueId = table.Column<Guid>(type: "uuid", nullable: false),
                AuthorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                Body = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Comment", x => x.Id);
                table.CheckConstraint("CK_Comment_BodyLength", "char_length(\"Body\") <= 10000");
                table.ForeignKey(
                    name: "FK_Comment_AspNetUsers_AuthorUserId",
                    column: x => x.AuthorUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Comment_Issue_WorkspaceId_IssueId",
                    columns: x => new { x.WorkspaceId, x.IssueId },
                    principalTable: "Issue",
                    principalColumns: new[] { "WorkspaceId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "IssueAttachment",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                IssueId = table.Column<Guid>(type: "uuid", nullable: false),
                UploadedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                OriginalFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                ObjectKey = table.Column<string>(type: "text", nullable: false),
                ContentType = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IssueAttachment", x => x.Id);
                table.CheckConstraint("CK_IssueAttachment_Size", "\"SizeBytes\" >= 0");
                table.ForeignKey(
                    name: "FK_IssueAttachment_AspNetUsers_UploadedByUserId",
                    column: x => x.UploadedByUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_IssueAttachment_Issue_WorkspaceId_IssueId",
                    columns: x => new { x.WorkspaceId, x.IssueId },
                    principalTable: "Issue",
                    principalColumns: new[] { "WorkspaceId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "IssueLabel",
            columns: table => new
            {
                IssueId = table.Column<Guid>(type: "uuid", nullable: false),
                LabelId = table.Column<Guid>(type: "uuid", nullable: false),
                WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IssueLabel", x => new { x.IssueId, x.LabelId });
                table.ForeignKey(
                    name: "FK_IssueLabel_Issue_WorkspaceId_IssueId",
                    columns: x => new { x.WorkspaceId, x.IssueId },
                    principalTable: "Issue",
                    principalColumns: new[] { "WorkspaceId", "Id" },
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_IssueLabel_Label_WorkspaceId_LabelId",
                    columns: x => new { x.WorkspaceId, x.LabelId },
                    principalTable: "Label",
                    principalColumns: new[] { "WorkspaceId", "Id" },
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ActivityLog_ActorUserId",
            table: "ActivityLog",
            column: "ActorUserId");

        migrationBuilder.CreateIndex(
            name: "IX_ActivityLog_WorkspaceId_IssueId_OccurredAt",
            table: "ActivityLog",
            columns: new[] { "WorkspaceId", "IssueId", "OccurredAt" });

        migrationBuilder.CreateIndex(
            name: "IX_AspNetRoleClaims_RoleId",
            table: "AspNetRoleClaims",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            table: "AspNetRoles",
            column: "NormalizedName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserClaims_UserId",
            table: "AspNetUserClaims",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserLogins_UserId",
            table: "AspNetUserLogins",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUserRoles_RoleId",
            table: "AspNetUserRoles",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            table: "AspNetUsers",
            column: "NormalizedEmail",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            table: "AspNetUsers",
            column: "NormalizedUserName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Comment_AuthorUserId",
            table: "Comment",
            column: "AuthorUserId");

        migrationBuilder.CreateIndex(
            name: "IX_Comment_WorkspaceId_IssueId",
            table: "Comment",
            columns: new[] { "WorkspaceId", "IssueId" });

        migrationBuilder.CreateIndex(
            name: "IX_Issue_AssigneeUserId",
            table: "Issue",
            column: "AssigneeUserId");

        migrationBuilder.CreateIndex(
            name: "IX_Issue_ProjectId_IssueNumber",
            table: "Issue",
            columns: new[] { "ProjectId", "IssueNumber" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Issue_ReporterUserId",
            table: "Issue",
            column: "ReporterUserId");

        migrationBuilder.CreateIndex(
            name: "IX_Issue_WorkspaceId_ProjectId_SprintId",
            table: "Issue",
            columns: new[] { "WorkspaceId", "ProjectId", "SprintId" });

        migrationBuilder.CreateIndex(
            name: "IX_IssueAttachment_UploadedByUserId",
            table: "IssueAttachment",
            column: "UploadedByUserId");

        migrationBuilder.CreateIndex(
            name: "IX_IssueAttachment_WorkspaceId_IssueId",
            table: "IssueAttachment",
            columns: new[] { "WorkspaceId", "IssueId" });

        migrationBuilder.CreateIndex(
            name: "IX_IssueLabel_WorkspaceId_IssueId",
            table: "IssueLabel",
            columns: new[] { "WorkspaceId", "IssueId" });

        migrationBuilder.CreateIndex(
            name: "IX_IssueLabel_WorkspaceId_LabelId",
            table: "IssueLabel",
            columns: new[] { "WorkspaceId", "LabelId" });

        migrationBuilder.CreateIndex(
            name: "IX_Label_WorkspaceId_NormalizedName",
            table: "Label",
            columns: new[] { "WorkspaceId", "NormalizedName" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Notification_RecipientUserId_SourceEventId_Type",
            table: "Notification",
            columns: new[] { "RecipientUserId", "SourceEventId", "Type" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Notification_WorkspaceId_RecipientUserId_CreatedAt",
            table: "Notification",
            columns: new[] { "WorkspaceId", "RecipientUserId", "CreatedAt" },
            filter: "\"ReadAt\" IS NULL");

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessage_AvailableAt_Id",
            table: "OutboxMessage",
            columns: new[] { "AvailableAt", "Id" },
            filter: "\"ProcessedAt\" IS NULL");

        migrationBuilder.CreateIndex(
            name: "IX_OutboxMessage_WorkspaceId",
            table: "OutboxMessage",
            column: "WorkspaceId");

        migrationBuilder.CreateIndex(
            name: "IX_Project_CreatedByUserId",
            table: "Project",
            column: "CreatedByUserId");

        migrationBuilder.CreateIndex(
            name: "IX_Project_WorkspaceId_ProjectKey",
            table: "Project",
            columns: new[] { "WorkspaceId", "ProjectKey" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Sprint_WorkspaceId_ProjectId",
            table: "Sprint",
            columns: new[] { "WorkspaceId", "ProjectId" },
            unique: true,
            filter: "\"Status\" = 'Active'");

        migrationBuilder.CreateIndex(
            name: "IX_Workspace_CreatedByUserId",
            table: "Workspace",
            column: "CreatedByUserId");

        migrationBuilder.CreateIndex(
            name: "IX_Workspace_Slug",
            table: "Workspace",
            column: "Slug",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_WorkspaceInvitation_InvitedByUserId",
            table: "WorkspaceInvitation",
            column: "InvitedByUserId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkspaceInvitation_TokenHash",
            table: "WorkspaceInvitation",
            column: "TokenHash",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_WorkspaceInvitation_WorkspaceId",
            table: "WorkspaceInvitation",
            column: "WorkspaceId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkspaceMember_InvitedByUserId",
            table: "WorkspaceMember",
            column: "InvitedByUserId");

        migrationBuilder.CreateIndex(
            name: "IX_WorkspaceMember_UserId_IsActive",
            table: "WorkspaceMember",
            columns: new[] { "UserId", "IsActive" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ActivityLog");

        migrationBuilder.DropTable(
            name: "AspNetRoleClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserClaims");

        migrationBuilder.DropTable(
            name: "AspNetUserLogins");

        migrationBuilder.DropTable(
            name: "AspNetUserRoles");

        migrationBuilder.DropTable(
            name: "AspNetUserTokens");

        migrationBuilder.DropTable(
            name: "Comment");

        migrationBuilder.DropTable(
            name: "IssueAttachment");

        migrationBuilder.DropTable(
            name: "IssueLabel");

        migrationBuilder.DropTable(
            name: "Notification");

        migrationBuilder.DropTable(
            name: "NotificationPreference");

        migrationBuilder.DropTable(
            name: "OutboxMessage");

        migrationBuilder.DropTable(
            name: "ProcessedEvent");

        migrationBuilder.DropTable(
            name: "ProjectIssueSequence");

        migrationBuilder.DropTable(
            name: "WorkspaceInvitation");

        migrationBuilder.DropTable(
            name: "WorkspaceMember");

        migrationBuilder.DropTable(
            name: "AspNetRoles");

        migrationBuilder.DropTable(
            name: "Issue");

        migrationBuilder.DropTable(
            name: "Label");

        migrationBuilder.DropTable(
            name: "Sprint");

        migrationBuilder.DropTable(
            name: "Project");

        migrationBuilder.DropTable(
            name: "Workspace");

        migrationBuilder.DropTable(
            name: "AspNetUsers");
    }
}
