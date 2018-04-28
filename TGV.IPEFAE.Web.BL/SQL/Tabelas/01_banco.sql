USE [tgv_prd_ipefae]
GO

/****** Object:  Table [dbo].[banco]    Script Date: 28/04/2018 16:57:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[banco](
	[id] [int] NOT NULL,
	[codigo] [varchar](3) NOT NULL,
	[nome] [varchar](100) NOT NULL,
	[ordem] [int] NOT NULL,
	[ativo] [bit] NOT NULL,
 CONSTRAINT [PK_banco] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[banco] ADD  CONSTRAINT [DF_banco_ativo]  DEFAULT ((1)) FOR [ativo]
GO


