# Manual do Sistema eSocial — Stark Industries

## A empresa que usaremos como exemplo

**Stark Industries** é uma empresa fictícia de tecnologia avançada fundada por **Tony Stark (Homem de Ferro)**. A empresa tem filiais em várias cidades, centenas de funcionários e é obrigada por lei a enviar informações trabalhistas ao governo brasileiro pelo sistema **eSocial**.

### Equipe que usa o sistema

| Personagem | Papel na empresa | O que faz no sistema |
|---|---|---|
| **Tony Stark** (Homem de Ferro) | Dono / CEO | Assina digitalmente os documentos (tem o certificado) |
| **Pepper Potts** | Diretora de RH | Aprova os eventos antes do envio |
| **Peter Parker** (Homem-Aranha) | Analista de TI | Opera o sistema no dia a dia |
| **Bruce Banner** (Hulk) | Contador | Gera os eventos de folha de pagamento |
| **Natasha Romanoff** (Viúva Negra) | Gerente de RH | Cadastra admissões e demissões |
| **Steve Rogers** (Capitão América) | Supervisor de Operações | Consulta os resultados dos envios |
| **Clark Kent** (Super-Homem) | Funcionário | Aparece nos eventos como trabalhador |
| **Bruce Wayne** (Batman) | CFO (Diretor Financeiro) | Aprova a folha |
| **Barry Allen** (Flash) | Analista de Folha | Lança as rubricas salariais |
| **Diana Prince** (Mulher-Maravilha) | Jurídica | Verifica conformidade das informações |

---

## O que é o eSocial?

Imagine que toda vez que a **Stark Industries** contrata alguém, demite, paga salário, aplica uma multa ou registra qualquer informação trabalhista, ela precisa contar isso para o governo. Antes do eSocial, a empresa precisava entregar essa informação em vários sistemas diferentes — RAIS, CAGED, SEFIP, GFIP, etc. Era uma bagunça.

O **eSocial** veio para unificar tudo isso. Agora a empresa manda tudo em um lugar só, num formato específico, e o governo recebe de forma organizada.

Pense no eSocial como o **WhatsApp do governo**. A empresa manda mensagens (chamadas de *eventos*) em um formato específico (XML), e o governo responde confirmando se recebeu e processou tudo.

---

## Glossário — O que significa cada sigla

### Siglas do mundo real (governo e documentos)

**CNPJ** — Cadastro Nacional da Pessoa Jurídica
> O "CPF da empresa". Todo negócio que existe legalmente no Brasil tem um CNPJ. É um número de 14 dígitos. A Stark Industries teria algo como `12.345.678/0001-99`.

**CPF** — Cadastro de Pessoas Físicas
> O número de identificação de cada cidadão brasileiro. São 11 dígitos. O CPF de Clark Kent (Super-Homem) seria `123.456.789-09`.

**ICP-Brasil** — Infraestrutura de Chaves Públicas Brasileira
> É a entidade do governo que "reconhece firma" digitalmente. Quando Tony Stark assina um documento digital, o ICP-Brasil é quem garante que foi mesmo ele que assinou.

**INSS** — Instituto Nacional do Seguro Social
> A previdência social. Parte do salário de Clark Kent todo mês vai para o INSS. O eSocial informa ao governo quanto foi descontado.

**FGTS** — Fundo de Garantia do Tempo de Serviço
> Uma poupança obrigatória que o empregador deposita mensalmente em nome do funcionário. Se Clark Kent for demitido sem justa causa, ele pode sacar esse dinheiro.

**PIS/PASEP** — Programas de integração social
> Outro número de identificação do trabalhador, ligado à previdência. Aparece nos recibos como **NIS** (Número de Inscrição Social).

**NIS** — Número de Inscrição Social
> O mesmo que PIS. É o número que identifica o trabalhador nos sistemas previdenciários.

**SEFAZ** — Secretaria da Fazenda
> O "departamento de finanças" do governo estadual. Não aparece muito aqui, mas é parte do contexto de obrigações fiscais.

**NT** — Nota Técnica
> Um documento oficial que o governo publica para explicar mudanças nas regras do eSocial. Exemplo: "NT 03.2025" significa uma nota técnica publicada em 2025.

---

