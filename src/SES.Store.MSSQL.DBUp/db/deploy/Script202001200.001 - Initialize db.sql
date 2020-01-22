SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SESEvents](
	[index] [bigint] IDENTITY(1,1) NOT NULL,
	[queuename] [varchar](512) NOT NULL,
	[data] [varchar](max) NOT NULL,
 CONSTRAINT [PK_SESEvents] PRIMARY KEY CLUSTERED 
(
	[index] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[SESEvents] SET (LOCK_ESCALATION = AUTO)
GO

CREATE NONCLUSTERED INDEX [IX_SESEvents_queuename] ON [dbo].[SESEvents]
(
	[queuename] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO