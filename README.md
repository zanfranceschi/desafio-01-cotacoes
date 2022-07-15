# Desafio 01: Cotações

## Docker
Para executar a imagem docker diretamente do dockerhub (sem precisar fazer build):

```docker run --rm -p 8080:80 zanfranceschi/desafio-01-cotacoes```


Se preferir fazer o build da imagem, siga os passos à seguir.

```cd src/```


Para construir a imagem docker, execute:

```docker build -t desafio-01-cotacoes .```


Para executar a imagem, execute:

```docker run --rm -p 8080:80 desafio-01-cotacoes```