### Siglas da tecnologia

**API** — Application Programming Interface (Interface de Programação de Aplicativos)
> Imagine um garçom num restaurante. Você (o cliente) não vai direto à cozinha pedir comida — você fala com o garçom, que leva seu pedido e traz o resultado. A API é esse garçom: um intermediário que permite que dois sistemas conversem sem precisar conhecer os detalhes internos um do outro.

**REST** — Representational State Transfer
> É um estilo de construir APIs. Pense numa lista de endereços (URLs) onde cada um representa uma ação: `/api/lotes` para enviar um lote, `/api/lotes/123` para consultar o lote 123. É o padrão que este sistema usa para sua API.

**HTTP / HTTPS** — HyperText Transfer Protocol / Secure
> O protocolo de comunicação da internet. É o "idioma" que o seu navegador usa para falar com sites. O **S** no final (HTTPS) significa que a conversa é criptografada — como falar em código secreto. O eSocial só aceita HTTPS.

**SOAP** — Simple Object Access Protocol
> Outro jeito de dois sistemas conversarem, mais antigo e mais formal que REST. O governo usa SOAP no eSocial — as mensagens são enviadas em um envelope XML específico, com regras rígidas de formato. Pense como um telegrama oficial em vez de uma mensagem de WhatsApp.

**WSDL** — Web Services Description Language
> O "manual de instruções" de um serviço SOAP. Ele descreve o que o serviço faz, que informações aceita e o que devolve. É um arquivo XML que os desenvolvedores usam para saber como chamar o serviço.

**XML** — eXtensible Markup Language
> Uma linguagem para organizar dados em texto. Parecido com HTML (a linguagem das páginas web), mas usado para trocar dados entre sistemas. Todo evento do eSocial é um arquivo XML. Exemplo resumido:
> ```xml
> <evento>
>   <funcionario>Clark Kent</funcionario>
>   <salario>5000.00</salario>
> </evento>
> ```

**XSD** — XML Schema Definition
> O "gabarito" que define como um XML deve ser. Se o XML do evento for um formulário preenchido, o XSD é o formulário em branco com as regras de preenchimento. O sistema valida cada XML contra o XSD antes de enviar.

**mTLS** — mutual Transport Layer Security (TLS mútuo)
> Normalmente quando você acessa um site seguro, só o site prova que é quem diz ser (com aquele cadeado verde no navegador). No **mTLS**, os dois lados se identificam: a Stark Industries prova ao governo que é ela mesma, e o governo prova ao sistema que é o servidor oficial do eSocial. Isso é feito com certificados digitais.

**TLS** — Transport Layer Security
> O protocolo que criptografa a comunicação na internet. É o que garante que ninguém "espiona" os dados no caminho entre a Stark Industries e o eSocial.

**PFX / P12** — PKCS#12 (Personal Information Exchange)
> O formato do arquivo do certificado digital. É como um "pen drive virtual" que contém a assinatura digital de Tony Stark. Tem senha e deve ser guardado com muito cuidado.

**X.509** — Padrão de certificado digital
> O "padrão internacional" que define como um certificado digital deve ser estruturado. Quando falamos em "certificado X.509", é como dizer "documento no padrão passaporte" — todo mundo sabe o que esperar.

**Thumbprint** — Impressão digital do certificado
> Um código único que identifica um certificado específico, como uma impressão digital humana. Pode ser usado no lugar do arquivo `.pfx` para localizar o certificado já instalado no computador.

**SDK** — Software Development Kit
> Um pacote de ferramentas para desenvolvedores. O **.NET SDK** é o kit que permite criar e rodar aplicações .NET, incluindo este sistema. É o que Peter Parker instala no computador para trabalhar.

**ORM** — Object-Relational Mapping
> Uma ferramenta que "traduz" entre o mundo dos objetos do programa e o mundo das tabelas do banco de dados. Em vez de escrever SQL na mão, o desenvolvedor trabalha com objetos normais e o ORM cuida da tradução.

**EF / EF Core** — Entity Framework Core
> O ORM que este projeto usa. É a biblioteca .NET que faz a conversa com o banco de dados MySQL.

