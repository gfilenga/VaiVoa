CREATE DATABASE CASADOCODIGO

CREATE TABLE [Authors] (
        [Id] uniqueidentifier NOT NULL,
        [Name] varchar(50) NOT NULL,
        [Email] varchar(50) NOT NULL,
        [Descricao] varchar(400) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Authors] PRIMARY KEY ([Id])
   );