docker image rm zanfranceschi/desafio-01-cotacoes
docker build -t zanfranceschi/desafio-01-cotacoes .
docker push zanfranceschi/desafio-01-cotacoes
docker run --rm -p 8080:80 zanfranceschi/desafio-01-cotacoes