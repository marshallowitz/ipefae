function gerarSenha()
{
    var url = homePage + 'Home/Neotrading_Senha_Gerar';
    var email = $('#txtEmail').val();
    var nome = $('#txtNome').val();
    var codigo = $('#txtCodigo').val();

    if ((email === '' || email === null || email === undefined) || (nome === '' || nome === null || nome === undefined) || (codigo === '' || codigo === null || codigo === undefined))
    {
        alert('Todos os campos precisam ser preenchidos');
        return;
    }

    $.ajax({
        type: "POST",
        url: url,
        data: { email: email, nome: nome, codigo: codigo },
        success: function (retorno)
        {
            if (retorno.Sucesso)
            {
                $('#txtEmail').val('');
                $('#txtNome').val('');
                $('#txtCodigo').val('');
                $('#lblSenhaGerada').text("A senha para o código " + codigo + " foi gerada com sucesso: " + retorno.Senha);
            }
            else
                alert("Ocorreu algum erro durante o processo");
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'gerarSenha()', ResponseText: xhr.responseText });
        }
    });
}