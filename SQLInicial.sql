CREATE DATABASE VinculacionDB;
GO

USE VinculacionDB;
GO

CREATE TABLE dbo.Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200) NULL
);
GO

CREATE TABLE dbo.Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    PasswordHash NVARCHAR(512) NOT NULL,
    RoleId INT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    LastLogin DATETIME2 NULL,

    CONSTRAINT FK_Users_Roles
        FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId)
);
GO

CREATE TABLE dbo.Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Slug NVARCHAR(120) NOT NULL,
    Description NVARCHAR(300) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE dbo.ArticleStatus (
    StatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusName NVARCHAR(50) NOT NULL
);
GO

CREATE TABLE dbo.Articles (
    ArticleId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(300) NOT NULL,
    Slug NVARCHAR(350) NOT NULL,
    ContentHtml NTEXT NULL,
    FeaturedImage NVARCHAR(500) NULL,

    StatusId INT NOT NULL,
    AuthorId INT NOT NULL,
    CategoryId INT NOT NULL,

    PublishedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt DATETIME2 NULL,
    ViewCount INT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Articles_Status
        FOREIGN KEY (StatusId) REFERENCES dbo.ArticleStatus(StatusId),

    CONSTRAINT FK_Articles_Users
        FOREIGN KEY (AuthorId) REFERENCES dbo.Users(UserId),

    CONSTRAINT FK_Articles_Categories
        FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(CategoryId)
);
GO

CREATE TABLE dbo.MediaFiles (
    MediaId INT IDENTITY(1,1) PRIMARY KEY,
    ArticleId INT NULL,
    FileName NVARCHAR(300) NOT NULL,
    FilePath NVARCHAR(500) NOT NULL,
    MimeType NVARCHAR(100) NULL,
    SizeBytes BIGINT NULL,
    IsWebP BIT NULL,
    UploadedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_MediaFiles_Articles
        FOREIGN KEY (ArticleId) REFERENCES dbo.Articles(ArticleId)
);
GO

CREATE TABLE dbo.Visitors (
    VisitorId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(200) NULL,
    Email NVARCHAR(200) NULL,
    Institution NVARCHAR(300) NULL,
    RegisteredAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CookieToken NVARCHAR(512) NULL,
    IPAddress NVARCHAR(50) NULL
);
GO


CREATE TABLE dbo.AuditLog (
    LogId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL,
    Action NVARCHAR(200) NOT NULL,
    Entity NVARCHAR(100) NULL,
    EntityId INT NULL,
    Detail NTEXT NULL,
    IPAddress NVARCHAR(50) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),

    CONSTRAINT FK_AuditLog_Users
        FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId)
);
GO

CREATE INDEX IX_Articles_Slug ON dbo.Articles(Slug);
CREATE INDEX IX_Articles_AuthorId ON dbo.Articles(AuthorId);
CREATE INDEX IX_Articles_CategoryId ON dbo.Articles(CategoryId);
CREATE INDEX IX_Users_Email ON dbo.Users(Email);
GO


ALTER TABLE dbo.Users
ADD CONSTRAINT UQ_Users_Email UNIQUE (Email);

ALTER TABLE dbo.Categories
ADD CONSTRAINT UQ_Categories_Slug UNIQUE (Slug);

ALTER TABLE dbo.Articles
ADD CONSTRAINT UQ_Articles_Slug UNIQUE (Slug);
GO
