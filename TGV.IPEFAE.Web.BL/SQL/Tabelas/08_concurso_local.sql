USE [tgv_prd_ipefae]
GO

/****** Object:  Table [dbo].[concurso_local]    Script Date: 28/04/2018 17:00:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[concurso_local](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[concurso_id] [int] NOT NULL,
	[local] [varchar](200) NOT NULL,
	[ativo] [bit] NOT NULL,
 CONSTRAINT [PK_concurso_local] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[concurso_local] ADD  CONSTRAINT [DF_concurso_local_ativo]  DEFAULT ((1)) FOR [ativo]
GO

ALTER TABLE [dbo].[concurso_local]  WITH CHECK ADD  CONSTRAINT [FK_concurso_local_concurso] FOREIGN KEY([concurso_id])
REFERENCES [dbo].[concurso] ([id])
GO

ALTER TABLE [dbo].[concurso_local] CHECK CONSTRAINT [FK_concurso_local_concurso]
GO