**MySQL** — Sistema de banco de dados
> Um banco de dados relacional (pense em várias planilhas do Excel conectadas entre si). O sistema guarda aqui o histórico de todos os lotes enviados, eventos, status, protocolos, etc.

**NuGet** — Gerenciador de pacotes do .NET
> É como uma "loja de aplicativos" para desenvolvedores .NET. Quando Peter Parker precisa de uma biblioteca pronta (como MediatR ou FluentValidation), ele a busca no NuGet.

---

### Siglas da arquitetura do sistema

**DDD** — Domain-Driven Design (Design Orientado ao Domínio)
> Uma filosofia de construção de software. Em vez de pensar primeiro em banco de dados ou telas, você pensa primeiro nas regras do negócio. O "domínio" do nosso sistema é o mundo do eSocial: lotes, eventos, empregadores, protocolos. O código espelha esse vocabulário.

**CQRS** — Command Query Responsibility Segregation (Separação de Responsabilidade de Comandos e Consultas)
> Uma regra simples: **comandos** mudam dados (enviar um lote), **consultas** só leem dados (consultar o resultado). Eles seguem caminhos separados no código para ficarem mais organizados. É como separar a caixa de entrada da caixa de saída.

**DTO** — Data Transfer Object (Objeto de Transferência de Dados)
> Um "envelope" que carrega dados de um lugar para outro no sistema, sem lógica de negócio dentro. Quando Peter Parker envia um lote pela API, os dados chegam em um `LoteDto` — um DTO.

**MediatR** — Biblioteca de mediação
> Um intermediário interno do sistema. Em vez de uma parte do código chamar diretamente outra, ela manda uma mensagem para o MediatR, que entrega para quem sabe processar. É como uma central de recados interna.

**API REST** — veja as definições de API e REST acima.

---

### Siglas e campos do protocolo eSocial

**tpInsc** — tipo de Inscrição
> Campo no XML que diz se o número de identificação é um CNPJ (valor `1`) ou CPF (valor `2`).

**nrInsc** — número de Inscrição
> Campo no XML com o número em si (os 14 dígitos do CNPJ ou os 11 do CPF).

**tpAmb** — tipo de Ambiente
> Campo que diz se o envio é para **Produção** (`1`) ou **Homologação** (`2`).

**cdResposta** — código de Resposta
> Um número que o governo devolve para dizer o que aconteceu. Exemplos:
> - `201` → Lote recebido com sucesso
> - `202` → Lote em processamento
> - `200` → Evento processado com sucesso
> - `4xx` → Erro na requisição (dados errados, certificado inválido, etc.)

**descResposta** — descrição da Resposta
> A versão em texto do `cdResposta`. Ex: `"Lote recebido com sucesso."` Em caso de erro, aqui vem a explicação.

**protocoloEnvio** — Protocolo de Envio
> O "número de protocolo" que o governo devolve depois que recebe um lote. É como o número de um processo judicial. Com ele, você consulta o resultado depois. Formato: `A.B.AAAAMM.NNNNN` (ex: `1.2.202503.12345`).

**nrRecibo** — número do Recibo
> Após o governo processar um evento com sucesso, ele emite um recibo. O `nrRecibo` é o número desse recibo. Guarde bem — é a prova de que o governo recebeu e aceitou o evento.

**ideEmpregador** — identificação do Empregador
> Bloco no XML que contém o CNPJ/CPF da Stark Industries.

**ideTransmissor** — identificação do Transmissor
> Bloco no XML com o CNPJ de quem está enviando o arquivo (pode ser a própria empresa ou uma contabilidade terceirizada).

**Ws** (prefixo) — WebService
> Prefixo nos nomes dos serviços SOAP. `WsEnviarLoteEventos` = WebService de Enviar Lote de Eventos.

**S-1.3** — versão do leiaute do eSocial
> O "modelo" que define como os eventos devem ser preenchidos. Atualizado periodicamente pelo governo com novas regras.

---

### Grupos de eventos

**Grupo 1 — Tabela**
> Eventos de configuração inicial, enviados uma vez ou quando mudam. Ex: cadastro da empresa (S-1000), cadastro de estabelecimentos, cargos, etc. É o "perfil" da empresa no eSocial.

