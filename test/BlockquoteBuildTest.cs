namespace test;

public class BlockquoteBuildBuildTest
{
    [Theory]
    [InlineData(default, default)]
    [InlineData("", "")]
    [InlineData(">[!NOTE]", "<blockquote>[!NOTE]</blockquote>")]
    [InlineData("> [!TIP]", "<blockquote>[!TIP]</blockquote>")]
    [InlineData(">  [!IMPORTANT]", "<blockquote>[!IMPORTANT]</blockquote>")]
    [InlineData(">    [!WARNING]", "<blockquote>[!WARNING]</blockquote>")]
    [InlineData(">     [!CAUTION]", "<blockquote>[!CAUTION]</blockquote>")]
    [InlineData(
@">paragraph 1
>paragraph 1.1", 
@"<blockquote>paragraph 1
paragraph 1.1</blockquote>")]
    [InlineData(
@"> paragraph 1 
>   paragraph 1.1       ", 
@"<blockquote>paragraph 1 
paragraph 1.1       </blockquote>")]
    [InlineData(
@">paragraph 1
>paragraph 1.1

>paragraph 2", 
@"<blockquote>paragraph 1
paragraph 1.1

</blockquote><blockquote>paragraph 2</blockquote>")]
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
>No geral, os *feature toggles* aumentam a agilidade e a segurança nas implementações.", @"
[Gpt](https://chatgpt.com/) Descreva pra mim por gentileza a definição e uso de feature foogles sem qualquer detalhe técnico ou de sowftware por gentileza?

<blockquote># Feature Toggles

*Feature toggles*, ou *feature flags*, são uma maneira de controlar a exibição de funcionalidades em um sistema. Elas permitem que você ative ou desative recursos sem precisar fazer mudanças complexas.

Por exemplo, se você tem uma nova funcionalidade que ainda não quer que todos vejam, pode mantê-la oculta até que esteja pronta.

## Vantagens

- **Testes**: Útil para testar novidades com um grupo menor de pessoas antes de lançar para todos.
- **Ajustes**: Facilita ajustes com base no feedback.
- **Gerenciamento**: Ajuda a gerenciar como e quando as novas funcionalidades são apresentadas aos usuários.

No geral, os *feature toggles* aumentam a agilidade e a segurança nas implementações.</blockquote>")]
    [InlineData(
@"<h2>Minha trajetória

</h2>>[!TIP]
>Conheça também a trajetória do [Ewerton](https://eportella.github.io/trajetoria/)

-", 
@"<h2>Minha trajetória

</h2><blockquote>[!TIP]
Conheça também a trajetória do [Ewerton](https://eportella.github.io/trajetoria/)

</blockquote>-")]
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