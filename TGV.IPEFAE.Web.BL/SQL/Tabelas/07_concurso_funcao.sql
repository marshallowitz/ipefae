USE [tgv_prd_ipefae]
GO

/****** Object:  Table [dbo].[concurso_funcao]    Script Date: 28/04/2018 17:00:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[concurso_funcao](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[concurso_id] [int] NOT NULL,
	[funcao] [varchar](100) NOT NULL,
	[valor_liquido] [numeric](9, 2) NOT NULL,
	[sem_desconto] [bit] NOT NULL,
	[ativo] [bit] NOT NULL,
 CONSTRAINT [PK_concurso_funcao_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[concurso_funcao] ADD  CONSTRAINT [DF_concurso_funcao_sem_desconto]  DEFAULT ((0)) FOR [sem_desconto]
GO

ALTER TABLE [dbo].[concurso_funcao] ADD  CONSTRAINT [DF_concurso_funcao_ativo]  DEFAULT ((1)) FOR [ativo]
GO

ALTER TABLE [dbo].[concurso_funcao]  WITH CHECK ADD  CONSTRAINT [FK_concurso_funcao_concurso] FOREIGN KEY([concurso_id])
REFERENCES [dbo].[concurso] ([id])
GO

ALTER TABLE [dbo].[concurso_funcao] CHECK CONSTRAINT [FK_concurso_funcao_concurso]
GO


