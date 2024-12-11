Essa é a **lista não ordenada**, ela pode ser usada em diversos pontos da página, mas sempre no início de cada linha. O texto dela pode conter diversas variações conforme os exemplos a seguir.

>[!TIP]
>A forma como esses exemplos foram implementados podem ser consultados nesse [código fonte](https://github.com/eportella/markdown-to-html-builder/tree/main/ul/README.md)

## uma lista com três itens

- item um
- item dois
- item três

## uma lista com cinco itens contendo sub níveis

- item um
- item dois
    - subitem um do item dois
    - subitem dois do item um
- item três
    - subitem um do item três
        - subitem um do subitem um
            - subitem um do subitem um
            - subitem dois do subitem um
            - subitem três do subitem um
- item quatro
- item cinco

## uma lista com cinco itens contendo sub níveis sendo alguns deles listas ordenadas

- item um
- item dois
    1. subitem um do item dois
    1. subitem dois do item um
- item três
- item quatro
- item cinco
    - subitem um do item cinco
        - subitem um do subitem um
            1. subitem um do subitem um
            1. subitem dois do subitem um
                - subitem um do subitem dois
            1. subitem três do subitem um
    - item dois do item cinco
    - item três do item cinco

## uma lista com um único item, contendo uma grande quantidade de texto e variação de formatação
    
- item um contendo uma grande quantidade de texto e com uma grande quantidade de formatação palavras em *itálico*, **negrito** e ***itálico mais negrito***. Uma citação[^1] em rodapé e um [hiperlink](/README.md)

[^1]: 1ª citação de apoio qualquer