Essa é a **lista ordenada**, ela pode ser usada em diversos pontos da página, mas sempre no início de cada linha. O texto dela pode conter diversas variações conforme os exemplos a seguir.

>[!TIP]
>A forma como esses exemplos foram implementados podem ser consultados nesse [código fonte](https://github.com/eportella/markdown-to-html-builder/tree/main/ol/README.md)

## uma lista com três itens

- item um
- item dois
- item três

## uma lista com cinco itens contendo sub níveis

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

## uma lista com cinco itens contendo sub níveis sendo alguns deles listas não ordenadas

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

## uma lista com um único item, contendo uma grande quantidade de texto e variação de formatação
    
1. item um contendo uma grande quantidade de texto e com uma grande quantidade de formatação palavras em *itálico*, **negrito** e ***itálico mais negrito***. Uma citação[^1] em rodapé e um [hiperlink](/README.md)

[^1]: 1ª citação de apoio qualquer
