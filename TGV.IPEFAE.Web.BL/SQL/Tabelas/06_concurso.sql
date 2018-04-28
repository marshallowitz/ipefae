USE [tgv_prd_ipefae]
GO

/****** Object:  Table [dbo].[concurso]    Script Date: 28/04/2018 17:00:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[concurso](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[nome] [varchar](200) NOT NULL,
	[data] [datetime] NOT NULL,
	[ativo] [bit] NOT NULL,
 CONSTRAINT [PK_concurso] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[concurso] ADD  CONSTRAINT [DF_concurso_ativo]  DEFAULT ((1)) FOR [ativo]
GO


