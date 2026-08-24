-- Adds the columns needed for mandatory two-factor authentication (email + WhatsApp).
--
-- REVIEW BEFORE RUNNING. Additive only:
--   - New columns are added to [dbo].[Users] only if they don't already exist, and are
--     nullable so existing rows are not touched. Any pre-existing account without an email
--     or phone number on file will need one added before it can log in again, since 2FA is
--     mandatory - this script does not (and cannot) invent contact details for you.
--   - Proc_User_GetByLoginName and Proc_User_Register are updated (CREATE OR ALTER) to
--     read/write the new columns; safe to re-run.
-- Assumes [dbo].[Users] already exists as created by sql/2026-08-24_full_schema.sql - run
-- that first if you haven't already.

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'email')
BEGIN
    ALTER TABLE [dbo].[Users] ADD email NVARCHAR(256) NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'phone_number')
BEGIN
    ALTER TABLE [dbo].[Users] ADD phone_number NVARCHAR(32) NULL;
END
GO

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
        email,
        phone_number,
        is_active,
        created_by,
        created_date,
        updated_by,
        updated_date
    FROM [dbo].[Users]
    WHERE login_name = @login_name;
END
GO

CREATE OR ALTER PROCEDURE [dbo].[Proc_User_Register]
    @login_name NVARCHAR(256),
    @login_password NVARCHAR(256),
    @first_name NVARCHAR(256),
    @last_name NVARCHAR(256),
    @email NVARCHAR(256),
    @phone_number NVARCHAR(32)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[Users] WHERE login_name = @login_name)
    BEGIN
        SELECT CAST(-1 AS BIGINT);
        RETURN;
    END;

    BEGIN TRY
        INSERT INTO [dbo].[Users] (login_name, login_password, first_name, last_name, email, phone_number, is_active, created_date)
        VALUES (@login_name, @login_password, @first_name, @last_name, @email, @phone_number, 1, SYSUTCDATETIME());

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
