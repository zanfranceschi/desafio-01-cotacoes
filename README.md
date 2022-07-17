# Desafio 01: Cotações
Esse repositório faz parte do desafio disponível [nessa thread do twitter](https://twitter.com/zanfranceschi/status/1548344242010869763) e também [nesse post de dev.to](https://dev.to/zanfranceschi/desafio-integracao-com-apis-4jco).

Por favor, note que o código disponível aqui não tem qualidade de produção e não deveria ser usado para referência sobre como desenvolver uma API ou um mecanismo de callback.

## Organização do Repositório
- As APIs propostas para consumo no desafio estão em [apis-cotacao/src](./apis-cotacao/src)
- Uma proposta de resolução está disponível em [resolucao-desafio/src](./resolucao-desafio/src)


## APIs de Apoio ao Desafio

O restante desse README é dedicado apenas às instruções sobre as APIs de apoio ao desafio. Ou seja, as APIs que você deveria consumir para completar o desafio.

### Docker
Para executar a [imagem docker](https://hub.docker.com/repository/docker/zanfranceschi/desafio-01-cotacoes) diretamente do dockerhub (sem precisar fazer build):
~~~
docker run --rm -p 8080:80 zanfranceschi/desafio-01-cotacoes
~~~

Se preferir fazer o build da imagem, siga os passos à seguir.
~~~
cd ./apis-cotacao/src
~~~

Para construir a imagem docker, execute:
~~~
docker build -t desafio-01-cotacoes .
~~~

Para executar um container, execute:
~~~
docker run --rm -p 8080:80 desafio-01-cotacoes
~~~


### Serviços
Os exemplos têm como premissa que você esteja executando o docker na porta 8080.

#### Serviço A

Requisição:
~~~
GET http://localhost:8080/servico-a/cotacao?moeda=USD
~~~

Resposta:
~~~
HTTP 200
{
	"cotacao": 2.674,
	"moeda": "USD",
	"symbol": "💵"
}
~~~


#### Serviço B

Requisição:
~~~
GET http://localhost:8080/servico-b/cotacao?curr=USD
~~~

Resposta:
~~~
HTTP 200
{
	"cotacao": {
		"fator": 1000,
		"currency": "x",
		"valor": "1468"
	}
}
~~~


#### Serviço C

Requisição:
~~~
POST http://localhost:8080/servico-c/cotacao
{
	"tipo": "EUR",
	"callback": "http://172.17.0.1:3000"
}
~~~

Resposta:
~~~
HTTP 202
{
	"mood": "✅",
	"cid": "74e3fb63-5621-46fd-85d1-56e4e9c04a3a",
	"mensagem": "Quando a cotação finalizar, uma requisição para http://172.17.0.1:3000 será feita."
}
~~~

Requisição do Callback
~~~
POST <URL informada em "callback" da requisição>
{
    "cid": "74e3fb63-5621-46fd-85d1-56e4e9c04a3a",
    "f": 1000,
    "t": "EUR",
    "v": 3.675
}
~~~

## Proposta de Resolução
Uma possível proposta de resolução está disponível em [resolucao-desafio/src](./resolucao-desafio/src)

Para testar, faça a seguinte requisição:
~~~
GET /cotacoes/{moeda}
~~~

A resposta será algo como:
~~~
HTTP 200
{
	"cotacao": 1.689,
	"moeda": "EUR",
	"comparativo": "BRL"
}
~~~

A cotação do Serviço C só é levada em conta se houver um callback em até 5 segundos. Caso contrário, o processamento desiste da cotação desse serviço. Para a comunicação inproc/intra-threads, a biblioteca [NetMQ](https://github.com/zeromq/netmq) foi usada.