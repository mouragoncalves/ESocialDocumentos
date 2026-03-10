# ESocial — Sistema de Integração

Sistema de integração com o **eSocial** (ambiente digital unificado do governo federal brasileiro), construído com **.NET 10**, **DDD** e **Clean Architecture**. Cobre o envio e a consulta de lotes de eventos via webservice SOAP, persistência de estado em MySQL e exposição de uma API REST.

---

## Índice

- [Visão Geral do eSocial](#visão-geral-do-esocial)
- [Webservices e Protocolo](#webservices-e-protocolo)
- [Arquitetura](#arquitetura)
- [Estrutura de Projetos](#estrutura-de-projetos)
- [Pré-requisitos](#pré-requisitos)
- [Configuração](#configuração)
- [Ambientes](#ambientes)
- [Como Executar](#como-executar)
- [API REST](#api-rest)
- [Testes](#testes)
- [Migrations (EF Core)](#migrations-ef-core)
- [Documentação de Referência](#documentação-de-referência)

---

## Visão Geral do eSocial

O eSocial é o sistema do governo federal que centraliza o envio de informações trabalhistas, previdenciárias, tributárias e fiscais dos empregadores. Os eventos são agrupados em **lotes** e enviados por webservice SOAP sobre HTTPS com autenticação mútua via certificado digital (mTLS).

### Leiaute em vigor

| Componente | Versão |
|---|---|
| Leiautes dos eventos | **S-1.3** (consolidado até NT 03.2025) |
| Pacote de comunicação (WSDLs/XSDs) | **v1.6** (abril/2025) |
| Manual do Desenvolvedor | **v1.15** (abril/2025) |

### Grupos de eventos

| Grupo | Código | Exemplos |
|---|---|---|
| Tabela | 1 | S-1000, S-1010, S-1020, S-1070 |
| Não-Periódicos do Empregador | 2 | S-2200, S-2205, S-2230, S-2299 |
| Periódicos do Empregador | 3 | S-1200, S-1210, S-1300, S-2500 |

---

## Webservices e Protocolo

O pacote de comunicação **v1.6** define quatro webservices SOAP, todos exigindo:

- Transporte: **HTTPS** obrigatório (sem HTTP)
- Autenticação: **certificado digital do empregador** (mTLS — `RequireClientCertificate="true"`)
- Criptografia: **Basic256**
- Estilo: **Document-literal**

### Serviços disponíveis

| Serviço WSDL | Versão | Operações |
|---|---|---|
| `WsEnviarLoteEventos` | v1_1_0 | `EnviarLoteEventos` |
| `WsConsultarLoteEventos` | v1_1_0 | `ConsultarLoteEventos` |
| `WsConsultarIdentificadoresEventos` | v1_0_0 | `ConsultarIdentificadoresEventosEmpregador` · `ConsultarIdentificadoresEventosTrabalhador` · `ConsultarIdentificadoresEventosTabela` |
| `WsSolicitarDownloadEventos` | v1_0_0 | `SolicitarDownloadEventosPorId` · `SolicitarDownloadEventosPorNrRecibo` |

### Endereços (v1.6)

| Ambiente | Envio de Lote | Consulta de Lote |
|---|---|---|
| Produção | `https://webservices.producao.esocial.gov.br/servicos/empregador/envio/lote/v1_1_0/ServicoEnviarLoteEventos.svc` | `https://webservices.producao.esocial.gov.br/servicos/empregador/consulta/lote/v1_1_0/ServicoConsultarLoteEventos.svc` |
| Homologação | `https://webservices.homologacao.esocial.gov.br/servicos/empregador/envio/lote/v1_1_0/ServicoEnviarLoteEventos.svc` | `https://webservices.homologacao.esocial.gov.br/servicos/empregador/consulta/lote/v1_1_0/ServicoConsultarLoteEventos.svc` |

> **Nota:** Os WSDLs do pacote v1.6 usam o placeholder `endereco_ambiente_acessar_*`  — os endereços reais devem ser configurados via `appsettings` conforme o ambiente.

### Fluxo de envio

```
Empregador monta lote (até 50 eventos)
    │
    ▼
Assina cada evento com certificado digital A1/A3
    │
    ▼
POST  →  WsEnviarLoteEventos  →  retorna protocoloEnvio
    │
    ▼  (processamento assíncrono no governo)
GET   →  WsConsultarLoteEventos (por protocolo)  →  retorna cdResposta + nrRecibo por evento
```

### Schemas XSD (v1.6)

| XSD | Versão | Descrição |
|---|---|---|
| `EnvioLoteEventos` | v1_1_1 | Estrutura do lote enviado |
| `RetornoEnvioLoteEventos` | v1_1_0 | Resposta do envio (protocolo) |
| `RetornoProcessamentoLote` | v1_3_0 | Resultado do processamento (ocorrências, recibos) |
| `ConsultaLoteEventos` | v1_0_0 | Estrutura da consulta de lote |
| `RetornoEvento` | **v1_3_0** | Recibo do evento + contrato + rubricas + CPFs _(novidade v1.6)_ |

A versão **RetornoEvento v1_3_0** (introduzida no pacote v1.6) acrescentou:
- `<rubricas>` — detalhes das rubricas salariais por evento
- `<cpfs>` — informações de CPF/NIS vinculados

---

## Arquitetura

O projeto segue **DDD (Domain-Driven Design)** combinado com **Clean Architecture**, com dependências sempre apontando para o centro (Domain):

```
┌─────────────────────────────────────────────────────────┐
│                      ESocial.Api                        │
│              (Controllers, Program.cs, DI)              │
└───────────┬───────────────────────┬─────────────────────┘
            │                       │
            ▼                       ▼
┌───────────────────┐   ┌──────────────────────────────┐
│ ESocial.Infra     │   │ ESocial.Infra.WebService      │
│ EF Core + MySQL   │   │ Clientes SOAP + Adapter mTLS  │
│ XsdValidator      │   │                              │
└─────────┬─────────┘   └──────────────┬───────────────┘
          │                             │
          └──────────────┬──────────────┘
                         ▼
              ┌─────────────────────┐
              │ ESocial.Application │
              │ MediatR CQRS        │
              │ IESocialWebService  │
              │ IXmlValidator       │
              └──────────┬──────────┘
                         ▼
              ┌─────────────────────┐
              │   ESocial.Domain    │
              │  Entities / VOs     │
              │  Repositories (I)   │
              │  Domain Events      │
              └─────────────────────┘
```

### Camadas

| Projeto | Responsabilidade |
|---|---|
| **ESocial.Domain** | Entidades (`LoteEventos`, `Empregador`, `Evento`), Value Objects (`Inscricao` com validação CNPJ/CPF, `ProtocoloEnvio`), interfaces de repositório, eventos de domínio |
| **ESocial.Application** | Casos de uso via CQRS com MediatR (`EnviarLote`, `ConsultarLote`, `ConsultarIdentificadores`, `SolicitarDownload`), interfaces `IESocialWebService` e `IXmlValidator`, DTOs |
| **ESocial.Infrastructure** | EF Core + Pomelo/MySQL, configurações `OwnsOne`, repositórios, validador XSD |
| **ESocial.Infrastructure.WebService** | Proxies WCF manuais para os 4 serviços SOAP, `ESocialWebServiceAdapter` com mTLS via X.509 |
| **ESocial.Api** | Controllers REST, injeção de dependência, configuração de ambientes |

---

## Estrutura de Projetos

```
Solution.ESocial/
├── Solution.ESocial.slnx
├── .gitignore
├── src/
│   ├── ESocial.Domain/
│   │   ├── Entities/          # Empregador, LoteEventos, Evento, Transmissor
│   │   ├── ValueObjects/      # Inscricao, ProtocoloEnvio, StatusProcessamento
│   │   ├── Enums/             # TipoInscricao, GrupoEvento, AmbienteEnvio, StatusLote
│   │   ├── Repositories/      # ILoteEventosRepository, IEmpregadorRepository
│   │   ├── Services/          # IXmlAssinaturaService
│   │   └── Events/            # LoteEnviadoEvent, LoteProcessadoEvent
│   │
│   ├── ESocial.Application/
│   │   ├── UseCases/
│   │   │   ├── EnviarLote/
│   │   │   ├── ConsultarLote/
│   │   │   ├── ConsultarIdentificadores/
│   │   │   └── SolicitarDownload/
│   │   ├── Interfaces/        # IESocialWebService, IXmlValidator
│   │   └── DTOs/
│   │
│   ├── ESocial.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── ESocialDbContext.cs
│   │   │   ├── Configurations/
│   │   │   ├── Repositories/
│   │   │   └── Migrations/
│   │   └── Validation/        # XsdValidator
│   │
│   ├── ESocial.Infrastructure.WebService/
│   │   ├── Generated/         # Proxies WCF manuais (baseados nos WSDLs v1.6)
│   │   └── Adapters/          # ESocialWebServiceAdapter, CertificadoConfiguration
│   │
│   └── ESocial.Api/
│       ├── Controllers/       # LoteEventosController, IdentificadoresController, DownloadController
│       ├── Properties/        # launchSettings.json (perfis Development/Staging/Production)
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── appsettings.Staging.json          ← gitignored
│       ├── appsettings.Production.json       ← gitignored
│       ├── appsettings.Staging.json.example  ← template no git
│       └── appsettings.Production.json.example
│
└── tests/
    ├── ESocial.Domain.Tests/
    ├── ESocial.Application.Tests/
    └── ESocial.Integration.Tests/
```

---

## Pré-requisitos

| Ferramenta | Versão mínima |
|---|---|
| .NET SDK | **10.0** |
| MySQL Server | 8.0+ |
| Certificado digital | A1 ou A3 (ICP-Brasil, e-CNPJ ou e-CPF) |

---

## Configuração

### 1. Banco de dados

Crie o banco e o usuário no MySQL:

```sql
CREATE DATABASE esocial_dev CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER 'esocial'@'localhost' IDENTIFIED BY 'esocial';
GRANT ALL PRIVILEGES ON esocial_dev.* TO 'esocial'@'localhost';
FLUSH PRIVILEGES;
```

### 2. Certificado digital

O certificado digital do empregador é obrigatório para autenticação mTLS junto aos webservices do governo. Configure em `appsettings.{Ambiente}.json`:

```json
"ESocial": {
  "Certificado": {
    "CaminhoArquivoPfx": "/caminho/para/certificado.pfx",
    "SenhaPfx": "senha-do-certificado"
  }
}
```

Alternativamente, referenciando pelo thumbprint no repositório do sistema operacional:

```json
"ESocial": {
  "Certificado": {
    "Thumbprint": "AABBCC...",
    "StoreLocation": "CurrentUser",
    "StoreName": "My"
  }
}
```

### 3. Schemas XSD

Aponte `ESocial:SchemasPath` para o diretório com os schemas de validação:

```json
"ESocial": {
  "SchemasPath": "Documents/CommunicationPackage/v1.6/XSD"
}
```

Os schemas de eventos estão em `Documents/CommunicationPackage/v1.6/XSD/` e os schemas de leiaute completos (S-1.3) estão em `Schemas/2025-04-22/01_03_00/`.

---

## Ambientes

| Ambiente | `ASPNETCORE_ENVIRONMENT` | Connection String | No git? |
|---|---|---|---|
| Development | `Development` | `localhost:3306/esocial_dev` | ✅ |
| Staging | `Staging` | host de homologação | ❌ (gitignored) |
| Production | `Production` | host de produção | ❌ (gitignored) |

Para configurar Staging ou Production, copie o template e preencha os valores:

```bash
# Staging
cp src/ESocial.Api/appsettings.Staging.json.example \
   src/ESocial.Api/appsettings.Staging.json

# Production
cp src/ESocial.Api/appsettings.Production.json.example \
   src/ESocial.Api/appsettings.Production.json
```

---

## Como Executar

```bash
# Restaurar e compilar
dotnet build Solution.ESocial.slnx

# Aplicar migrations (requer MySQL em execução)
dotnet ef database update \
  --project src/ESocial.Infrastructure \
  --startup-project src/ESocial.Api

# Executar em Development (padrão)
dotnet run --project src/ESocial.Api --launch-profile Development

# Executar em Staging
dotnet run --project src/ESocial.Api --launch-profile Staging
```

| Perfil | URL HTTP | URL HTTPS |
|---|---|---|
| Development | `http://localhost:5052` | `https://localhost:7141` |
| Staging | `http://localhost:5053` | `https://localhost:7142` |
| Production | `http://localhost:5054` | `https://localhost:7143` |

A documentação OpenAPI fica disponível em: `http://localhost:5052/openapi/v1.json`

---

## API REST

### `POST /api/lotes` — Enviar lote de eventos

```json
{
  "empregadorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "numeroLote": 1,
  "grupo": 1,
  "ambiente": 2,
  "eventos": [
    {
      "tipoEvento": "evtTabRubrica",
      "xmlContent": "<eSocial xmlns=\"http://www.esocial.gov.br/schema/evt/evtTabRubrica/v04_00_01\">...</eSocial>"
    }
  ]
}
```

**Resposta 200:**
```json
{
  "loteId": "...",
  "protocolo": "1.2.202503.00012345",
  "sucesso": true,
  "cdResposta": "201",
  "descResposta": "Lote recebido com sucesso."
}
```

> Um lote aceita **até 50 eventos**. Lotes maiores devem ser quebrados em múltiplas requisições.

---

### `GET /api/lotes/{protocolo}?ambiente=2` — Consultar resultado do lote

```bash
GET /api/lotes/1.2.202503.00012345?ambiente=2
```

**Resposta 200:**
```json
{
  "protocolo": "1.2.202503.00012345",
  "cdResposta": "201",
  "descResposta": "Lote processado com sucesso.",
  "sucesso": true,
  "eventos": [
    { "id": "ev1", "cdResposta": "200", "descResposta": "Sucesso." }
  ]
}
```

---

### `GET /api/identificadores` — Consultar identificadores de eventos

```bash
GET /api/identificadores?tipo=Empregador&tipoInscricaoEmpregador=1&nrInscricaoEmpregador=12345678000195&ambiente=2
```

---

### `POST /api/download` — Solicitar download de eventos

```json
{
  "tipo": "PorNrRecibo",
  "tipoInscricaoEmpregador": "1",
  "nrInscricaoEmpregador": "12345678000195",
  "ambiente": 2,
  "identificadores": ["S-1200-12345678000195-2025-03-001"]
}
```

---

### Enumerações

| Enum | Valores |
|---|---|
| `ambiente` | `1` = Produção · `2` = Homologação |
| `grupo` | `1` = Tabela · `2` = Não-Periódicos · `3` = Periódicos |
| `tipoInscricao` | `1` = CNPJ · `2` = CPF |

---

## Testes

```bash
# Todos os testes (exceto integração)
dotnet test Solution.ESocial.slnx --filter "Category!=Integration"

# Todos os testes
dotnet test Solution.ESocial.slnx
```

| Suite | Testes | Cobertura |
|---|---|---|
| `ESocial.Domain.Tests` | 14 | Value Objects (`Inscricao` CNPJ/CPF), regras de domínio de `LoteEventos` |
| `ESocial.Application.Tests` | 3 | Handler `EnviarLote` com mocks de webservice e validador |
| `ESocial.Integration.Tests` | 1 | Smoke test contra homologação (requer certificado) |

Os testes de integração são ignorados automaticamente se a variável de ambiente `ESOCIAL_CERT_PATH` não estiver definida:

```bash
ESOCIAL_CERT_PATH=/path/to/cert.pfx \
ESOCIAL_CERT_PASS=senha \
dotnet test --filter "Category=Integration"
```

---

## Migrations (EF Core)

```bash
# Adicionar nova migration
dotnet ef migrations add <NomeDaMigration> \
  --project src/ESocial.Infrastructure \
  --startup-project src/ESocial.Api \
  --output-dir Persistence/Migrations

# Aplicar ao banco
dotnet ef database update \
  --project src/ESocial.Infrastructure \
  --startup-project src/ESocial.Api

# Reverter para migration anterior
dotnet ef database update <MigrationAnterior> \
  --project src/ESocial.Infrastructure \
  --startup-project src/ESocial.Api
```

---

## Documentação de Referência

Toda a documentação oficial está em `Documents/`:

```
Documents/
├── CommunicationPackage/
│   ├── v1.6/                                       ← versão atual
│   │   ├── WSDL/LoteEventos/                       ← WSDLs de envio e consulta
│   │   ├── WSDL/Eventos/                           ← WSDLs de identificadores e download
│   │   ├── XSD/LoteEventos/                        ← schemas de lote
│   │   ├── XSD/Eventos/RetornoEvento/              ← schema de retorno v1_3_0 + exemplo.xml
│   │   ├── XSD/Eventos/ConsultaIdentificadores/
│   │   ├── XSD/Eventos/SolicitacaoDownload/
│   │   └── Controle de Alteracoes do XSD e WSDL do eSocial.txt
│   └── v1.5/                                       ← versão anterior
│
├── DeveloperManual/
│   ├── Manual-2025-04-1-15.pdf                     ← Manual do Desenvolvedor v1.15 (atual)
│   └── Manual-2018-01-1-6-1.pdf                    ← Manual v1.6.1 (legado)
│
└── TechnicalNotes/
    ├── 2025-03/                                     ← Leiautes S-1.3 (NT 03.2025) — atual
    │   ├── Leiautes do eSocial v. S-1.3 (...).pdf
    │   ├── Anexo I - Tabelas (...).pdf
    │   └── Anexo II - Regras (...).pdf
    └── 2024-02/                                     ← Leiautes S-1.3 (NT 02.2024)
```

### Histórico de alterações do pacote de comunicação

| Versão | Principal mudança |
|---|---|
| **v1.6** (abr/2025) | `RetornoEvento` → v1_3_0: adicionado `<rubricas>` e `<cpfs>` |
| **v1.5** | Novos serviços: `WsConsultarIdentificadoresEventos` (3 ops) e `WsSolicitarDownloadEventos` (2 ops); `RetornoEvento` → v1_2_1 |
| **v1.4.1** | Endpoints tornados configuráveis via placeholder nos WSDLs |
| **v1.4.0** | `RetornoEvento`: adicionado bloco `<contrato>` nos recibos |
| **v1.3.2** | Incluído `xmldsig-core-schema.xsd` para suporte a assinatura digital |

---

> Para suporte e contribuições, consulte o Manual do Desenvolvedor em `Documents/DeveloperManual/Manual-2025-04-1-15.pdf` e os leiautes S-1.3 em `Documents/TechnicalNotes/2025-03/`.
