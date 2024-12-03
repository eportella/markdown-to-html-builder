namespace test;

public class LiBuildTest
{
    [Theory]
    [InlineData(
@"
- list item
",
@"
<li>list item
</li>")]
[InlineData(
@"<ul>- list item
</ul>",
@"<li>list item
</li>")]
    [InlineData(
@"
- Conheça a minha [trajetória](trajetoria/README.md).
- Conheça a [Portella LTDA](https://portella-ltda.github.io/).
- Conheça o [Código fonte](https://github.com/eportella/eportella.github.io) dessa página.",
@"
<li>Conheça a minha [trajetória](trajetoria/README.md).
</li><li>Conheça a [Portella LTDA](https://portella-ltda.github.io/).
</li><li>Conheça o [Código fonte](https://github.com/eportella/eportella.github.io) dessa página.</li>")]
    [InlineData(
@"
- Fui constituida em 24/06/2016 por [Ewerton](https://github.com/eportella).
- Meu 1º ano foi na [Envision Tecnologia](https://www.envisiontecnologia.com.br/) onde conheci o setor de turismo, como fui contratada com um scopo específico esse desafio durou 8 meses;
- Ainda no 1º ano fui indicada pela própria [Envision Tecnologia](https://www.envisiontecnologia.com.br/) para atuar na [Trade Tours](https://tradetours.com.br/) e [Trade Tech](https://tradetech.com.br/). Lá além de sustentar os sistemas já existentes contribui ativamente e principalmente até os meus 4º ano no desenvolvimento dos produtos;
    - [Reservecar](https://reservecar.com.br/)
    - [Seus Ingressos](https://seusingressos.com.br/)
    - [Horas Mágicas](https://horasmagicas.com/)
- No meu 4º ano em parceria com a [Logical Minds](https://www.logicalminds.com.br/) contribui para a **Edenred Soluções pré pagas** hoje conhecida como [Edenred Pay](https://www.edenredpay.com.br/) do grupo [Edenred](https://www.edenred.com.br/). Na ocasião meus principais desafios foram;
    - Contribuir com o desenvolvimento da **Conta Digital Pessoa Jurídica**;
    - Contribuir com e liderar o time Faturamento no desenvolvimento da **sustentação** e ***Upgrade*** **da plataforma de integração com o backoffice global**.
- Próximo ao 6º ano iniciei meu relacionamento com a [Crefisa](https://www.crefisa.com.br/), onde em parceria com a [200 DEV](https://200dev.com/) sigo com um único porém grande desafio.
    - Contribuir com e liderar o desenvolvimento da migração de um conjunto de módulos ""*middle office*"" com âmbito nacional voltado para gestão administrativa.
- Hoje estou no <span id=""portella-idade"">`tempo atual`</span>º ano da minha tragetória.
",
@"
<li>Fui constituida em 24/06/2016 por [Ewerton](https://github.com/eportella).
</li><li>Meu 1º ano foi na [Envision Tecnologia](https://www.envisiontecnologia.com.br/) onde conheci o setor de turismo, como fui contratada com um scopo específico esse desafio durou 8 meses;
</li><li>Ainda no 1º ano fui indicada pela própria [Envision Tecnologia](https://www.envisiontecnologia.com.br/) para atuar na [Trade Tours](https://tradetours.com.br/) e [Trade Tech](https://tradetech.com.br/). Lá além de sustentar os sistemas já existentes contribui ativamente e principalmente até os meus 4º ano no desenvolvimento dos produtos;
    - [Reservecar](https://reservecar.com.br/)
    - [Seus Ingressos](https://seusingressos.com.br/)
    - [Horas Mágicas](https://horasmagicas.com/)
</li><li>No meu 4º ano em parceria com a [Logical Minds](https://www.logicalminds.com.br/) contribui para a **Edenred Soluções pré pagas** hoje conhecida como [Edenred Pay](https://www.edenredpay.com.br/) do grupo [Edenred](https://www.edenred.com.br/). Na ocasião meus principais desafios foram;
    - Contribuir com o desenvolvimento da **Conta Digital Pessoa Jurídica**;
    - Contribuir com e liderar o time Faturamento no desenvolvimento da **sustentação** e ***Upgrade*** **da plataforma de integração com o backoffice global**.
</li><li>Próximo ao 6º ano iniciei meu relacionamento com a [Crefisa](https://www.crefisa.com.br/), onde em parceria com a [200 DEV](https://200dev.com/) sigo com um único porém grande desafio.
    - Contribuir com e liderar o desenvolvimento da migração de um conjunto de módulos ""*middle office*"" com âmbito nacional voltado para gestão administrativa.
</li><li>Hoje estou no <span id=""portella-idade"">`tempo atual`</span>º ano da minha tragetória.
</li>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new HtmlLiStringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlLiStringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}