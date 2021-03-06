CREATE TABLE [dbo].[Session](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[FBID] [varchar](max) NOT NULL,
	[Type] [int] NOT NULL,
	[NoOfLink] [int] NOT NULL,
	[CurrentLink] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[CreatedTime] [datetime] NOT NULL,
	[FinishedTime] [datetime] NULL
 CONSTRAINT [PK_Session] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

CREATE TABLE [dbo].[Account](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Login] [varchar](max) NULL,
	[Pass] [varchar](max) NULL,
	[Email] [varchar](max) NULL,
	[FBToken] [varchar](max) NULL,
	[FBID] [varchar](max) NULL,
	[FBName] [varchar](max) NULL,
	[RealName] [varchar](max) NULL,
	[LastLinkedTime] [datetime] NOT NULL,
	[CreatedTime] [datetime] NOT NULL,
	[LastLoginTime] [datetime] NOT NULL
 CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

CREATE TABLE [dbo].[SessionLink](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SessionID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[LinkedTime] [int] NOT NULL,
 CONSTRAINT [PK_SessionLink] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

ALTER TABLE [dbo].[SessionLink]  WITH CHECK ADD  CONSTRAINT [FK_SessionLink_Account] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([ID])
ALTER TABLE [dbo].[SessionLink] CHECK CONSTRAINT [FK_SessionLink_Account]

ALTER TABLE [dbo].[SessionLink]  WITH CHECK ADD  CONSTRAINT [FK_SessionLink_Session] FOREIGN KEY([SessionID])
REFERENCES [dbo].[Session] ([ID])
ALTER TABLE [dbo].[SessionLink] CHECK CONSTRAINT [FK_SessionLink_Session]

ALTER TABLE [dbo].[Session]  WITH CHECK ADD  CONSTRAINT [FK_Session_Account] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([ID])
ALTER TABLE [dbo].[Session] CHECK CONSTRAINT [FK_Session_Account]
