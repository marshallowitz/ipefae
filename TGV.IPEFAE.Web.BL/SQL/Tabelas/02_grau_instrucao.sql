USE [tgv_prd_ipefae]
GO

/****** Object:  Table [dbo].[grau_instrucao]    Script Date: 28/04/2018 16:59:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[grau_instrucao](
	[id] [int] NOT NULL,
	[nome] [varchar](200) NOT NULL,
	[ativo] [bit] NOT NULL,
 CONSTRAINT [PK_grau_instrucao] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[grau_instrucao] ADD  CONSTRAINT [DF_grau_instrucao_ativo]  DEFAULT ((1)) FOR [ativo]
GO


