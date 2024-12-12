Essa é a **lista ordenada**, ela pode ser usada em diversos pontos da página, mas sempre no início de cada linha. O texto dela pode conter diversas variações conforme os exemplos a seguir.

>[!TIP]
>A forma como esses exemplos foram implementados podem ser consultados nesse [código fonte](https://github.com/eportella/markdown-to-html-builder/blob/main/ol/README.md?plain=1)

## Uma lista com três itens

- item um
- item dois
- item três

## Uma lista com cinco itens contendo sub níveis

1. item um
1. item dois
    1. subitem um do item dois
    1. subitem dois do item um
1. item três
    1. subitem um do item três
        1. subitem um do subitem um
            1. subitem um do subitem um
            1. subitem dois do subitem um
            1. subitem três do subitem um
1. item quatro
1. item cinco

## Uma lista com cinco itens contendo sub níveis sendo alguns deles listas não ordenadas

1. item um
1. item dois
    - subitem um do item dois
    - subitem dois do item um
1. item três
1. item quatro
1. item cinco
    1. subitem um do item cinco
        - subitem um do subitem um
            - subitem um do subitem um
            - subitem dois do subitem um
                1. subitem um do subitem dois
            - subitem três do subitem um
    1. item dois do item cinco
    1. item três do item cinco

## Uma lista com um único item, contendo uma grande quantidade de texto e variação de formatação
    
1. item um contendo uma grande quantidade de texto e com uma grande quantidade de formatação palavras em *itálico*, **negrito** e ***itálico mais negrito***. Uma citação[^1] em rodapé e um [hiperlink](/README.md)

>## Uma lista com três itens dentro de uma citação em bloco
>
>- item um
>- item dois
>- item três
>
>## Uma lista com cinco itens contendo sub níveis dentro de uma citação em bloco
>
>1. item um
>1. item dois
>    1. subitem um do item dois
>    1. subitem dois do item um
>1. item três
>    1. subitem um do item três
>        1. subitem um do subitem um
>            1. subitem um do subitem um
>            1. subitem dois do subitem um
>            1. subitem três do subitem um
>1. item quatro
>1. item cinco
>
>## Uma lista com cinco itens contendo sub níveis sendo alguns deles listas não ordenadas dentro de uma citação em bloco
>
>1. item um
>1. item dois
>    - subitem um do item dois
>    - subitem dois do item um
>1. item três
>1. item quatro
>1. item cinco
>    1. subitem um do item cinco
>        - subitem um do subitem um
>            - subitem um do subitem um
>            - subitem dois do subitem um
>                1. subitem um do subitem dois
>            - subitem três do subitem um
>    1. item dois do item cinco
>    1. item três do item cinco
>
>## Uma lista com um único item, contendo uma grande quantidade de texto e variação de formatação dentro de uma citação em bloco
>    
>1. item um contendo uma grande quantidade de texto e com uma grande quantidade de formatação palavras em *itálico*, **negrito** e ***itálico mais negrito***. Uma citação[^2] em rodapé e um [hiperlink](/README.md)

>[!NOTE]
>## Uma lista com três itens dentro de um alerta como nota
>
>- item um
>- item dois
>- item três
>
>## Uma lista com cinco itens contendo sub níveis dentro de um alerta como nota
>
>1. item um
>1. item dois
>    1. subitem um do item dois
>    1. subitem dois do item um
>1. item três
>    1. subitem um do item três
>        1. subitem um do subitem um
>            1. subitem um do subitem um
>            1. subitem dois do subitem um
>            1. subitem três do subitem um
>1. item quatro
>1. item cinco
>
>## Uma lista com cinco itens contendo sub níveis sendo alguns deles listas não ordenadas dentro de um alerta como nota
>
>1. item um
>1. item dois
>    - subitem um do item dois
>    - subitem dois do item um
>1. item três
>1. item quatro
>1. item cinco
>    1. subitem um do item cinco
>        - subitem um do subitem um
>            - subitem um do subitem um
>            - subitem dois do subitem um
>                1. subitem um do subitem dois
>            - subitem três do subitem um
>    1. item dois do item cinco
>    1. item três do item cinco
>
>## Uma lista com um único item, contendo uma grande quantidade de texto e variação de formatação dentro de um alerta como nota
>    
>1. item um contendo uma grande quantidade de texto e com uma grande quantidade de formatação palavras em *itálico*, **negrito** e ***itálico mais negrito***. Uma citação[^3] em rodapé e um [hiperlink](/README.md)

>[!TIP]
>## Uma lista com três itens dentro de um alerta como dica
>
>- item um
>- item dois
>- item três
>
>## Uma lista com cinco itens contendo sub níveis dentro de um alerta como dica
>
>1. item um
>1. item dois
>    1. subitem um do item dois
>    1. subitem dois do item um
>1. item três
>    1. subitem um do item três
>        1. subitem um do subitem um
>            1. subitem um do subitem um
>            1. subitem dois do subitem um
>            1. subitem três do subitem um
>1. item quatro
>1. item cinco
>
>## Uma lista com cinco itens contendo sub níveis sendo alguns deles listas não ordenadas dentro de um alerta como dica
>
>1. item um
>1. item dois
>    - subitem um do item dois
>    - subitem dois do item um
>1. item três
>1. item quatro
>1. item cinco
>    1. subitem um do item cinco
>        - subitem um do subitem um
>            - subitem um do subitem um
>            - subitem dois do subitem um
>                1. subitem um do subitem dois
>            - subitem três do subitem um
>    1. item dois do item cinco
>    1. item três do item cinco
>
>## Uma lista com um único item, contendo uma grande quantidade de texto e variação de formatação dentro de um alerta como dica
>    
>1. item um contendo uma grande quantidade de texto e com uma grande quantidade de formatação palavras em *itálico*, **negrito** e ***itálico mais negrito***. Uma citação[^4] em rodapé e um [hiperlink](/README.md)

>[!IMPORTANT]
>## Uma lista com três itens dentro de um alerta como importante
>
>- item um
>- item dois
>- item três
>
>## Uma lista com cinco itens contendo sub níveis dentro de um alerta como importante
>
>1. item um
>1. item dois
>    1. subitem um do item dois
>    1. subitem dois do item um
>1. item três
>    1. subitem um do item três
>        1. subitem um do subitem um
>            1. subitem um do subitem um
>            1. subitem dois do subitem um
>            1. subitem três do subitem um
>1. item quatro
>1. item cinco
>
>## Uma lista com cinco itens contendo sub níveis sendo alguns deles listas não ordenadas dentro de um alerta como importante
>
>1. item um
>1. item dois
>    - subitem um do item dois
>    - subitem dois do item um
>1. item três
>1. item quatro
>1. item cinco
>    1. subitem um do item cinco
>        - subitem um do subitem um
>            - subitem um do subitem um
>            - subitem dois do subitem um
>                1. subitem um do subitem dois
>            - subitem três do subitem um
>    1. item dois do item cinco
>    1. item três do item cinco
>
>## Uma lista com um único item, contendo uma grande quantidade de texto e variação de formatação dentro de um alerta como importante
>    
>1. item um contendo uma grande quantidade de texto e com uma grande quantidade de formatação palavras em *itálico*, **negrito** e ***itálico mais negrito***. Uma citação[^5] em rodapé e um [hiperlink](/README.md)

>[!WARNING]
>## Uma lista com três itens dentro de um alerta como atenção
>
>- item um
>- item dois
>- item três
>
>## Uma lista com cinco itens contendo sub níveis dentro de um alerta como atenção
>
>1. item um
>1. item dois
>    1. subitem um do item dois
>    1. subitem dois do item um
>1. item três
>    1. subitem um do item três
>        1. subitem um do subitem um
>            1. subitem um do subitem um
>            1. subitem dois do subitem um
>            1. subitem três do subitem um
>1. item quatro
>1. item cinco
>
>## Uma lista com cinco itens contendo sub níveis sendo alguns deles listas não ordenadas dentro de um alerta como atenção
>
>1. item um
>1. item dois
>    - subitem um do item dois
>    - subitem dois do item um
>1. item três
>1. item quatro
>1. item cinco
>    1. subitem um do item cinco
>        - subitem um do subitem um
>            - subitem um do subitem um
>            - subitem dois do subitem um
>                1. subitem um do subitem dois
>            - subitem três do subitem um
>    1. item dois do item cinco
>    1. item três do item cinco
>
>## Uma lista com um único item, contendo uma grande quantidade de texto e variação de formatação dentro de um alerta como atenção
>    
>1. item um contendo uma grande quantidade de texto e com uma grande quantidade de formatação palavras em *itálico*, **negrito** e ***itálico mais negrito***. Uma citação[^6] em rodapé e um [hiperlink](/README.md)

>[!CAUTION]
>## Uma lista com três itens dentro de um alerta como cuidado
>
>- item um
>- item dois
>- item três
>
>## Uma lista com cinco itens contendo sub níveis dentro de um alerta como cuidado
>
>1. item um
>1. item dois
>    1. subitem um do item dois
>    1. subitem dois do item um
>1. item três
>    1. subitem um do item três
>        1. subitem um do subitem um
>            1. subitem um do subitem um
>            1. subitem dois do subitem um
>            1. subitem três do subitem um
>1. item quatro
>1. item cinco
>
>## Uma lista com cinco itens contendo sub níveis sendo alguns deles listas não ordenadas dentro de um alerta como cuidado
>
>1. item um
>1. item dois
>    - subitem um do item dois
>    - subitem dois do item um
>1. item três
>1. item quatro
>1. item cinco
>    1. subitem um do item cinco
>        - subitem um do subitem um
>            - subitem um do subitem um
>            - subitem dois do subitem um
>                1. subitem um do subitem dois
>            - subitem três do subitem um
>    1. item dois do item cinco
>    1. item três do item cinco
>
>## Uma lista com um único item, contendo uma grande quantidade de texto e variação de formatação dentro de um alerta como cuidado
>    
>1. item um contendo uma grande quantidade de texto e com uma grande quantidade de formatação palavras em *itálico*, **negrito** e ***itálico mais negrito***. Uma citação[^7] em rodapé e um [hiperlink](/README.md)

[^1]: 1ª citação de apoio qualquer
[^2]: 2ª citação de apoio qualquer
[^3]: 3ª citação de apoio qualquer
[^4]: 4ª citação de apoio qualquer
[^5]: 5ª citação de apoio qualquer
[^6]: 6ª citação de apoio qualquer
[^7]: 7ª citação de apoio qualquer
