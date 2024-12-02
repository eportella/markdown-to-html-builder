namespace test;

public class BlockquoteBuildBuildTest
{
    [Theory]
    [InlineData(default, default)]
    [InlineData("", "")]
    [InlineData(">[!NOTE]", "<blockquote><p>[!NOTE]</p></blockquote>")]
    [InlineData("> [!TIP]", "<blockquote><p>[!TIP]</p></blockquote>")]
    [InlineData(">  [!IMPORTANT]", "<blockquote><p>[!IMPORTANT]</p></blockquote>")]
    [InlineData(">    [!WARNING]", "<blockquote><p>[!WARNING]</p></blockquote>")]
    [InlineData(">     [!CAUTION]", "<blockquote><p>[!CAUTION]</p></blockquote>")]
    [InlineData(">paragraph 1\n>paragraph 1.1", "<blockquote><p>paragraph 1</p><p>paragraph 1.1</p></blockquote>")]
    [InlineData("> paragraph 1 \n>   paragraph 1.1       ", "<blockquote><p>paragraph 1</p><p>paragraph 1.1</p></blockquote>")]
    [InlineData(">paragraph 1\n>paragraph 1.1\n\n>paragraph 2", "<blockquote><p>paragraph 1</p><p>paragraph 1.1</p></blockquote>\n<blockquote><p>paragraph 2</p></blockquote>")]
    [InlineData(@"
[Gpt](https://chatgpt.com/) Descreva pra mim por gentileza a definição e uso de feature foogles sem qualquer detalhe técnico ou de sowftware por gentileza?

># Feature Toggles
>
>*Feature toggles*, ou *feature flags*, são uma maneira de controlar a exibição de funcionalidades em um sistema. Elas permitem que você ative ou desative recursos sem precisar fazer mudanças complexas.
>
>Por exemplo, se você tem uma nova funcionalidade que ainda não quer que todos vejam, pode mantê-la oculta até que esteja pronta.
>
>## Vantagens
>
>- **Testes**: Útil para testar novidades com um grupo menor de pessoas antes de lançar para todos.
>- **Ajustes**: Facilita ajustes com base no feedback.
>- **Gerenciamento**: Ajuda a gerenciar como e quando as novas funcionalidades são apresentadas aos usuários.
>
>No geral, os *feature toggles* aumentam a agilidade e a segurança nas implementações.", "")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new BlockquoteBuildRequest
        {
            String = informed
        };

        var result = await new BlockquoteBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}