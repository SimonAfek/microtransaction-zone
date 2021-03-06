USE [microtransaction_zone_db]
GO
/****** Object:  Table [dbo].[BlockList]    Script Date: 5/26/2019 12:58:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlockList](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BlockedIp] [nvarchar](256) NOT NULL,
	[Expiration] [date] NOT NULL,
	[Explicit] [bit] NOT NULL,
	[Reason] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbGame]    Script Date: 5/26/2019 12:58:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbGame](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Summary] [nvarchar](2000) NULL,
	[RatingExplanation] [nvarchar](4000) NULL,
	[LastUpdated] [datetime] NOT NULL,
	[SmallImageUrl] [nvarchar](1000) NULL,
	[ThumbImageUrl] [nvarchar](1000) NULL,
	[GbSiteDetailUrl] [nvarchar](500) NULL,
	[IsRated] [bit] NOT NULL,
	[RatingLastUpdated] [datetime] NULL,
	[Basically] [nvarchar](280) NULL,
	[Aliases] [nvarchar](500) NULL,
	[ReleaseDate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbGameGenre]    Script Date: 5/26/2019 12:58:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbGameGenre](
	[GameId] [int] NOT NULL,
	[GenreId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_DbGameGenre_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbGameLink]    Script Date: 5/26/2019 12:58:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbGameLink](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GameId] [int] NOT NULL,
	[Link] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_DbGameLink] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbGamePlatform]    Script Date: 5/26/2019 12:58:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbGamePlatform](
	[GameId] [int] NOT NULL,
	[PlatformId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbGameRating]    Script Date: 5/26/2019 12:58:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbGameRating](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GameId] [int] NOT NULL,
	[RatingId] [int] NOT NULL,
	[PlatformId] [int] NULL,
 CONSTRAINT [PK_DbGameRating_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbGenre]    Script Date: 5/26/2019 12:58:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbGenre](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_DbGenre_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbPlatform]    Script Date: 5/26/2019 12:58:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbPlatform](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](100) NULL,
	[ImageUrl] [nvarchar](1024) NULL,
	[Abbreviation] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbRating]    Script Date: 5/26/2019 12:58:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbRating](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](4000) NOT NULL,
	[ImageUrl] [nvarchar](400) NULL,
	[ShortDescription] [nvarchar](500) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[DeletedFlag] [bit] NOT NULL,
 CONSTRAINT [PK_DbRating_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PendingDbGameRating]    Script Date: 5/26/2019 12:58:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PendingDbGameRating](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PendingSubmissionId] [int] NOT NULL,
	[RatingId] [int] NOT NULL,
	[PlatformId] [int] NULL,
 CONSTRAINT [PK_PendingDbGameRating_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PendingGameLink]    Script Date: 5/26/2019 12:58:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PendingGameLink](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PendingSubmissionId] [int] NOT NULL,
	[Link] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_PendingGameLink] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PendingSubmission]    Script Date: 5/26/2019 12:58:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PendingSubmission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GameId] [int] NOT NULL,
	[RatingExplanation] [nvarchar](4000) NULL,
	[TimeOfSubmission] [datetime] NOT NULL,
	[SubmitterIp] [nvarchar](256) NOT NULL,
	[Basically] [nvarchar](280) NULL,
	[Comment] [nvarchar](1000) NULL,
 CONSTRAINT [PK_PendingSubmission_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ThumbImage]    Script Date: 5/26/2019 12:58:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ThumbImage](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GameId] [int] NOT NULL,
	[ContentType] [nvarchar](100) NOT NULL,
	[LastUpdated] [datetime] NOT NULL,
	[ImageUrl] [nvarchar](200) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DbGame] ADD  DEFAULT ((0)) FOR [IsRated]
GO
ALTER TABLE [dbo].[DbRating] ADD  DEFAULT ('') FOR [ShortDescription]
GO
ALTER TABLE [dbo].[DbRating] ADD  DEFAULT ((0)) FOR [SortOrder]
GO
ALTER TABLE [dbo].[DbRating] ADD  DEFAULT ((0)) FOR [DeletedFlag]
GO
ALTER TABLE [dbo].[DbGameGenre]  WITH CHECK ADD  CONSTRAINT [FK_DbGameGenre_GameId] FOREIGN KEY([GameId])
REFERENCES [dbo].[DbGame] ([Id])
GO
ALTER TABLE [dbo].[DbGameGenre] CHECK CONSTRAINT [FK_DbGameGenre_GameId]
GO
ALTER TABLE [dbo].[DbGameGenre]  WITH CHECK ADD  CONSTRAINT [FK_DbGameGenre_GenreId] FOREIGN KEY([GenreId])
REFERENCES [dbo].[DbGenre] ([Id])
GO
ALTER TABLE [dbo].[DbGameGenre] CHECK CONSTRAINT [FK_DbGameGenre_GenreId]
GO
ALTER TABLE [dbo].[DbGameLink]  WITH CHECK ADD  CONSTRAINT [FK_DbGameLink_DbGame_Id] FOREIGN KEY([GameId])
REFERENCES [dbo].[DbGame] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DbGameLink] CHECK CONSTRAINT [FK_DbGameLink_DbGame_Id]
GO
ALTER TABLE [dbo].[DbGamePlatform]  WITH CHECK ADD FOREIGN KEY([GameId])
REFERENCES [dbo].[DbGame] ([Id])
GO
ALTER TABLE [dbo].[DbGamePlatform]  WITH CHECK ADD FOREIGN KEY([PlatformId])
REFERENCES [dbo].[DbPlatform] ([Id])
GO
ALTER TABLE [dbo].[DbGameRating]  WITH CHECK ADD  CONSTRAINT [FK_DbGameRating_GameId] FOREIGN KEY([GameId])
REFERENCES [dbo].[DbGame] ([Id])
GO
ALTER TABLE [dbo].[DbGameRating] CHECK CONSTRAINT [FK_DbGameRating_GameId]
GO
ALTER TABLE [dbo].[DbGameRating]  WITH CHECK ADD  CONSTRAINT [FK_DbGameRating_PlatformId] FOREIGN KEY([PlatformId])
REFERENCES [dbo].[DbPlatform] ([Id])
GO
ALTER TABLE [dbo].[DbGameRating] CHECK CONSTRAINT [FK_DbGameRating_PlatformId]
GO
ALTER TABLE [dbo].[DbGameRating]  WITH CHECK ADD  CONSTRAINT [FK_DbGameRating_RatingId] FOREIGN KEY([RatingId])
REFERENCES [dbo].[DbRating] ([Id])
GO
ALTER TABLE [dbo].[DbGameRating] CHECK CONSTRAINT [FK_DbGameRating_RatingId]
GO
ALTER TABLE [dbo].[PendingDbGameRating]  WITH CHECK ADD  CONSTRAINT [FK_PendingDbGameRating_PendingSubmission] FOREIGN KEY([PendingSubmissionId])
REFERENCES [dbo].[PendingSubmission] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PendingDbGameRating] CHECK CONSTRAINT [FK_PendingDbGameRating_PendingSubmission]
GO
ALTER TABLE [dbo].[PendingDbGameRating]  WITH CHECK ADD  CONSTRAINT [FK_PendingDbGameRating_PlatformId] FOREIGN KEY([PlatformId])
REFERENCES [dbo].[DbPlatform] ([Id])
GO
ALTER TABLE [dbo].[PendingDbGameRating] CHECK CONSTRAINT [FK_PendingDbGameRating_PlatformId]
GO
ALTER TABLE [dbo].[PendingDbGameRating]  WITH CHECK ADD  CONSTRAINT [FK_PendingDbGameRating_Rating] FOREIGN KEY([RatingId])
REFERENCES [dbo].[DbRating] ([Id])
GO
ALTER TABLE [dbo].[PendingDbGameRating] CHECK CONSTRAINT [FK_PendingDbGameRating_Rating]
GO
ALTER TABLE [dbo].[PendingGameLink]  WITH CHECK ADD  CONSTRAINT [FK_PendingGameLink_PendingSubmission_Id] FOREIGN KEY([PendingSubmissionId])
REFERENCES [dbo].[PendingSubmission] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PendingGameLink] CHECK CONSTRAINT [FK_PendingGameLink_PendingSubmission_Id]
GO
ALTER TABLE [dbo].[PendingSubmission]  WITH CHECK ADD  CONSTRAINT [FK_GameId_Game_Id] FOREIGN KEY([GameId])
REFERENCES [dbo].[DbGame] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PendingSubmission] CHECK CONSTRAINT [FK_GameId_Game_Id]
GO
ALTER TABLE [dbo].[ThumbImage]  WITH CHECK ADD FOREIGN KEY([GameId])
REFERENCES [dbo].[DbGame] ([Id])
GO
