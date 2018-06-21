USE [tgv_prd_ipefae]
GO

/****** Object:  Table [dbo].[concurso_local_colaborador]    Script Date: 28/04/2018 17:01:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[concurso_local_colaborador](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[concurso_local_id] [int] NOT NULL,
	[colaborador_id] [int] NOT NULL,
	[funcao_id] [int] NOT NULL,
	[valor] [numeric](9, 2) NOT NULL,
	[inss] [bit] NOT NULL,
	[iss] [bit] NOT NULL,
	[ativo] [bit] NOT NULL,
 CONSTRAINT [PK_concurso_colaborador] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[concurso_local_colaborador] ADD  CONSTRAINT [DF_concurso_colaborador_inss]  DEFAULT ((1)) FOR [inss]
GO

ALTER TABLE [dbo].[concurso_local_colaborador] ADD  CONSTRAINT [DF_concurso_colaborador_iss]  DEFAULT ((1)) FOR [iss]
GO

ALTER TABLE [dbo].[concurso_local_colaborador] ADD  CONSTRAINT [DF_concurso_colaborador_ativo]  DEFAULT ((1)) FOR [ativo]
GO

ALTER TABLE [dbo].[concurso_local_colaborador]  WITH CHECK ADD  CONSTRAINT [FK_concurso_local_colaborador_colaborador] FOREIGN KEY([colaborador_id])
REFERENCES [dbo].[colaborador] ([id])
GO

ALTER TABLE [dbo].[concurso_local_colaborador] CHECK CONSTRAINT [FK_concurso_local_colaborador_colaborador]
GO

ALTER TABLE [dbo].[concurso_local_colaborador]  WITH CHECK ADD  CONSTRAINT [FK_concurso_local_colaborador_concurso_funcao] FOREIGN KEY([funcao_id])
REFERENCES [dbo].[concurso_funcao] ([id])
GO

ALTER TABLE [dbo].[concurso_local_colaborador] CHECK CONSTRAINT [FK_concurso_local_colaborador_concurso_funcao]
GO

ALTER TABLE [dbo].[concurso_local_colaborador]  WITH CHECK ADD  CONSTRAINT [FK_concurso_local_colaborador_concurso_local] FOREIGN KEY([concurso_local_id])
REFERENCES [dbo].[concurso_local] ([id])
GO

ALTER TABLE [dbo].[concurso_local_colaborador] CHECK CONSTRAINT [FK_concurso_local_colaborador_concurso_local]
GO