**Grupo 2 — Não-Periódicos do Empregador**
> Eventos que acontecem quando algo muda: admissão (S-2200), alteração de contrato (S-2205), demissão (S-2299), acidente de trabalho, etc. Natasha Romanoff (Viúva Negra) envia esses quando contrata ou demite alguém.

**Grupo 3 — Periódicos do Empregador**
> Eventos mensais: folha de pagamento (S-1200), pagamentos de autônomos (S-1210), etc. Bruce Banner (Hulk) e Barry Allen (Flash) geram esses todo mês.

---

### Status de um lote no sistema

| Status | O que significa |
|---|---|
| **Pendente** | Lote criado, mas ainda não enviado ao governo |
| **Enviado** | Enviado ao governo, aguardando processamento (temos o protocolo) |
| **Processado** | Governo processou e aceitou. Temos os recibos |
| **Rejeitado** | Governo recebeu mas rejeitou (erro nas informações) |
| **Erro** | Falha na comunicação — nem chegou ao governo |

---

## Passo a Passo — Como usar o sistema

### Antes de começar: o que você precisa ter em mãos

1. **O sistema rodando** (o Peter Parker já deve ter configurado)
2. **O certificado digital** da Stark Industries (arquivo `.pfx` e a senha — guarde bem)
3. **O XML do evento** que você quer enviar (o contador Bruce Banner normalmente gera isso)

---

### PASSO 1 — Verificar se o sistema está funcionando

Abra o navegador e acesse:

```
http://localhost:5052/swagger
```

Se abrir o Swagger UI, o sistema está no ar.

---

### PASSO 2 — Configurar a empresa (apenas na primeira vez)

Antes de enviar qualquer evento, o sistema precisa conhecer a Stark Industries. Isso é feito editando o arquivo de configuração do ambiente.

