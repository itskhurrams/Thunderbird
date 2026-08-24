-- Thunderbird database schema and stored procedures.
--
-- REVIEW BEFORE RUNNING. Written blind (no DB access from this session) by reverse-engineering
-- the column/parameter names already used in src/Thunderbird.Infrastructure.Persistance and the
-- shapes of src/Thunderbird.Domain/Entities. If your existing database already has these tables
-- under different names or columns, adjust this script to match rather than running it as-is.
--
-- Safe to run against an already-populated database:
--   - Tables are created only if they don't already exist (no data loss for existing tables).
--   - All procedures use CREATE OR ALTER, so re-running this script is a no-op / idempotent.
--   - The pre-existing [dbo].[Proc_User_Login] procedure (now unused by the app - login/password
--     verification moved into the application layer) is left untouched, not dropped.

-- =============================================================
-- Tables
-- =============================================================

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Users] (
        user_id        BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        login_name     NVARCHAR(256)  NOT NULL,
        login_password NVARCHAR(256)  NOT NULL,
        first_name     NVARCHAR(256)  NOT NULL,
        last_name      NVARCHAR(256)  NOT NULL,
        is_active      BIT            NOT NULL DEFAULT 1,
        created_by     INT            NULL,
        created_date   DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
        updated_by     INT            NULL,
        updated_date   DATETIME2      NULL,
        CONSTRAINT UQ_Users_login_name UNIQUE (login_name)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Captcha]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Captcha] (
        id            BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        captcha_code  VARCHAR(10)   NOT NULL,
        created_date  DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Divisions]') AND type = 'U')
BEGIN
    -- Reference/lookup table: the app only ever reads divisions (ITerritoryRepository.GetDivisions),
    -- it never inserts into it, so division_id is not an identity column here - populate it yourself.
    CREATE TABLE [dbo].[Divisions] (
        division_id    TINYINT       NOT NULL PRIMARY KEY,
        province_id    TINYINT       NOT NULL,
        division_name  NVARCHAR(256) NOT NULL,
        is_active      BIT           NOT NULL DEFAULT 1,
        created_by     INT           NULL,
        created_date   DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
        updated_by     INT           NULL,
        updated_date   DATETIME2     NULL
    );
END
GO

-- =============================================================
-- Users
-- =============================================================

-- Fetch a user by login name only. The application verifies the password hash itself,
-- so the password is no longer passed into (or compared inside) SQL.
CREATE OR ALTER PROCEDURE [dbo].[Proc_User_GetByLoginName]
    @login_name NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        user_id,
        login_name,
        login_password,
        first_name,
        last_name,
        is_active,
        created_by,
        created_date,
        updated_by,
        updated_date
    FROM [dbo].[Users]
    WHERE login_name = @login_name;
END
GO

-- Persist a (re-)hashed password. Used for the initial hash migration the first time an
-- existing legacy/plaintext account logs in successfully, and for any future password changes.
CREATE OR ALTER PROCEDURE [dbo].[Proc_User_UpdatePassword]
    @user_id BIGINT,
    @login_password NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Users]
    SET login_password = @login_password,
        updated_date = SYSUTCDATETIME()
    WHERE user_id = @user_id;
END
GO

-- Register a new user. @login_password must already be hashed by the application - this
-- procedure never sees a plaintext password. Returns the new user_id, or -1 if the login
-- name is already taken (the UNIQUE constraint on login_name is the last line of defense
-- against a race between the existence check and the insert).
CREATE OR ALTER PROCEDURE [dbo].[Proc_User_Register]
    @login_name NVARCHAR(256),
    @login_password NVARCHAR(256),
    @first_name NVARCHAR(256),
    @last_name NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[Users] WHERE login_name = @login_name)
    BEGIN
        SELECT CAST(-1 AS BIGINT);
        RETURN;
    END

    BEGIN TRY
        INSERT INTO [dbo].[Users] (login_name, login_password, first_name, last_name, is_active, created_date)
        VALUES (@login_name, @login_password, @first_name, @last_name, 1, SYSUTCDATETIME());

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() = 2627 OR ERROR_NUMBER() = 2601 -- unique constraint violation
        BEGIN
            SELECT CAST(-1 AS BIGINT);
            RETURN;
        END;
        THROW;
    END CATCH
END
GO

-- =============================================================
-- Captcha
-- =============================================================

CREATE OR ALTER PROCEDURE [dbo].[Proc_Captch_Insert]
    @CaptchaCode VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[Captcha] (captcha_code, created_date)
    VALUES (@CaptchaCode, SYSUTCDATETIME());

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Proc_Captch_Validate]
    @Id BIGINT,
    @CaptchaCode VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(1)
    FROM [dbo].[Captcha]
    WHERE id = @Id AND captcha_code = @CaptchaCode;
END
GO

-- =============================================================
-- Divisions
-- =============================================================

CREATE OR ALTER PROCEDURE [dbo].[Proc_Division_GetAll]
    @is_active INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        division_id,
        province_id,
        division_name,
        is_active,
        created_by,
        created_date,
        updated_by,
        updated_date
    FROM [dbo].[Divisions]
    WHERE (@is_active IS NULL OR is_active = @is_active);
END
GO
