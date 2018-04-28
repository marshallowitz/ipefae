USE [tgv_prd_ipefae]
GO

/****** Object:  Table [dbo].[relatorio_pdf]    Script Date: 28/04/2018 16:59:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[relatorio_pdf](
	[id] [int] NOT NULL,
	[data] [datetime] NOT NULL,
	[quantidade] [int] NOT NULL,
 CONSTRAINT [PK_relatorio_pdf] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