Peter Parker abre o arquivo `appsettings.Development.json` e preenche:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Port=3306;Database=esocial_dev;User=esocial;Password=esocial;"
  },
  "ESocial": {
    "Ambiente": 2,
    "Certificado": {
      "CaminhoArquivoPfx": "C:/Certificados/stark-industries.pfx",
      "SenhaPfx": "senhaDoTonySpark123"
    },
    "SchemasPath": "C:/Schemas/eSocial/2025-04-22/01_03_00"
  }
}
```

**Traduzindo cada linha:**
- `Ambiente: 2` → estamos em Homologação (ambiente de testes). Quando for para o ar de verdade, vira `1` (Produção).
- `CaminhoArquivoPfx` → onde está o arquivo do certificado digital do Tony Stark.
- `SenhaPfx` → a senha do certificado (nunca compartilhe esta senha).
- `SchemasPath` → pasta com os gabaritos (XSDs) para validar os XMLs.

---

### PASSO 3 — Criar e enviar um lote de eventos

**Situação:** Natasha Romanoff (Viúva Negra) acabou de contratar **Clark Kent (Super-Homem)** como jornalista investigativo na Stark Industries. Ela precisa informar ao governo essa admissão pelo evento **S-2200**.

O XML do evento S-2200 foi gerado pelo sistema de RH. Agora Peter Parker vai enviá-lo.

**Chamada à API:**

```http
POST http://localhost:5052/api/lotes
Content-Type: application/json
```

**Corpo da requisição** (o que Peter Parker envia):

```json
{
  "empregadorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "numeroLote": 1,
  "grupo": 2,
  "ambiente": 2,
  "eventos": [
    {
      "tipoEvento": "evtAdmissao",
      "xmlContent": "<eSocial>... XML completo da admissão do Clark Kent ...</eSocial>"
    }
  ]
}
```

**Traduzindo:**
- `empregadorId` → o código interno da Stark Industries no nosso banco de dados.
- `numeroLote` → Peter Parker controla essa numeração. É como o número da nota fiscal.
- `grupo: 2` → Não-Periódico (admissão não é mensal, acontece quando contrata).
- `ambiente: 2` → Homologação (teste).
- `tipoEvento: "evtAdmissao"` → tipo do evento (admissão de empregado).
- `xmlContent` → o XML completo com todos os dados do Clark Kent.

**Resposta de sucesso:**

```json
{
  "loteId": "abc-123-def-456",
  "protocolo": "1.2.202503.00001",
  "sucesso": true,
  "cdResposta": "201",
  "descResposta": "Lote recebido com sucesso."
}
```

> **Atenção!** Guarde o `protocolo`. Você precisará dele para verificar o resultado. Steve Rogers vai querer saber esse número.

---

### PASSO 4 — Consultar o resultado do lote

O governo processa o lote de forma assíncrona — não é na hora. Em geral leva de alguns segundos a alguns minutos. Depois desse tempo, Steve Rogers pergunta: *"O governo aceitou?"*

Peter Parker consulta:

```http
GET http://localhost:5052/api/lotes/1.2.202503.00001?ambiente=2
```

**Resposta positiva:**

```json
{
  "protocolo": "1.2.202503.00001",
  "cdResposta": "201",
  "descResposta": "Lote processado com sucesso.",
  "sucesso": true,
  "eventos": [
    {
      "id": "ev1",
      "cdResposta": "200",
      "descResposta": "Sucesso."
    }
  ]
}
```

Ótimo! O `cdResposta: "200"` no evento confirma que a admissão do Clark Kent foi aceita. O sistema internamente já atualizou o status do lote para **Processado**.

**Resposta negativa (rejeição):**

```json
{
  "protocolo": "1.2.202503.00002",
  "cdResposta": "422",
  "descResposta": "Rejeição: CPF do trabalhador inválido.",
  "sucesso": false,
  "eventos": [
    {
      "id": "ev1",
      "cdResposta": "422",
      "descResposta": "Rejeição 0041 - CPF do Trabalhador difere do cadastrado no CPF/MF."
    }
  ]
}
```

Natasha Romanoff vai precisar corrigir o CPF do Clark Kent e reenviar.

---

### PASSO 5 — Folha de pagamento mensal (evento periódico)

Todo mês, Bruce Banner (Hulk) fecha a folha e Barry Allen (Flash) precisa enviar os eventos S-1200 (remuneração dos empregados).

Desta vez são vários funcionários, então o lote vai ter múltiplos eventos:

```json
{
  "empregadorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "numeroLote": 15,
  "grupo": 3,
  "ambiente": 2,
  "eventos": [
    {
      "tipoEvento": "evtRemun",
      "xmlContent": "<eSocial>... remuneração de Clark Kent - R$ 8.500,00 ...</eSocial>"
    },
    {
      "tipoEvento": "evtRemun",
      "xmlContent": "<eSocial>... remuneração de Bruce Wayne - R$ 45.000,00 ...</eSocial>"
    },
    {
      "tipoEvento": "evtRemun",
      "xmlContent": "<eSocial>... remuneração de Barry Allen - R$ 6.200,00 ...</eSocial>"
    }
  ]
}
```

> **Regra importante:** Um lote aceita **no máximo 50 eventos**. Se a Stark Industries tiver 300 funcionários, Barry Allen vai precisar enviar pelo menos 6 lotes (300 ÷ 50 = 6).

---

### PASSO 6 — Baixar eventos anteriores (download)

Diana Prince (Mulher-Maravilha), do jurídico, precisa de uma cópia de um evento já enviado para um processo trabalhista do Clark Kent (Super-Homem). Ela sabe o `nrRecibo` do evento.

```http
POST http://localhost:5052/api/download
Content-Type: application/json
```

```json
{
  "tipo": "PorNrRecibo",
  "tipoInscricaoEmpregador": "1",
  "nrInscricaoEmpregador": "12345678000195",
  "ambiente": 2,
  "identificadores": [
    "S-2200-12345678000195-2025-02-001"
  ]
}
```

**Traduzindo:**
- `tipo: "PorNrRecibo"` → vou buscar pelo número do recibo.
- `tipoInscricaoEmpregador: "1"` → a empresa usa CNPJ.
- `nrInscricaoEmpregador` → o CNPJ da Stark Industries (só números).
- `identificadores` → o(s) recibo(s) que Diana quer baixar.

A resposta trará o XML original do evento, que Diana pode usar como comprovante.

---

### PASSO 7 — Consultar eventos por tipo (identificadores)

Steve Rogers (Capitão América) quer saber quais admissões foram enviadas no mês passado para o funcionário Clark Kent (Super-Homem).

```http
GET /api/identificadores?tipo=Trabalhador&tipoInscricaoEmpregador=1&nrInscricaoEmpregador=12345678000195&cpfTrabalhador=12345678909&ambiente=2
```

A resposta traz a lista de recibos de todos os eventos daquele trabalhador.

---

## Resumo visual do fluxo completo

```
STARK INDUSTRIES                          GOVERNO FEDERAL (eSocial)
      │                                           │
      │  1. Monta o lote (até 50 eventos XML)     │
      │                                           │
      │──── POST /api/lotes ──────────────────────▶│
      │                                           │
      │◀─── protocolo: "1.2.202503.00001" ────────│
      │          (lote recebido, aguarde)         │
      │                                           │
      │  2. Aguarda processamento (segundos/min)  │
      │                                           │
      │──── GET /api/lotes/1.2.202503.00001 ──────▶│
      │                                           │
      │◀─── cdResposta: "201" ────────────────────│
      │     eventos: [{ cdResposta: "200" }]      │
      │          (sucesso! guarda o nrRecibo)     │
      │                                           │
      │  3. Se precisar de uma cópia depois:      │
      │                                           │
      │──── POST /api/download ───────────────────▶│
      │◀─── XML do evento original ───────────────│
