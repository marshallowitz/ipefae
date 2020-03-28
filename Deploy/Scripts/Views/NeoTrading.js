function obterSenha()
{
    var url = homePage + 'Home/Neotrading_ObterSenha';
    var codigo = $('#txtCodigo').val();

    if (codigo === '' || codigo === null || codigo === undefined)
    {
        alert('Todos os campos precisam ser preenchidos');
        return;
    }

    $.ajax({
        type: "POST",
        url: url,
        data: { codigo: codigo },
        success: function (retorno)
        {
            if (retorno.Sucesso)
            {
                $('#txtCodigo').val('');
                $('#lblSenhaGerada').text(retorno.Senha);
            }
            else
                alert("Ocorreu algum erro durante o processo");
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'obterSenha()', ResponseText: xhr.responseText });
        }
    });
}