```

---

## Situações de erro mais comuns

### "Certificado inválido" ou "mTLS falhou"
**O que aconteceu:** O arquivo `.pfx` não foi encontrado, a senha está errada, ou o certificado expirou.
**O que fazer:** Chamar Tony Stark (Homem de Ferro) para verificar o certificado digital. Certificados têm validade (geralmente 1 ou 3 anos).

### "cdResposta: 422" — Rejeição
**O que aconteceu:** O governo recebeu o lote, mas encontrou erros nas informações (CPF inválido, campo obrigatório faltando, data no formato errado, etc.).
**O que fazer:** Ler o `descResposta` — ele diz exatamente o que está errado. Corrigir o XML e reenviar em um **novo lote** (nunca reutilize o número de protocolo de um lote rejeitado).

### Status "Erro" no lote
**O que aconteceu:** O sistema nem conseguiu falar com o governo. Pode ser falta de internet, o servidor do eSocial está fora do ar, ou problema no certificado.
**O que fazer:** Verificar a conexão, esperar alguns minutos e tentar novamente. Se persistir, verificar o site do eSocial para avisos de manutenção.

### "The Eventos field is required"
**O que aconteceu:** A requisição foi enviada sem eventos no lote.
**O que fazer:** Incluir pelo menos um evento no array `eventos`.

---

## Dicas de segurança

1. **Nunca compartilhe a senha do certificado `.pfx` por e-mail ou WhatsApp.** O certificado tem o mesmo valor jurídico que a assinatura de Tony Stark num contrato.

2. **Os arquivos `appsettings.Staging.json` e `appsettings.Production.json` não entram no repositório de código.** Isso é intencional e protege as senhas.

3. **Em produção, o `Ambiente` deve ser `1`, nunca `2`.** Enviar dados reais em homologação pode causar problemas nos registros.

4. **Guarde os `nrRecibo` de todos os eventos processados.** Eles são a prova jurídica de que o governo recebeu. Sem ele, é difícil comprovar o envio em caso de fiscalização.

---

## Onde encontrar mais informações

| Documento | Onde está | Para quê |
|---|---|---|
| Manual do Desenvolvedor v1.15 | `Documents/DeveloperManual/Manual-2025-04-1-15.pdf` | Entender o protocolo completo |
| Leiautes S-1.3 (NT 03.2025) | `Documents/TechnicalNotes/2025-03/` | Ver todos os campos de cada evento |
| Tabelas e Enumerações | `Documents/TechnicalNotes/2025-03/Anexo I - Tabelas.pdf` | Consultar códigos e tabelas oficiais |
| Regras de Negócio | `Documents/TechnicalNotes/2025-03/Anexo II - Regras.pdf` | Entender validações do governo |
| WSDLs dos serviços | `Documents/CommunicationPackage/v1.6/WSDL/` | Detalhes técnicos dos webservices |

---

*Manual interno da Stark Industries — uso restrito. Não distribuir externamente.*
*Personagens utilizados são fictícios, pertencentes à Marvel Comics e DC Comics.*
